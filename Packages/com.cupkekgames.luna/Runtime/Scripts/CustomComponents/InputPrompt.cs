using System;
using System.Collections.Generic;
using CupkekGames.Core;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_INPUT
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
#endif

namespace CupkekGames.Luna
{
    [UxmlElement]
    public partial class InputPrompt : Button
    {
        private string _inputActionName = "UI/Submit";
        [UxmlAttribute]
        public string InputActionName
        {
            get
            {
                return _inputActionName;
            }
            set
            {
                _inputActionName = value;
            }
        }
        private Sprite _overrideIconSprite = null;
        [UxmlAttribute]
        public Sprite OverrideIconSprite
        {
            get
            {
                return _overrideIconSprite;
            }
            set
            {
                _overrideIconSprite = value;
                if (_overrideIconSprite != null)
                {
                    _iconSprite.style.backgroundImage = new StyleBackground(_overrideIconSprite);
                }
                else
                {
                    _iconSprite.style.backgroundImage = new StyleBackground(StyleKeyword.Auto);
                }
            }
        }
        private string _overrideIconText = null;
        [UxmlAttribute]
        public string OverrideIconText
        {
            get
            {
                return _overrideIconText;
            }
            set
            {
                _overrideIconText = value;
                _iconLabel.text = _overrideIconText;
            }
        }
        private int _minSize = 48;
        [UxmlAttribute]
        public int MinSize
        {
            get
            {
                return _minSize;
            }
            set
            {
                _minSize = value;
                StyleLength length = new StyleLength(new Length(_minSize, LengthUnit.Pixel));

                style.minHeight = length;
                style.maxHeight = length;
                style.minWidth = length;
                _iconContainer.style.minHeight = length;
                _iconContainer.style.minWidth = length;

                if (_hold != null)
                {
                    StyleLength holdLength = new StyleLength(new Length(_minSize * 1.12f, LengthUnit.Pixel));
                    StyleLength holdLeft = new StyleLength(new Length(-(holdLength.value.value - _minSize) / 2f, LengthUnit.Pixel));
                    _hold.style.width = holdLength;
                    _hold.style.left = holdLeft;
                }
            }
        }
        private bool _slice = false;
        [UxmlAttribute]
        public bool Slice
        {
            get
            {
                return _slice;
            }
            set
            {
                _slice = value;

                if (_slice)
                {
                    _iconSprite.AddToClassList(_ussIconSlice);
                }
                else
                {
                    _iconSprite.RemoveFromClassList(_ussIconSlice);
                }
            }
        }
        private string _labelText = null;
        [UxmlAttribute, CreateProperty]
        public string LabelText
        {
            get
            {
                return _labelText;
            }
            set
            {
                _labelText = value;
                _label.text = _labelText;
            }
        }
        private List<InputIconControlScheme> _buttonIf = new List<InputIconControlScheme>()
        {
           InputIconControlScheme.KeyboardMouse
        };
        [UxmlAttribute]
        public List<InputIconControlScheme> ButtonIf
        {
            get
            {
                return _buttonIf;
            }
            set
            {
                _buttonIf = value;
            }
        }
        private List<InputIconControlScheme> _hideIf = new();
        [UxmlAttribute]
        public List<InputIconControlScheme> HideIf
        {
            get
            {
                return _hideIf;
            }
            set
            {
                _hideIf = value;
            }
        }
        private bool _hide = false;
        [UxmlAttribute]
        public bool Hide
        {
            get
            {
                return _hide;
            }
            set
            {
                _hide = value;
            }
        }

