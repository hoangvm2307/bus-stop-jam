#if UNITY_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;
using CupkekGames.Core.Pool;
using System.Collections.Generic;

namespace CupkekGames.Systems.Test
{
    public class TestAddressableGameObjectPool : MonoBehaviour
    {
        [SerializeField] private AssetReference _assetReference;
        [SerializeField] private int _defaultCapacity = 5;
        [SerializeField] private int _maxSize = 10;
        [SerializeField] private bool _prewarm = true;
        
        private AddressableGameObjectPool _pool;

        [Header("Debug List, don't edit")]
        [SerializeField] private List<GameObject> _spawned;

        private void Awake()
        {
            AddressableAssetManager.Test = true;
            Recreate();
        }

        public void Spawn()
        {
            GameObject gameObject = _pool.Pool.Get();
            _spawned.Add(gameObject);

            Vector3 randomPosition = new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(0f, 3f),
                Random.Range(-5f, 5f)
            );

            gameObject.transform.SetPositionAndRotation(randomPosition, Random.rotation);
            gameObject.SetActive(true);
        }

        public void Despawn()
        {
            if (_spawned.Count > 0)
            {
                GameObject gameObject = _spawned[0];
                _spawned.RemoveAt(0);
                gameObject.SetActive(false);
            }
        }

        public void Dispose()
        {
            _pool.Dispose();
        }

        public void Recreate()
        {
            _pool = new AddressableGameObjectPool(_assetReference, _defaultCapacity, _maxSize, _prewarm);
            _spawned = new();
        }
    }
}
#endif 