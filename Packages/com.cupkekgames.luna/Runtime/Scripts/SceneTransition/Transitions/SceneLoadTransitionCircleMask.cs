using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  // Do not use struct, because we need to pass it by reference
  public class SceneLoadTransitionCircleMask : SceneTransition
  {
    [SerializeField] UIDocument _uiDocument;
    [SerializeField] string _maskElementName = "LoadingMask";
    [SerializeField] int _fadeOutSize = 3000;
    [SerializeField] float _duration = 2;
    private VisualElement _loadingMask;
    private void Awake()
    {
      _loadingMask = _uiDocument.rootVisualElement.Q<VisualElement>(_maskElementName);
    }
    public override float GetStartDelay()
    {
      return _duration; // Wait for transition to complete before starting scene unload/load
    }
    public override void FadeIn()
    {
      Debug.Log("FadeIn");
      SceneTransition.LoadingScreenToggleEvent?.Invoke(true, 0);

      _loadingMask.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0, TimeUnit.Second) });
      Length length = new Length(0, LengthUnit.Pixel);
      _loadingMask.style.backgroundSize = new BackgroundSize(length, length);

      _loadingMask.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(_duration, TimeUnit.Second) });
      length = new Length(_fadeOutSize, LengthUnit.Pixel);
      _loadingMask.style.backgroundSize = new BackgroundSize(length, length);
    }

    public override void FadeOut()
    {
      Debug.Log("FadeOut");
      SceneTransition.LoadingScreenToggleEvent?.Invoke(true, 0);

      _loadingMask.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(0, TimeUnit.Second) });
      Length length = new Length(_fadeOutSize, LengthUnit.Pixel);
      _loadingMask.style.backgroundSize = new BackgroundSize(length, length);

      _loadingMask.style.transitionDuration = new StyleList<TimeValue>(new List<TimeValue> { new TimeValue(_duration, TimeUnit.Second) });
      length = new Length(0, LengthUnit.Pixel);
      _loadingMask.style.backgroundSize = new BackgroundSize(length, length);

      StartCoroutine(FadeInLoadingScreen());
    }
    private IEnumerator FadeInLoadingScreen()
    {
      yield return new WaitForSeconds(_duration);

      SceneTransition.LoadingScreenToggleEvent?.Invoke(false, 0f);
    }
  }
}
