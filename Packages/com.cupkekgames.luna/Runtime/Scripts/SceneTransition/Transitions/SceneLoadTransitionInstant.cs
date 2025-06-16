namespace CupkekGames.Luna
{
  // Do not use struct, because we need to pass it by reference
  public class SceneLoadTransitionInstant : SceneTransition
  {
    public override float GetStartDelay()
    {
      return 0;
    }
    public override void FadeIn()
    {
      SceneTransition.LoadingScreenToggleEvent?.Invoke(true, 0);
    }

    public override void FadeOut()
    {
      SceneTransition.LoadingScreenToggleEvent?.Invoke(false, 0);
    }
  }
}