        private const string _ussClassName = "InputPrompt";
        private const string _ussContainer = _ussClassName + "__icon_container";
        private const string _ussIconSprite = _ussClassName + "__icon_sprite";
        private const string _ussIconSlice = _ussClassName + "__icon_slice";
        private const string _ussIconLabel = _ussClassName + "__icon_label";
        private const string _ussLabel = _ussClassName + "__label";
        private const string _ussHold = _ussClassName + "__hold";
        private VisualElement _iconContainer;
        private VisualElement _iconSprite;
        private Label _iconLabel;
        private Label _label;
        private RadialProgressBar _hold;
        // State
#if UNITY_INPUT
        private InputAction _action;
        public InputAction Action => _action;
#endif
        private int _bindingIndex;
        public int BindingIndex => _bindingIndex;
        private IVisualElementScheduledItem _holdSchedule;
        // Events
        public event Action OnUpdate;

        public InputPrompt()
        {
            AddToClassList("btn");
            AddToClassList("primary");
            AddToClassList("ghost");
            AddToClassList(_ussClassName);

            _iconContainer = new VisualElement();
            _iconContainer.AddToClassList(_ussContainer);
            _iconContainer.pickingMode = PickingMode.Ignore;
            Add(_iconContainer);

            _iconSprite = new VisualElement();
            _iconSprite.AddToClassList(_ussIconSprite);
            _iconSprite.pickingMode = PickingMode.Ignore;
            _iconContainer.Add(_iconSprite);

            _iconLabel = new Label();
            _iconLabel.AddToClassList(_ussIconLabel);
            _iconLabel.pickingMode = PickingMode.Ignore;
            _iconContainer.Add(_iconLabel);

            _label = new Label();
            _label.AddToClassList(_ussLabel);
            _label.pickingMode = PickingMode.Ignore;
            Add(_label);

            MinSize = _minSize; // Apply minsize

            this.RegisterCallback<AttachToPanelEvent>(e => OnAttach());
            this.RegisterCallback<DetachFromPanelEvent>(e => OnDetach());
        }

        public void OnAttach()
        {
            HoldOnDisable();

            UpdateBindingDisplay(InputDeviceManager.CurrentScheme);

            InputDeviceManager.OnControlSchemeChange += UpdateBindingDisplay;
        }

        public void OnDetach()
        {
            InputDeviceManager.OnControlSchemeChange -= UpdateBindingDisplay;

            HoldOnDisable();
        }

        private void UpdateBindingDisplay(InputIconControlScheme scheme)
        {
            if (_hide)
            {
                style.visibility = Visibility.Hidden;
                return;
            }

            if (_hideIf.Contains(scheme))
            {
                style.visibility = Visibility.Hidden;
                SetEnabled(false);
                return;
            }

            style.visibility = Visibility.Visible;

            if (_buttonIf.Contains(scheme))
            {
                SetEnabled(true);
            }
            else
            {
                SetEnabled(false);
            }

            if (scheme == InputIconControlScheme.KeyboardMouse)
            {
                _iconSprite.RemoveFromClassList("circle");
            }
            else
            {
                _iconSprite.AddToClassList("circle");
            }

            if (_overrideIconSprite != null)
            {
                _iconSprite.style.backgroundImage = new StyleBackground(_overrideIconSprite);
            }
            else
            {
                _iconSprite.style.backgroundImage = new StyleBackground(StyleKeyword.Auto);
            }

            if (_overrideIconText != null)
            {
                _iconLabel.text = _overrideIconText;
            }
            else
            {
                _iconLabel.text = "A";
            }

            if (!Application.isPlaying)
            {
                return;
            }

#if UNITY_INPUT
            LunaUIManager lunaUIManager = LunaUIManager.Instance;

            if (lunaUIManager != null && lunaUIManager.IconDatabase != null && InputDeviceManager.PlayerInput != null)
            {
                InputIconDatabaseSO inputIconDatabase = lunaUIManager.IconDatabase;
                InputIconResultExtra result = inputIconDatabase.GetInputPromptFromName(_inputActionName);

                _iconLabel.text = result.Text;
                if (string.IsNullOrEmpty(result.Text) || result.Text.Length < 2)
                {
                    _iconLabel.RemoveFromClassList("long");
                }
                else
                {
                    _iconLabel.AddToClassList("long");
                }

                if (result.IconResult != null)
                {
                    _iconSprite.style.backgroundImage = new StyleBackground(result.IconResult.Icon);

                    this.Slice = result.IconResult.Slice;
                    if (scheme == InputIconControlScheme.KeyboardMouse || result.IconResult.Square)
                    {
                        _iconSprite.RemoveFromClassList("circle");
                    }
                    else
                    {
                        _iconSprite.AddToClassList("circle");
                    }

                    _iconSprite.style.display = DisplayStyle.Flex;
                }
                else
                {
                    // _iconSprite.style.backgroundImage = new StyleBackground(StyleKeyword.Auto);
                    _iconSprite.style.display = DisplayStyle.None;
                }

                _action = InputDeviceManager.PlayerInput.actions[_inputActionName];
                _bindingIndex = result.BindingIndex;

                if (_bindingIndex != -1 && 
                        (
                            HasHoldInteraction(_action.interactions) || 
                            HasHoldInteraction(_action.bindings[_bindingIndex].interactions)
                        )
                    )
                {
                    AddHold();
                    HoldOnEnable();
                }
            }
#endif

            MinSize = _minSize; // Apply minsize
            OnUpdate?.Invoke();
        }

