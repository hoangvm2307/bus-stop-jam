#if UNITY_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;
using CupkekGames.Core.Pool;

namespace CupkekGames.Systems
{
    public class AddressableGameObjectPool : GameObjectPoolBase
    {
        private AssetReference _assetReference;

        public AddressableGameObjectPool(AssetReference assetReference, int defaultCapacity, int maxSize, bool prewarm = true, bool collectionCheck = true) 
            : base(defaultCapacity, maxSize, collectionCheck)
        {
            _assetReference = assetReference;

            if (prewarm)
            {
                Prewarm();
                // Debug.Log($"Created Addressable GameObject pool for {assetReference} with {Pool.CountAll} objects");
            }
        }

        public override GameObject CreateObject()
        {
            return AddressableAssetManager.InstantiateSync(_assetReference);
        }

        public void Dispose()
        {   
            Pool.Dispose();
            
            // Release the addressable asset
            if (_assetReference != null)
            {
                AddressableAssetManager.DestroyAllThenRelease(_assetReference);
            }
        }
    }
}
#endif 