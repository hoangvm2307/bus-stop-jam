using System.Collections;
using UnityEngine;

namespace CupkekGames.Luna
{
  // Do not use struct, because we need to pass it by reference
  public class SceneLoadTransitionCircle : SceneTransition
  {
    [SerializeField] SceneLoadTransitionCircleUI _circle;
    [Header("Settings")]
    [SerializeField] bool _showLoadingScreen = false;
    [SerializeField] private float _fadeInDuration = 2f;
    [SerializeField] private float _fadeOutDuration = 0.5f;
    private Coroutine _coroutine = null;
    public override float GetStartDelay()
    {
      return 1f; // Wait for transition to complete before starting scene unload/load
    }
    public override void FadeIn()
    {
      if (_coroutine != null)
      {
        StopCoroutine(_coroutine);
      }

      _circle.Fadeable.FadeIn();

      _coroutine = StartCoroutine(FadeInLoadingScreen());
    }

    private IEnumerator FadeInLoadingScreen()
    {
      yield return new WaitForSeconds(GetStartDelay());

      if (_showLoadingScreen)
      {
        SceneTransition.LoadingScreenToggleEvent?.Invoke(true, _fadeInDuration);

        yield return new WaitForSeconds(_fadeInDuration);

        _circle.Fadeable.SetFadedOut();
      }

      _coroutine = null;
    }

    public override void FadeOut()
    {
      if (_coroutine != null)
      {
        StopCoroutine(_coroutine);
      }

      if (_showLoadingScreen)
      {
        SceneTransition.LoadingScreenToggleEvent?.Invoke(false, _fadeOutDuration);
      }

      _coroutine = StartCoroutine(FadeOutLoadingScreen());
    }
    private IEnumerator FadeOutLoadingScreen()
    {
      _circle.Fadeable.SetFadedIn();

      yield return new WaitForSeconds(_fadeOutDuration);

      _circle.Fadeable.FadeOut();

      _coroutine = null;
    }
  }
}
