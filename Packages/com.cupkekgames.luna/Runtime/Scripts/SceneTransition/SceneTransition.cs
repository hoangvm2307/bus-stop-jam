using System;
using UnityEngine;

namespace CupkekGames.Luna
{
  // Do not use struct, because we need to pass it by reference
  public abstract class SceneTransition : MonoBehaviour
  {
    public static Action<bool, float> LoadingScreenToggleEvent; // fadeIn, duration
    /// <summary>
    /// Delay before scene unloading/loading starts to make sure transition is done
    /// </summary>
    /// <returns>Delay</returns>
    public abstract float GetStartDelay();
    public abstract void FadeIn();
    public abstract void FadeOut();
  }
}