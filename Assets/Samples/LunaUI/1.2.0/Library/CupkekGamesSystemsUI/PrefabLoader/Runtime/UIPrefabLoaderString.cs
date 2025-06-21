namespace CupkekGames.Systems.UI
{
  public class UIPrefabLoaderString : UIPrefabLoader<string>
  {
    private static UIPrefabLoaderString _instance;

    public static UIPrefabLoaderString Instance
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