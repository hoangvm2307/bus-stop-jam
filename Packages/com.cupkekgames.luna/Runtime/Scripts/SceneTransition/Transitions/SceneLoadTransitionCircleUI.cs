using UnityEngine;
using CupkekGames.Core;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  [RequireComponent(typeof(UIDocument))]
  public class SceneLoadTransitionCircleUI : FadeableMono
  {
    private UIDocument _uiDocument;
    private CircleHoleElement _circleHoleElement;

    protected override void Awake()
    {
      base.Awake();

      _uiDocument = GetComponent<UIDocument>();

      _circleHoleElement = _uiDocument.rootVisualElement.Q<CircleHoleElement>();

      Fadeable.OnApply += OnApply;
      // Fadeable.OnFadeOutStart += OnFadeOutStart;
      // Fadeable.OnFadeOutComplete += OnFadeOutComplete;
      // Fadeable.OnFadeInStart += OnFadeInStart;

      Fadeable.SetFadedOut();
    }

    private void OnApply()
    {
      _circleHoleElement.Radius = Fadeable.Value;
    }
    // private void OnFadeOutStart()
    // {
    //   _uiDocument.gameObject.SetActive(true);
    // }
    // private void OnFadeOutComplete()
    // {
    //   _uiDocument.gameObject.SetActive(false);
    // }
    // private void OnFadeInStart()
    // {
    //   _uiDocument.gameObject.SetActive(true);
    // }
  }
}
