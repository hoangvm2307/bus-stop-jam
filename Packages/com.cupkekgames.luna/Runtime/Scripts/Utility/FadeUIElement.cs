using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System;
using System.Collections;

namespace CupkekGames.Luna
{
  public class FadeUIElement
  {
    private VisualElement _visualElement;
    public event Action OnFadeOutStart;
    public event Action OnFadeOut;
    public event Action OnFadeInStart;
    public event Action OnFadeIn;
    private bool _debug;
    private MonoBehaviour _coroutineRunner;
    private Coroutine _fadeCoroutine;
    public FadeUIElement(MonoBehaviour coroutineRunner, VisualElement visualElement, EasingMode easingMode = EasingMode.EaseOutCirc, bool debug = false)
    {
      _debug = debug;
      _coroutineRunner = coroutineRunner;

      _visualElement = visualElement;

      SetEasing(easingMode);
    }

    private void FadeSetup()
    {
      if (!IsDisplaying())
      {
        SetDisplayStyle(true);
      }

      if (_fadeCoroutine != null)
      {
        _coroutineRunner.StopCoroutine(_fadeCoroutine);
      }
    }

    private bool StartFade(float opacity)
    {
      if (_coroutineRunner == null)
      {
        return false;
      }

      FadeSetup();

      if (_debug)
      {
        Debug.Log("Duration: " + GetDuration());

        Debug.Log("Opacity: " + GetOpacity() + " => " + opacity);
      }

      _visualElement.style.opacity = opacity;

      float duration = GetDuration();
      if (duration == 0)
      {
        OnTransitionComplete();
      }
      else
      {
        _fadeCoroutine = _coroutineRunner.StartCoroutine(FadeCoroutine(duration));
      }

      return true;
    }
    private IEnumerator FadeCoroutine(float duration)
    {
      yield return new WaitForSeconds(duration);

      OnTransitionComplete();
    }
    private float GetOpacity()
    {
      return _visualElement.style.opacity.value;
    }
    private void SetDisplayStyle(bool show)
    {
      if (show)
      {
        _visualElement.style.display = DisplayStyle.Flex;
      }
      else
      {
        _visualElement.style.display = DisplayStyle.None;
      }
    }
    private bool IsDisplaying()
    {
      return _visualElement.style.display == DisplayStyle.Flex;
    }

    public void FadeIn()
    {
      if (StartFade(1))
      {
        if (_debug)
        {
          Debug.Log("FadeFade IN " + _visualElement.name);
        }

        OnFadeInStart?.Invoke();
      }
    }

    public void FadeOut()
    {
      if (StartFade(0))
      {
        if (_debug)
        {
          Debug.Log("FadeFade OUT " + _visualElement.name);
        }

        OnFadeOutStart?.Invoke();
      }
    }

    public void SetDuration(float seconds)
    {
      _visualElement.style.transitionDuration = new List<TimeValue>()
        {
            new TimeValue(seconds, TimeUnit.Second)
        };
    }
    public void SetTransitionProperty()
    {
      _visualElement.style.transitionProperty = new List<StylePropertyName>()
        {
            new StylePropertyName("opacity")
        };
    }

    public float GetDuration()
    {
      if (_visualElement.style.transitionDuration.value == null)
      {
        if (_debug)
        {
          Debug.Log("GetDuration null" + _visualElement.name);
        }
        return 0;
      }

      return _visualElement.style.transitionDuration.value[0].value;
    }

    public void SetEasing(EasingMode easingMode)
    {
      _visualElement.style.transitionTimingFunction = new List<EasingFunction>()
        {
            new EasingFunction(easingMode)
        };
    }

    private void OnTransitionComplete()
    {
      if (GetOpacity() == 0)
      {
        if (_debug)
        {
          Debug.Log("OnTransitionComplete 1 " + _visualElement.name);
        }
        SetDisplayStyle(false);

        OnFadeOut?.Invoke();
      }
      else
      {
        if (_debug)
        {
          Debug.Log("OnTransitionComplete 2 " + _visualElement.name);
        }
        OnFadeIn?.Invoke();
      }
    }

    public void Stop()
    {
      if (_fadeCoroutine != null)
      {
        _coroutineRunner.StopCoroutine(_fadeCoroutine);
      }
    }
  }
}