namespace CupkekGames.Luna
{
  public class SceneLoadTransitionFade : SceneTransition
  {
    public float _fadeInDuration = 0.5f;
    public float _fadeOutDuration = 1f; // Longer because game lags when loading scene

    // Delay before scene loading starts
    public override float GetStartDelay()
    {
      return _fadeInDuration + 0.2f; // add some duration to make sure fade is complete before scene loading starts
    }
    public override void FadeIn()
    {
      SceneTransition.LoadingScreenToggleEvent?.Invoke(true, _fadeInDuration); // fade in, fade duration
    }

    public override void FadeOut()
    {
      SceneTransition.LoadingScreenToggleEvent?.Invoke(false, _fadeOutDuration); // fade out, fade duration
    }
  }
}
