#if UNITY_ADDRESSABLES
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Threading.Tasks;

namespace CupkekGames.Systems
{
  public static class AddressableAssetManager
  {
#region State
    private static Dictionary<AssetReference, AsyncOperationHandle> _loadedAssetReference = new();
    private static Dictionary<AsyncOperationHandle, List<GameObject>> _instantiatedGameObjects = new();
#endregion

#region Events
    public static event Action<AssetReference, GameObject> OnInstanceDestroy;
    public static event Action<AssetReference> OnAssetUnloaded;
#endregion

#region Debug
    public static bool Test = false;
#endregion

    /// <summary>
    /// Loads an asset of type T from the given asset reference.
    /// </summary>
    /// <typeparam name="T">The type of asset to load.</typeparam>
    /// <param name="assetReference">The asset reference to load the asset from.</param>
    /// <returns>The loaded asset.</returns>
    public static object LoadAsset<T>(AssetReference assetReference)
    {
      if (assetReference == null)
      {
        Debug.LogError("LoadAsset: AssetReference is null");
        return null;
      }

      AsyncOperationHandle? handle = null;

      if (_loadedAssetReference.ContainsKey(assetReference))
      {
        if (Test)
        {
          Debug.Log("[AddressableAssetManager] [LoadAsset] Using existing handle");
        }

        handle = _loadedAssetReference[assetReference];

        if (handle.Value.IsValid() && handle.Value.Task.IsCompleted)
        {
          if (Test)
          {
            Debug.Log("[AddressableAssetManager] [LoadAsset] Returning result directly");
          }

          return handle.Value.Result;
        }
      }
      
      if (!handle.HasValue || !handle.Value.IsValid())
      {
        if (Test)
        {
          Debug.Log("[AddressableAssetManager] [LoadAsset] Creating new handle");
        }

        handle = assetReference.LoadAssetAsync<T>();
        _loadedAssetReference.Add(assetReference, handle.Value);
      }

      handle.Value.WaitForCompletion();

      if (handle.Value.Result == null || handle.Value.Status != AsyncOperationStatus.Succeeded)
      {
        Debug.LogError($"Failed to load asset {assetReference}. Status: {handle.Value.Status}");

        _loadedAssetReference.Remove(assetReference);

        return null;
      }

      if (Test)
      {
        Debug.Log("[AddressableAssetManager] [LoadAsset] Returning result after wait");
      }

      return handle.Value.Result;
    }

    public static GameObject InstantiateSync(AssetReference assetReference, Vector3? position = null, Quaternion? rotation = null)
    {
      if (!IsLoadedAndValid(assetReference))
      {
        object loadResult = LoadAsset<GameObject>(assetReference);
        if (loadResult == null)
        {
          return null;
        }
      }

      return InstantiateGameObjectWithoutLoad(assetReference, position, rotation);
    }

    private static GameObject InstantiateGameObjectWithoutLoad(AssetReference assetReference, Vector3? position = null, Quaternion? rotation = null)
    {
      AsyncOperationHandle handle;

      if (!_loadedAssetReference.ContainsKey(assetReference))
      {
        Debug.LogError($"Asset reference {assetReference} not found in loaded assets");
        return null;
      }
      handle = _loadedAssetReference[assetReference];

      if (handle.Status != AsyncOperationStatus.Succeeded)
      {
        Debug.LogError($"Failed to instantiate {assetReference}, handle status: {handle.Status}");
        return null;
      }

      if (position == null)
      {
        position = Vector3.zero;
      }
      if (rotation == null)
      {
        rotation = Quaternion.identity;
      }

      GameObject instance = GameObject.Instantiate(handle.Result as GameObject, position.Value, rotation.Value);

      if (_instantiatedGameObjects.ContainsKey(handle))
      {
        _instantiatedGameObjects[handle].Add(instance);
      }
      else
      {
        _instantiatedGameObjects.Add(handle, new List<GameObject> { instance });
      }

      AddressableAssetReportDestroy addressableAssetReleaseOnDestroy = instance.GetComponent<AddressableAssetReportDestroy>();
      if (addressableAssetReleaseOnDestroy == null)
      {
        // Add the script to the targetGameObject
        addressableAssetReleaseOnDestroy = instance.AddComponent(typeof(AddressableAssetReportDestroy)) as AddressableAssetReportDestroy;
      }
      // Only set the asset reference if it's not already set
      if (addressableAssetReleaseOnDestroy != null && !addressableAssetReleaseOnDestroy.HasAssetReference())
      {
        addressableAssetReleaseOnDestroy.SetAssetReference(assetReference);
      }

      
      if (Test)
      {
        Debug.Log("[AddressableAssetManager] [InstantiateGameObjectWithoutLoad] Success");
      }

      return instance;
    }

