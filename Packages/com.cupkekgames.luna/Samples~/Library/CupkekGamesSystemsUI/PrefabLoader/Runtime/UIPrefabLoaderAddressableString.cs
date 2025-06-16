#if UNITY_ADDRESSABLES
namespace CupkekGames.Systems.UI
{
  public class UIPrefabLoaderAddressableString : UIPrefabLoaderAddressable<string>
  {
    private static UIPrefabLoaderAddressableString _instance;

    public static UIPrefabLoaderAddressableString Instance
    {
      get
      {
        return _instance;
      }
    }

    protected override void Awake()
    {
      if (_instance == null)
      {
        _instance = this;
        DontDestroyOnLoad(gameObject); // Optional: keep this instance across scenes
      }
      else
      {
        Destroy(gameObject); // Destroy duplicate instances
        return;
      }

      base.Awake();
    }

    private void OnDestroy()
    {
      if (_instance == this)
      {
        _instance = null;
      }
    }
  }
}
#endif