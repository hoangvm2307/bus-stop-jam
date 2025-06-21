using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace CupkekGames.Luna.Demo.Components
{
  public class LoadingTransitionMasking : MonoBehaviour
  {
    [SerializeField] private int _size = 1080;
    private UIDocument _uiDocument;
    private VisualElement _loadingMask;
    private Coroutine _loopingFadeRoutine;

    private void Awake()
    {
      _uiDocument = GetComponent<UIDocument>();
      _loadingMask = _uiDocument.rootVisualElement.Q<VisualElement>("LoadingMask");
    }

    private void Start()
    {
      StartFadingLoop(2f);
    }

    public void StartFadingLoop(float delay)
    {
      if (_loopingFadeRoutine != null)
        StopCoroutine(_loopingFadeRoutine);

      _loopingFadeRoutine = StartCoroutine(FadingLoop(delay));
    }

    public void StopFadingLoop()
    {
      if (_loopingFadeRoutine != null)
      {
        StopCoroutine(_loopingFadeRoutine);
        _loopingFadeRoutine = null;
      }
    }

    private IEnumerator FadingLoop(float delay)
    {
      while (true)
      {
        FadeIn();
        yield return new WaitForSeconds(delay);
        FadeOut();
        yield return new WaitForSeconds(delay);
      }
    }

    public void FadeIn()
    {
      Length length = new Length(_size, LengthUnit.Pixel);

      _loadingMask.style.backgroundSize = new BackgroundSize(length, length);
    }

    public void FadeOut()
    {
      Length length = new Length(0, LengthUnit.Pixel);

      _loadingMask.style.backgroundSize = new BackgroundSize(length, length);
    }
  }
}
