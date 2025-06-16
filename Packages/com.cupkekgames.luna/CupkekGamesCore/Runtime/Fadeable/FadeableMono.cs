using UnityEngine;


namespace CupkekGames.Core
{
  public abstract class FadeableMono : MonoBehaviour
  {
    [SerializeField] public Fadeable Fadeable;
    protected virtual void Awake()
    {
      if (Fadeable == null)
      {
        Fadeable = new Fadeable(this);
      }
      else
      {
        Fadeable.Initialize(this);
      }
    }
    protected virtual void OnDestroy()
    {
      Fadeable.Kill();
    }
  }
}