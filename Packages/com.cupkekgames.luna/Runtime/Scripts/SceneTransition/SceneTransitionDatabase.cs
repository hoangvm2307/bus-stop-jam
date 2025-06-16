using CupkekGames.Core;

namespace CupkekGames.Luna
{
  // Do not use struct, because we need to pass it by reference
  public class SceneTransitionDatabase : Singleton<SceneTransitionDatabase>
  {
    public KeyValueDatabase<string, SceneTransition> Transitions = new();
  }
}