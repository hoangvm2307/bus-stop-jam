using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_INPUT
using CupkekGames.Core;
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Systems.UI
{
    public class SettingsMenuViewRebind : SettingsMenuViewSection
    {
        [SerializeField] List<string> _inputList;
        [SerializeField] string _cancelControlPath = "<Keyboard>/escape";
        // UI
        private VisualElement _rebindOverlay;
        private Label _rebindOverlayLabel;
        private VisualElement _rebindParent;
        private List<SettingsMenuViewRebindElement> _rebindElements = new();
        // References
#if UNITY_INPUT
        private PlayerInput _playerInput;
#endif

        protected override void Initialize()
        {
            _rebindParent = UIDocument.rootVisualElement.Q<VisualElement>("RebindContainer");
            _rebindOverlay = UIDocument.rootVisualElement.Q<VisualElement>("RebindOverlay");
            _rebindOverlay.style.display = DisplayStyle.None;
            _rebindOverlayLabel = UIDocument.rootVisualElement.Q<Label>("RebindOverlayLabel");

#if UNITY_INPUT
            _playerInput = InputDeviceManager.PlayerInput;

            if (_playerInput == null)
            {
                Debug.LogWarning("LunaUIManager: PlayerInput is null. Settings input rebinding wont work.");
                return;
            }

            foreach (string inputName in _inputList)
            {
                InputAction inputAction = _playerInput.actions[inputName];
                if (inputAction == null)
                {
                    Debug.LogWarning("InputAction is invalid: " + inputName);
                    continue;
                }
                _rebindElements.Add(new SettingsMenuViewRebindElement(this, _rebindParent, inputName));
            }
#endif
        }

#if UNITY_INPUT
        public void OnDisable()
        {
            m_RebindOperation?.Dispose();
            m_RebindOperation = null;
        }
#endif

        public override void OnTabSelected()
        {
            if (_rebindElements.Count > 0)
            {
                _rebindElements[0].InputPrompt.Focus();
            }
        }

        // Apply Settings to UI
        public override void ApplySettingsToUI()
        {
            // Rebind and display UI is not different unlike other setting sections, so we just apply settings
            SettingsDataSectionRebind rebind = (SettingsDataSectionRebind)_changedSettings.GetValue("rebind");

            rebind.ApplySettings(rebind);
        }

#if UNITY_INPUT
        private InputActionRebindingExtensions.RebindingOperation m_RebindOperation;

        /// <summary>
        /// Initiate an interactive rebind that lets the player actuate a control to choose a new binding
        /// for the action.
        /// </summary>
        public void StartInteractiveRebind(SettingsMenuViewRebindElement element)
        {
            InputAction action = element.InputPrompt.Action;
            int bindingIndex = element.InputPrompt.BindingIndex;

            // If the binding is a composite, we need to rebind each part in turn.
            if (action.bindings[bindingIndex].isComposite)
            {
                Debug.Log("Composite");
                var firstPartIndex = bindingIndex + 1;
                if (firstPartIndex < action.bindings.Count && action.bindings[firstPartIndex].isPartOfComposite)
                    PerformInteractiveRebind(element, action, firstPartIndex, allCompositeParts: true);
            }
            else
            {
                PerformInteractiveRebind(element, action, bindingIndex);
            }
        }
        private void PerformInteractiveRebind(SettingsMenuViewRebindElement element, InputAction action, int bindingIndex, bool allCompositeParts = false)
        {
            m_RebindOperation?.Cancel(); // Will null out m_RebindOperation.

            void CleanUp()
            {
                m_RebindOperation?.Dispose();
                m_RebindOperation = null;
                action.Enable();
                _rebindOverlay.style.display = DisplayStyle.None;
            }

            //Fixes the "InvalidOperationException: Cannot rebind action x while it is enabled" error
            action.Disable();

            // Configure the rebind.
            m_RebindOperation = action.PerformInteractiveRebinding(bindingIndex)
                .OnCancel(
                    operation =>
                    {
                        CleanUp();
                    })
                .OnComplete(
                    operation =>
                    {
                        CleanUp();
                        OnRebindChanged();

                        // If there's more composite parts we should bind, initiate a rebind
                        // for the next part.
                        if (allCompositeParts)
                        {
                            var nextBindingIndex = bindingIndex + 1;
                            if (nextBindingIndex < action.bindings.Count && action.bindings[nextBindingIndex].isPartOfComposite)
                                PerformInteractiveRebind(element, action, nextBindingIndex, true);
                        }
                    })
                .WithCancelingThrough(_cancelControlPath);

            // If it's a part binding, show the name of the part in the UI.
            var partName = default(string);
            if (action.bindings[bindingIndex].isPartOfComposite)
                partName = $"Binding '{action.bindings[bindingIndex].name}'. ";

            // Bring up rebind overlay, if we have one.
            _rebindOverlay.style.display = DisplayStyle.Flex;

            var text = !string.IsNullOrEmpty(m_RebindOperation.expectedControlType)
                ? $"{partName}Waiting for {m_RebindOperation.expectedControlType} input..."
                : $"{partName}Waiting for input...";
            _rebindOverlayLabel.text = text;

            m_RebindOperation.Start();
        }

        /// <summary>
        /// Remove currently applied binding overrides.
        /// </summary>
        public void ResetToDefault(SettingsMenuViewRebindElement element)
        {
            InputAction action = element.InputPrompt.Action;
            int bindingIndex = element.InputPrompt.BindingIndex;

            if (action.bindings[bindingIndex].isComposite)
            {
                // It's a composite. Remove overrides from part bindings.
                for (var i = bindingIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; ++i)
                    action.RemoveBindingOverride(i);
            }
            else
            {
                action.RemoveBindingOverride(bindingIndex);
            }
        }

        private void OnRebindChanged()
        {
            SettingsDataSectionRebind rebind = (SettingsDataSectionRebind)_changedSettings.GetValue("rebind");

            rebind.BindingOverridesJson = _playerInput.actions.SaveBindingOverridesAsJson();
        }

        private void CancelRebind()
        {
            m_RebindOperation?.Cancel();
        }
#endif
    }
}