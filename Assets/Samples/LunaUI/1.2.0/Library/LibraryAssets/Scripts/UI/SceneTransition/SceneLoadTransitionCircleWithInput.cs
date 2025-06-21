using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Luna.Library
{
  // Do not use struct, because we need to pass it by reference
  public class SceneLoadTransitionCircleWithInput : SceneTransition
  {
    [SerializeField] SceneLoadTransitionCircleUI _circle;
    [SerializeField] private LoadingViewController _controller;
#if UNITY_INPUT
    [SerializeField] private InputActionAsset _inputActionAsset;
#endif
    [Header("Settings")]
    [SerializeField]
    private List<string> _actionMapsToDisable = new List<string>()
        {
            "Player",
            "UI"
        };
    [SerializeField] private float _fadeInDuration = 2f;
    [SerializeField] private float _fadeOutDuration = 0.5f;

    // State
#if UNITY_INPUT
    private InputAction _continueAction;
#endif
    private Coroutine _coroutine = null;
#if UNITY_INPUT
    private void Start()
    {
      _continueAction = _inputActionAsset[_controller.InputPromptContinue.InputActionName];
    }
#endif
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

    public override void FadeOut()
    {
      _controller.SetInputContinueVisiblity(true, true);

#if UNITY_INPUT
      DisableActionMaps();
      _continueAction.performed += OnContinueInput;
#endif
      _controller.InputPromptContinue.clicked += OnContinue;
    }

    private IEnumerator FadeInLoadingScreen()
    {
      yield return new WaitForSeconds(GetStartDelay());

      DisableActionMaps();
      SceneTransition.LoadingScreenToggleEvent?.Invoke(true, _fadeInDuration);
      _controller.ShowTips(true);

      yield return new WaitForSeconds(_fadeInDuration);

      _circle.Fadeable.SetFadedOut();

      _coroutine = null;
    }
#if UNITY_INPUT
    private void OnContinueInput(InputAction.CallbackContext context)
    {
      OnContinue();
    }
#endif
    private void OnContinue()
    {
      if (_coroutine != null)
      {
        StopCoroutine(_coroutine);
      }

#if UNITY_INPUT
      _continueAction.performed -= OnContinueInput;
      EnableActionMaps();
#endif

      _controller.InputPromptContinue.clicked -= OnContinue;
      SceneTransition.LoadingScreenToggleEvent?.Invoke(false, _fadeOutDuration);

      _coroutine = StartCoroutine(FadeOutLoadingScreen());
    }

    private IEnumerator FadeOutLoadingScreen()
    {
      _circle.Fadeable.SetFadedIn();

      yield return new WaitForSeconds(_fadeOutDuration);

      _circle.Fadeable.FadeOut();

      _coroutine = null;
    }
    public void DisableActionMaps()
    {
#if UNITY_INPUT
      foreach (var actionMapName in _actionMapsToDisable)
      {
        var actionMap = _inputActionAsset.FindActionMap(actionMapName);
        if (actionMap != null)
        {
          actionMap.Disable();
        }
        else
        {
          Debug.LogWarning($"Action map '{actionMapName}' not found.");
        }
      }
#endif
    }

#if UNITY_INPUT
    public void EnableActionMaps()
    {
      foreach (var actionMapName in _actionMapsToDisable)
      {
        var actionMap = _inputActionAsset.FindActionMap(actionMapName);
        if (actionMap != null)
        {
          actionMap.Enable();
        }
        else
        {
          Debug.LogWarning($"Action map '{actionMapName}' not found.");
        }
      }
    }
#endif
  }
}