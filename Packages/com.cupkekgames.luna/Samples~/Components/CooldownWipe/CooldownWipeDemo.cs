using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using CupkekGames.Core;
using System;

namespace CupkekGames.Luna.Demo.Components
{
  public class CooldownWipeDemo : MonoBehaviour
  {
    private UIDocument _uiDocument;
    private CooldownWipe _cooldownWipe;
    [SerializeField] Fadeable _fadeable;
    private Coroutine _loopingFadeRoutine;

    private void Awake()
    {
      _uiDocument = GetComponent<UIDocument>();
      _cooldownWipe = _uiDocument.rootVisualElement.Q<CooldownWipe>();

      _fadeable.Initialize(this);
    }

    private void OnEnable()
    {
      _fadeable.OnApply += Apply;
    }
    private void OnDisable()
    {
      _fadeable.OnApply -= Apply;
    }

    private void Apply()
    {
      _cooldownWipe.Progress = _fadeable.Value;
    }

    private void Start()
    {
      StartFadingLoop();
    }

    public void StartFadingLoop()
    {
      if (_loopingFadeRoutine != null)
        StopCoroutine(_loopingFadeRoutine);

      _loopingFadeRoutine = StartCoroutine(FadingLoop());
    }

    public void StopFadingLoop()
    {
      if (_loopingFadeRoutine != null)
      {
        StopCoroutine(_loopingFadeRoutine);
        _loopingFadeRoutine = null;
      }
    }

    private IEnumerator FadingLoop()
    {
      while (true)
      {
        _fadeable.FadeIn();
        yield return new WaitForSeconds(_fadeable._fadeInDuration + 0.5f);
        _fadeable.FadeOut();
        yield return new WaitForSeconds(_fadeable._fadeOutDuration + 0.5f);
      }
    }
  }
}
