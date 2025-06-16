using UnityEngine;

#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Core
{
    [CreateAssetMenu(fileName = "InputIconDatabaseSO", menuName = "CupkekGames/Input/InputIconDatabase")]
    public class InputIconDatabaseSO : ScriptableObject
    {
        [Header("Keyboard & Mouse")]
        [SerializeField] private KeyValueDatabase<string, InputIconResult> _keyboardMouse;
        [Header("Xbox")]
        [SerializeField] private KeyValueDatabase<string, InputIconResult> _xbox;
        [Header("PlayStation 4")]
        [SerializeField] private KeyValueDatabase<string, InputIconResult> _playStation;
        [Header("PlayStation 5")]
        [SerializeField] private KeyValueDatabase<string, InputIconResult> _playStation5;
#if UNITY_INPUT
        public InputIconResultExtra GetInputPromptFromName(string actionName)
        {
            return GetInputPrompt(InputDeviceManager.PlayerInput.actions[actionName]);
        }
        public InputIconResultExtra GetInputPrompt(InputAction action, bool textOnly = false)
        {
            InputIconResultExtra result = new InputIconResultExtra();

            string controlScheme = InputDeviceManager.PlayerInput.currentControlScheme;
            result.BindingIndex = action.GetBindingIndex(group: controlScheme);
            if (result.BindingIndex == -1)
            {
                Debug.LogWarning("bindingIndex == -1: " + action.name + " - " + controlScheme);
                return result;
            }
            var binding = action.bindings[result.BindingIndex];
            if (binding.isPartOfComposite)
            {
                // hard coded logic - assumes that if you found a part of a composite, that it's the first one.
                // And that the one preceeding it, must be the 'Composite head' that contains the parts
                result.BindingIndex--;
            }
            string displayString = action.GetBindingDisplayString(result.BindingIndex, out string deviceLayoutName, out string controlPath, InputBinding.DisplayStringOptions.DontIncludeInteractions);
            // Debug.Log(deviceLayoutName + " - " + displayString + " - " + controlPath);

            if (!textOnly)
            {
                InputIconControlScheme device = InputIconControlSchemeExtensions.FromString(controlScheme);

                KeyValueDatabase<string, InputIconResult> database = GetDatabase(device);
                if (controlPath != null && database.ContainsKey(controlPath))
                {
                    result.IconResult = database.GetValue(controlPath);
                }
                else
                {
                    result.IconResult = database.GetValue("Default");
                    result.Text = displayString;
                }
            }
            else
            {
                result.Text = displayString;
            }

            return result;
        }
#endif

        public KeyValueDatabase<string, InputIconResult> GetDatabase(InputIconControlScheme controlScheme)
        {
            switch (controlScheme)
            {
                case InputIconControlScheme.KeyboardMouse:
                    return _keyboardMouse;
                case InputIconControlScheme.PlayStation4:
                    return _playStation;
                case InputIconControlScheme.PlayStation5:
                    return _playStation5;
                default:
                    return _xbox;
            }
        }
    }
}
