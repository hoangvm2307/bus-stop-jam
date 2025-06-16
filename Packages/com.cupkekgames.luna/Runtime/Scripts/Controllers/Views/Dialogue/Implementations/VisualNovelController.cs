using System;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Luna
{
    public class VisualNovelController : DialogueController
    {
        protected Button _buttonContinue;
        protected InputPrompt _buttonRestart;
        protected InputPrompt _buttonSkip;
        protected InputPrompt _nextIcon;
        protected Label _charName;
        protected VisualElement _avatarLeftContainer;
        protected VisualElement _avatarLeft;
        protected VisualElement _avatarRightContainer;
        protected VisualElement _avatarRight;

        // Events
        public event Action OnContinue;
        public event Action OnRestart;
        public event Action OnSkip;
        // State
        private TransitionToggleRepeat _avatarLeftSchedule;
        private TransitionToggleRepeat _avatarRightSchedule;
        private TransitionToggleRepeat _nextIconSchedule;
#if UNITY_INPUT
        // Input
        private InputAction _inputRestart;
        private InputAction _inputSkip;
        private InputAction _inputNext;
#endif
        protected override void Awake()
        {
            base.Awake();

            _buttonContinue = ParentElement.Q<Button>("ContinueButton");
            _buttonRestart = ParentElement.Q<InputPrompt>("RestartButton");
            _buttonSkip = ParentElement.Q<InputPrompt>("SkipButton");

            _charName = ParentElement.Q<Label>("CharacterName");
            SetCharacterName("");

            _nextIcon = ParentElement.Q<InputPrompt>("NextIcon");
            _avatarLeftContainer = ParentElement.Q<VisualElement>("AvatarLeftContainer");
            _avatarRightContainer = ParentElement.Q<VisualElement>("AvatarRightContainer");
            _avatarLeft = ParentElement.Q<VisualElement>("AvatarLeft");
            _avatarRight = ParentElement.Q<VisualElement>("AvatarRight");

            _nextIconSchedule = new TransitionToggleRepeat(_nextIcon, "vn_next_icon_anim", 100);
            _avatarLeftSchedule = new TransitionToggleRepeat(_avatarLeft, "vn_avatar_anim", 200);
            _avatarRightSchedule = new TransitionToggleRepeat(_avatarRight, "vn_avatar_anim", 200);

            HideAvatarLeft();
            HideAvatarRight();

#if UNITY_INPUT
            _inputRestart = LunaUIManager.Instance.PlayerInput.actions[_buttonRestart.InputActionName];
            _inputSkip = LunaUIManager.Instance.PlayerInput.actions[_buttonSkip.InputActionName];
            _inputNext = LunaUIManager.Instance.PlayerInput.actions[_nextIcon.InputActionName];
#endif
        }

        private void OnEnable()
        {
            if (_buttonContinue != null)
            {
                _buttonContinue.clicked += ButtonContinue;
            }

            if (_buttonRestart != null)
            {
                _buttonRestart.clicked += ButtonRestart;
            }

            if (_buttonSkip != null)
            {
                _buttonSkip.clicked += ButtonSkip;
            }

#if UNITY_INPUT
            if (_inputRestart != null)
            {
                _inputRestart.performed += InputRestart;
            }

            if (_inputSkip != null)
            {
                _inputSkip.performed += InputSkip;
            }

            if (_inputNext != null)
            {
                _inputNext.performed += InputContinue;
            }
#endif
        }


        private void OnDisable()
        {
            if (_buttonContinue != null)
            {
                _buttonContinue.clicked -= ButtonContinue;
            }

            if (_buttonRestart != null)
            {
                _buttonRestart.clicked -= ButtonRestart;
            }

            if (_buttonSkip != null)
            {
                _buttonSkip.clicked -= ButtonSkip;
            }

#if UNITY_INPUT
            if (_inputRestart != null)
            {
                _inputRestart.performed -= InputRestart;
            }

            if (_inputSkip != null)
            {
                _inputSkip.performed -= InputSkip;
            }

            if (_inputNext != null)
            {
                _inputNext.performed -= InputContinue;
            }
#endif
        }

#if UNITY_INPUT
        private void InputContinue(InputAction.CallbackContext context)
        {
            ButtonContinue();
        }
        private void InputRestart(InputAction.CallbackContext context)
        {
            ButtonRestart();
        }
        private void InputSkip(InputAction.CallbackContext context)
        {
            ButtonSkip();
        }
#endif

        private void ButtonContinue()
        {
            OnContinue?.Invoke();
        }

        private void ButtonRestart()
        {
            OnRestart?.Invoke();
        }

        private void ButtonSkip()
        {
            OnSkip?.Invoke();
        }
        public bool Continue(string text, string name, bool skipCurrent)
        {
            bool result = Continue(text, skipCurrent);

            if (result)
            {
                SetCharacterName(name);
            }

            return result;
        }
        public bool Continue(string text, string name, Sprite avatarLeft, Sprite avatarRight, bool skipCurrent)
        {
            bool result = Continue(text, skipCurrent);

            if (result)
            {
                SetCharacterName(name);

                if (avatarLeft != null)
                {
                    ShowAvatarLeft(avatarLeft);
                }
                else
                {
                    HideAvatarLeft();
                }

                if (avatarRight != null)
                {
                    ShowAvatarRight(avatarRight);
                }
                else
                {
                    HideAvatarRight();
                }
            }

            return result;
        }

        public void ShowNext()
        {
            _nextIcon.style.display = DisplayStyle.Flex;
            _nextIconSchedule.Start(1);
        }

        public void HideNext()
        {
            _nextIconSchedule.Pause();
            _nextIcon.style.display = DisplayStyle.None;
        }

        public void ShowAvatarRight(Sprite sprite)
        {
            _avatarRight.style.backgroundImage = new StyleBackground(sprite);
            _avatarRightContainer.style.display = DisplayStyle.Flex;
            _avatarRightSchedule.Start(500);
        }

        public void HideAvatarRight()
        {
            _avatarRightSchedule.Pause();
            _avatarRightContainer.style.display = DisplayStyle.None;
        }
        public void ShowAvatarLeft(Sprite sprite)
        {
            _avatarLeft.style.backgroundImage = new StyleBackground(sprite);
            _avatarLeftContainer.style.display = DisplayStyle.Flex;
            _avatarLeftSchedule.Start(1);
        }

        public void HideAvatarLeft()
        {
            _avatarLeftSchedule.Pause();
            _avatarLeftContainer.style.display = DisplayStyle.None;
        }

        public void SetCharacterName(string name)
        {
            if (_charName != null)
            {
                _charName.text = name;
            }
        }
    }
}