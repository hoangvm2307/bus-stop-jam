using UnityEngine;
using UnityEngine.Pool;

namespace CupkekGames.Core.Pool
{
  public abstract class ObjectPoolBase<T> : IObjectPool<T> where T : class
  {
    private bool _collectionCheck = true;
    private int _defaultCapacity;
    private int _maxSize;
    private ObjectPool<T> pool;

    public ObjectPool<T> Pool => pool;

    public ObjectPoolBase(int defaultCapacity, int maxSize, bool collectionCheck = true)
    {
      _collectionCheck = collectionCheck;
      _defaultCapacity = defaultCapacity;
      _maxSize = maxSize;

      pool = new ObjectPool<T>(CreatePooledObject, OnTakeFromPool, OnReturnToPool, OnDestroyObject, _collectionCheck, _defaultCapacity, _maxSize);
    }

    public void Prewarm()
    {
      T[] instances = new T[_defaultCapacity];
      for (int i = 0; i < _defaultCapacity; i++)
      {
        instances[i] = Pool.Get();
      }
      for (int i = 0; i < _defaultCapacity; i++)
      {
        Pool.Release(instances[i]);
      }
    }

    public abstract T CreatePooledObject();

    public abstract void OnTakeFromPool(T Instance);

    public abstract void OnReturnToPool(T Instance);

    public abstract void OnDestroyObject(T Instance);
  }
}