using System.Collections.Generic;
using UnityEngine;

#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Luna.Library
{
    public class SceneLoadTransitionFadeWithInput : SceneTransition
    {
        public float _fadeInDuration = 0.5f;
        public float _fadeOutDuration = 1f; // Longer because game lags when loading scene
        [SerializeField] private LoadingViewController _controller;
#if UNITY_INPUT
        [SerializeField] private InputActionAsset _inputActionAsset;
#endif
        [SerializeField]
        private List<string> _actionMapsToDisable = new List<string>()
        {
            "Player",
            "UI"
        };

        // State
#if UNITY_INPUT
        private InputAction _continueAction;
#endif
        private void Start()
        {
#if UNITY_INPUT
            _continueAction = _inputActionAsset[_controller.InputPromptContinue.InputActionName];
#endif
        }
        public override float GetStartDelay()
        {
            return _fadeInDuration + 0.2f; // add some duration to make sure fade is complete before scene loading starts
        }
        public override void FadeIn()
        {
#if UNITY_INPUT
            DisableActionMaps();
#endif
            SceneTransition.LoadingScreenToggleEvent?.Invoke(true, _fadeInDuration);
            _controller.ShowTips(true);
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
#if UNITY_INPUT
        private void OnContinueInput(InputAction.CallbackContext context)
        {
            OnContinue();
        }
#endif
        private void OnContinue()
        {
            _controller.InputPromptContinue.clicked -= OnContinue;
            SceneTransition.LoadingScreenToggleEvent?.Invoke(false, _fadeOutDuration);

#if UNITY_INPUT
            EnableActionMaps();
            _continueAction.performed -= OnContinueInput;
#endif
        }

#if UNITY_INPUT
        public void DisableActionMaps()
        {
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
        }

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