    /// <summary>
    /// Releases the handle and destroys all game objects associated with it.
    /// </summary>
    /// <param name="assetReference">The asset reference to release and destroy.</param>
    /// <returns>True if the release and destruction was successful, false otherwise.</returns>
    public static bool DestroyAllThenRelease(AssetReference assetReference)
    {
      if (assetReference == null)
      {
        return false;
      }

      AsyncOperationHandle handle;

      if (!_loadedAssetReference.ContainsKey(assetReference))
      {
        if (Test)
        {
          Debug.Log("[AddressableAssetManager] [DestroyAllThenRelease] AssetReference is not loaded");
        }
        return false;
      }

      // Get the handle
      handle = _loadedAssetReference[assetReference];
      // Remove the handle from the dictionary
      _loadedAssetReference.Remove(assetReference);

      if (!handle.IsValid())
      {
        if (Test)
        {
          Debug.Log("[AddressableAssetManager] [DestroyAllThenRelease] AssetReference is not valid");
        }
        return false;
      }

      // Destroy all game objects associated with the handle
      DestroyAllWithoutRelease(handle);

      // Release the handle
      Addressables.Release(handle);

      OnAssetUnloaded?.Invoke(assetReference);

      if (Test)
      {
        Debug.Log("[AddressableAssetManager] [DestroyAllThenRelease] Success");
      }

      return true;
    }

    public static bool ReportDestroy(AssetReference assetReference, GameObject instance)
    {
      if (assetReference == null || instance == null)
      {
        return false;
      }

      AsyncOperationHandle handle;

      if (!_loadedAssetReference.ContainsKey(assetReference))
      {
        return false;
      }

      handle = _loadedAssetReference[assetReference];

      if (!_instantiatedGameObjects.ContainsKey(handle))
      {
        return false;
      }

      if (!_instantiatedGameObjects[handle].Remove(instance))
      {
        return false;
      }

      OnInstanceDestroy?.Invoke(assetReference, instance);

      bool shouldRelease = false;

      if (_instantiatedGameObjects.ContainsKey(handle) && _instantiatedGameObjects[handle].Count == 0)
      {
        shouldRelease = true;
      }

      if (shouldRelease)
      {
        DestroyAllThenRelease(assetReference);
      }

      return true;
    }

    /// <summary>
    /// Checks if the specified asset reference is loaded and handle is valid.
    /// </summary>
    /// <param name="assetReference">The asset reference to check.</param>
    /// <returns><c>true</c> if the asset reference is loaded; otherwise, <c>false</c>.</returns>
    public static bool IsLoadedAndValid(AssetReference assetReference)
    {
      if (assetReference == null)
      {
        return false;
      }

      if (_loadedAssetReference.TryGetValue(assetReference, out var handle))
      {
        return handle.IsValid();
      }

      return false;
    }

    /// <summary>
    /// Gets the first instance of the specified asset reference.
    /// </summary>
    /// <param name="assetReference">The asset reference to get the instance for.</param>
    /// <returns>The first instance of the specified asset reference, or null if none exists.</returns>
    public static GameObject GetFirstInstance(AssetReference assetReference)
    {
      List<GameObject> instances = GetInstances(assetReference);

      if (instances == null || instances.Count == 0)
      {
        return null;
      }

      return instances[0];
    }

    /// <summary>
    /// Gets all instances of the specified asset reference.
    /// </summary>
    /// <param name="assetReference">The asset reference to get instances for.</param>
    /// <returns>A list of instances of the specified asset reference, or null if none exist.</returns>
    public static List<GameObject> GetInstances(AssetReference assetReference)
    {
      if (assetReference == null || !IsLoadedAndValid(assetReference))
      {
        return null;
      }

      AsyncOperationHandle handle = _loadedAssetReference[assetReference];

      if (!_instantiatedGameObjects.ContainsKey(handle))
      {
        return null;
      }

      // Return a copy of the list to prevent external modification
      return new List<GameObject>(_instantiatedGameObjects[handle]);
    }

    /// <summary>
    /// Clears invalid handles associated with the specified asset reference.
    /// If the handle is not valid, it is removed from the dictionary and all associated game objects are destroyed.
    /// </summary>
    /// <param name="assetReference">The asset reference to clear invalid handles for.</param>
    /// <returns>True if an invalid handle was removed, false otherwise.</returns>
    public static bool ClearInvalidHandles(AssetReference assetReference)
    {
      if (assetReference == null)
      {
        return false;
      }

      if (!_loadedAssetReference.ContainsKey(assetReference))
      {
        return false;
      }

      AsyncOperationHandle handle = _loadedAssetReference[assetReference];

      if (!handle.IsValid())
      {
        // Remove the handle from the dictionary
        _loadedAssetReference.Remove(assetReference);
        // Destroy all game objects associated with the handle
        DestroyAllWithoutRelease(handle);
        return true;
      }

      return false;
    }

    /// <summary>
    /// Cleans up all loaded assets and destroys all instantiated game objects.
    /// </summary>
    public static void CleanupAll()
    {
      List<AssetReference> assetReferencesToRelease = new List<AssetReference>();

      // Create a copy of the keys to avoid modifying the collection during iteration
      foreach (AssetReference assetReference in _loadedAssetReference.Keys)
      {
        assetReferencesToRelease.Add(assetReference);
      }

      // Release each asset reference
      foreach (AssetReference assetReference in assetReferencesToRelease)
      {
        DestroyAllThenRelease(assetReference);
      }
    }

