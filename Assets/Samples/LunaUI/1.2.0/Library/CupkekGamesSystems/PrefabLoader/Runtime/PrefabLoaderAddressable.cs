#if UNITY_ADDRESSABLES
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using CupkekGames.Core;

namespace CupkekGames.Systems
{
  public abstract class PrefabLoaderAddressable<TKey> : KeyValueDatabaseMono<TKey, AssetReference>, IPrefabLoader<TKey, AssetReference>
  {
#pragma warning disable CS0414
    [SerializeField] string _searchLabel = "Prefab"; // Used by editor code
#pragma warning restore CS0414

    public event EventHandler<TKey> OnInstanceDestroyed;

    public bool IsLoadedAndValid(TKey key)
    {
      return AddressableAssetManager.IsLoadedAndValid(GetValue(key));
    }

    // public void LoadOnly(string key)
    // {
    //   _addressableAssetManager.LoadAsset<GameObject>(_prefabs.GetValue(key));
    // }

    // public void DestroyWithoutRelease(string key)
    // {
    //   if (_addressableAssetManager.DestroyAllWithoutRelease(_prefabs.GetValue(key)))
    //   {
    //     OnUIUnloaded?.Invoke(key);
    //   }
    // }

    public GameObject Instantiate(TKey key)
    {
      if (!ContainsKey(key))
      {
        Debug.LogWarning("Key not found: " + key);

        return null;
      }

      GameObject instance = AddressableAssetManager.InstantiateSync(GetValue(key));

      AddReportDestroy(key, instance);

      return instance;
    }

    public void DestroyAllOf(TKey key)
    {
      bool unloaded = AddressableAssetManager.DestroyAllThenRelease(GetValue(key));

      if (unloaded)
      {
        OnInstanceDestroyed?.Invoke(this, key);
      }
    }

    public List<GameObject> GetInstances(TKey key)
    {
      return AddressableAssetManager.GetInstances(GetValue(key));
    }

    public IEnumerator DestroyAllOfWithDelay(TKey key, float duration = 0.5f)
    {
      yield return new WaitForSeconds(duration);

      DestroyAllOf(key);
    }

    public void AddReportDestroy(object key, GameObject instance)
    {
      if (!instance.TryGetComponent<PrefabLoaderReportDestroy>(out var report))
      {
        report = instance.AddComponent<PrefabLoaderReportDestroy>();
      }

      report.PrefabLoader = this;
      report.PrefabKey = key;
    }

    public void ReportDestroy(object key, GameObject instance)
    {
      OnInstanceDestroyed?.Invoke(this, (TKey)key);
    }

    public void DestroyAll()
    {
      foreach (var key in Keys)
      {
        DestroyAllOf(key);
      }
    }
  }
}
#endif