using System;

#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Core
{
    public static class InputDeviceManager
    {
#if UNITY_INPUT
        private static PlayerInput _playerInput;
        public static PlayerInput PlayerInput => _playerInput;
#endif
        private static InputIconControlScheme _currentScheme = InputIconControlScheme.Xbox;
        public static InputIconControlScheme CurrentScheme => _currentScheme;
        public static event Action<InputIconControlScheme> OnControlSchemeChange;

#if UNITY_INPUT
        public static bool OnEnable(PlayerInput playerInput)
        {
            _playerInput = playerInput;

            if (_playerInput == null)
            {
                return false;
            }

            _currentScheme = InputIconControlSchemeExtensions.FromString(_playerInput.currentControlScheme);

            InputSystem.onActionChange += OnActionChange;

            return true;
        }

        public static void OnDisable()
        {
            if (_playerInput == null)
            {
                return;
            }

            InputSystem.onActionChange -= OnActionChange;
        }

        // When the action system re-resolves bindings, we want to update our UI in response. While this will
        // also trigger from changes we made ourselves, it ensures that we react to changes made elsewhere. If
        // the user changes keyboard layout, for example, we will get a BoundControlsChanged notification and
        // will update our UI to reflect the current keyboard layout.
        private static void OnActionChange(object obj, InputActionChange change)
        {
            if (change == InputActionChange.ActionPerformed)
            {
                UpdateControlScheme();
            }
            else if (change == InputActionChange.BoundControlsChanged || change == InputActionChange.ActionPerformed)
            {
                UpdateControlScheme(true);
            }
        }

        public static void UpdateControlScheme(bool forceInvoke = false)
        {
            InputIconControlScheme newScheme = InputIconControlSchemeExtensions.FromString(_playerInput.currentControlScheme);

            UpdateControlScheme(newScheme, forceInvoke);
        }
#endif

        public static void UpdateControlScheme(InputIconControlScheme newScheme, bool forceInvoke = false)
        {
            if (_currentScheme != newScheme || forceInvoke)
            {
                OnControlSchemeChange?.Invoke(newScheme);
            }

            _currentScheme = newScheme;
        }
    }
}