    public static bool DestroyAllWithoutRelease(AssetReference assetReference)
    {
      if (assetReference == null)
      {
        return false;
      }

      AsyncOperationHandle handle;

      if (!_loadedAssetReference.ContainsKey(assetReference))
      {
        return false;
      }

      handle = _loadedAssetReference[assetReference];

      DestroyAllWithoutRelease(handle);

      return true;
    }

    /// <summary>
    /// Destroys all instantiated game objects associated with the specified AsyncOperationHandle
    /// and removes the handle from the dictionary.
    /// </summary>
    /// <param name="handle">The AsyncOperationHandle to destroy game objects for.</param>
    public static void DestroyAllWithoutRelease(AsyncOperationHandle handle)
    {
      List<GameObject> objectsToDestroy = null;

      if (_instantiatedGameObjects.ContainsKey(handle))
      {
        // Create a copy of the list to avoid modifying it while iterating
        objectsToDestroy = new List<GameObject>(_instantiatedGameObjects[handle]);
        _instantiatedGameObjects.Remove(handle);
      }

      if (objectsToDestroy != null)
      {
        foreach (GameObject instance in objectsToDestroy)
        {
          if (instance != null)
          {
            GameObject.Destroy(instance);
          }
        }
      }
    }

    /// <summary>
    /// Loads an asset asynchronously from an AssetReference.
    /// </summary>
    /// <typeparam name="T">The type of asset to load.</typeparam>
    /// <param name="assetReference">The AssetReference of the asset to load.</param>
    /// <returns>A UniTask representing the asynchronous loading operation.</returns>
    public static async Task<object> LoadAssetAsync<T>(AssetReference assetReference)
    {
      if (assetReference == null)
      {
        Debug.LogError("LoadAssetAsync: AssetReference is null");
        return null;
      }

      AsyncOperationHandle? handle = null;

      if (_loadedAssetReference.ContainsKey(assetReference))
      {
        if (Test)
        {
          Debug.Log("[AddressableAssetManager] [LoadAssetAsync] Using existing handle");
        }

        handle = _loadedAssetReference[assetReference];

        if (handle.Value.IsValid() && handle.Value.Task.IsCompleted)
        {
          if (Test)
          {
            Debug.Log("[AddressableAssetManager] [LoadAsset] Returning result directly");
          }

          return handle.Value.Result;
        }
      }

      if (!handle.HasValue || !handle.Value.IsValid())
      {
        if (Test)
        {
          Debug.Log("[AddressableAssetManager] [LoadAssetAsync] Creating new handle");
        }

        handle = assetReference.LoadAssetAsync<T>();
        _loadedAssetReference.Add(assetReference, handle.Value);
      }

      try {
        await handle.Value.Task;

        if (handle.Value.Result == null || handle.Value.Status != AsyncOperationStatus.Succeeded)
        {
          Debug.LogError($"Failed to load asset {assetReference}. Status: {handle.Value.Status}");

          _loadedAssetReference.Remove(assetReference);

          return null;
        }

        if (Test)
        {
          Debug.Log("[AddressableAssetManager] [LoadAssetAsync] Returning result after wait");
        }

        return handle.Value.Result;
      }
      catch (Exception ex)
      {
        Debug.LogError($"Exception while loading asset {assetReference} asynchronously: {ex.Message}");

        if (_loadedAssetReference.ContainsKey(assetReference))
        {
          _loadedAssetReference.Remove(assetReference);
        }

        return null;
      }
    }

    /// <summary>
    /// Instantiates a GameObject asynchronously from an AssetReference.
    /// </summary>
    /// <param name="assetReference">The AssetReference of the GameObject to instantiate.</param>
    /// <param name="position">The position of the instantiated GameObject. If null, the default position is Vector3.zero.</param>
    /// <param name="rotation">The rotation of the instantiated GameObject. If null, the default rotation is Quaternion.identity.</param>
    /// <returns>The instantiated GameObject.</returns>
    public static async Task<GameObject> InstantiateAsync(AssetReference assetReference, Vector3? position = null, Quaternion? rotation = null)
    {
      if (assetReference == null)
      {
        Debug.LogError("InstantiateAsync: AssetReference is null");
        return null;
      }

      try
      {
        if (!IsLoadedAndValid(assetReference))
        {
          object loadResult = await LoadAssetAsync<GameObject>(assetReference);
          if (loadResult == null)
          {
            Debug.LogError($"Failed to load asset {assetReference} for instantiation");
            return null;
          }
        }

        return InstantiateGameObjectWithoutLoad(assetReference, position, rotation);
      }
      catch (Exception ex)
      {
        Debug.LogError($"Exception while instantiating asset {assetReference} asynchronously: {ex.Message}");
        return null;
      }
    }
  }
}
#endif
