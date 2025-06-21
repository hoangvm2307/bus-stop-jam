#if UNITY_ADDRESSABLES
using System.Collections.Generic;
using CupkekGames.Luna;

namespace CupkekGames.Systems
{
  // Do not use struct, because we need to pass it by reference
  public class SceneLoadRequest
  {
    //Parameters coming from scene loading requests
    public List<SceneSO> ScenesToLoad;
    public int ScenesLeftToLoad;
    public List<SceneSO> ScenesToUnload;
    public int ScenesLeftToUnload;
    public SceneTransition SceneLoadTransition;

    public SceneLoadRequest(List<SceneSO> scenesToLoad, List<SceneSO> scenesToUnload, SceneTransition sceneLoadTransition)
    {
      ScenesToLoad = scenesToLoad != null ? scenesToLoad : new List<SceneSO>();
      ScenesLeftToLoad = scenesToLoad != null ? scenesToLoad.Count : 0;
      ScenesToUnload = scenesToUnload != null ? scenesToUnload : new List<SceneSO>();
      ScenesLeftToUnload = scenesToUnload != null ? scenesToUnload.Count : 0;
      SceneLoadTransition = sceneLoadTransition;
    }

    public int GetNextSceneToLoadIndex()
    {
      // Load scenes in order
      return ScenesToLoad.Count - ScenesLeftToLoad;
    }
  }
}
#endif