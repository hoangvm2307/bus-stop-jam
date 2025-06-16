using UnityEngine;


namespace CupkekGames.Systems
{
  public abstract class ServiceLocatorRegisterMono : MonoBehaviour, IServiceLocatorRegister
  {
    protected virtual void Awake()
    {
      RegisterServices();
    }

    protected virtual void OnDestroy()
    {
      UnregisterServices();
    }

    public abstract void RegisterServices();

    public abstract void UnregisterServices();
  }
}