        // Hold
        private bool HasHoldInteraction(string interactions)
        {
            foreach (var interaction in interactions.Split(';'))
            {
                if (interaction == "Hold")
                {
                    return true;
                }
            }

            return false;
        }

        public void AddHold()
        {
            if (_hold != null)
            {
                return;
            }

            _hold = new RadialProgressBar();
            _hold.name = "HoldProgress";
            _hold.AddToClassList(_ussHold);
            _hold.pickingMode = PickingMode.Ignore;
            _hold.Thickness = 4;
            _hold.SecondDelay = 0;
            _hold.Instant = true;
            _hold.TargetValue = 0;
            _hold.TargetIndicator = 0;
            _iconContainer.Add(_hold);
        }

        public void HoldOnEnable()
        {
            HoldOnDisable();
            AddToClassList("hold");

#if UNITY_INPUT
            if (_action == null)
            {
                return;
            }

            _action.started += HoldStart;
            _action.performed += HoldPerformed;
            _action.canceled += HoldCanceled;
#endif
        }

        public void HoldOnDisable()
        {
            RemoveFromClassList("hold");

#if UNITY_INPUT
            if (_action == null)
            {
                return;
            }

            _action.started -= HoldStart;
            _action.performed -= HoldPerformed;
            _action.canceled -= HoldCanceled;
#endif
        }

#if UNITY_INPUT
        private void HoldStart(InputAction.CallbackContext context)
        {
            if (context.interaction is HoldInteraction holdInteraction)
            {
                StartHoldAnimation(holdInteraction.duration);
            }
        }

        private void HoldPerformed(InputAction.CallbackContext context)
        {
            if (context.interaction is HoldInteraction)
            {
                _hold.TargetValue = 0;
                StoptHoldAnimation();
            }
        }

        private void HoldCanceled(InputAction.CallbackContext context)
        {
            if (context.interaction is HoldInteraction)
            {
                _hold.TargetValue = 0;
                StoptHoldAnimation();
            }
        }
#endif

        public void StartHoldAnimation(float duration)
        {
            StoptHoldAnimation();

#if UNITY_INPUT
            if (duration <= 0)
            {
                duration = InputSystem.settings.defaultHoldTime;
            }
#endif

            float elapsedTime = 0f;

            // Start scheduling an update every frame
            _holdSchedule = _hold.schedule.Execute(() =>
            {
                // Update elapsed time
                elapsedTime += Time.deltaTime;

                _hold.TargetValue = elapsedTime / duration;
            })
            .Every(1) // must be 1 for deltaTime to work correctly
            .Until(() => _hold.TargetValue >= 1f);
        }
        public void StoptHoldAnimation()
        {
            _holdSchedule?.Pause();
        }
    }
}