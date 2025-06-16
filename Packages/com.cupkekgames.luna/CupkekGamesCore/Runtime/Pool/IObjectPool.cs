namespace CupkekGames.Core.Pool
{
  public interface IObjectPool<T>
  {
    T CreatePooledObject();

    void OnTakeFromPool(T Instance);

    void OnReturnToPool(T Instance);

    void OnDestroyObject(T Instance);
  }
}