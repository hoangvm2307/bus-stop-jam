using System;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Luna
{
    public class SpeechBubbleController : DialogueController
    {
        protected VisualElement _container;
        protected Button _buttonContinue;
        protected VisualElement _avatarLeftContainer;
        protected VisualElement _avatarLeft;
        protected VisualElement _avatarRightContainer;
        protected VisualElement _avatarRight;
        protected InputPrompt _nextIcon;

        // Events
        public event Action OnContinue;
        // State
        private TransitionToggleRepeat _avatarLeftSchedule;
        private TransitionToggleRepeat _avatarRightSchedule;
        private TransitionToggleRepeat _nextIconSchedule;
#if UNITY_INPUT
        // Input
        private InputAction _inputNext;
#endif
        protected override void Awake()
        {
            base.Awake();

            _container = ParentElement.Q<VisualElement>("SpeechBubble");

            _buttonContinue = ParentElement.Q<Button>("ContinueButton");

            _nextIcon = ParentElement.Q<InputPrompt>("NextIcon");
            _avatarLeftContainer = ParentElement.Q<VisualElement>("AvatarLeftContainer");
            _avatarRightContainer = ParentElement.Q<VisualElement>("AvatarRightContainer");
            _avatarLeft = ParentElement.Q<VisualElement>("AvatarLeft");
            _avatarRight = ParentElement.Q<VisualElement>("AvatarRight");

            _nextIconSchedule = new TransitionToggleRepeat(_nextIcon, "vn_next_icon_anim", 100);
            _avatarLeftSchedule = new TransitionToggleRepeat(_avatarLeft, "vn_avatar_anim", 200);
            _avatarRightSchedule = new TransitionToggleRepeat(_avatarRight, "vn_avatar_anim", 200);

#if UNITY_INPUT
            _inputNext = LunaUIManager.Instance.PlayerInput.actions[_nextIcon.InputActionName];
#endif

            HideAvatarLeft();
            HideAvatarRight();
        }

        private void OnEnable()
        {
            _buttonContinue.clicked += ButtonContinue;

#if UNITY_INPUT
            _inputNext.performed += InputContinue;
#endif
        }


        private void OnDisable()
        {
            _buttonContinue.clicked -= ButtonContinue;

#if UNITY_INPUT
            _inputNext.performed -= InputContinue;
#endif
        }

#if UNITY_INPUT
        private void InputContinue(InputAction.CallbackContext context)
        {
            ButtonContinue();
        }
#endif

        private void ButtonContinue()
        {
            OnContinue?.Invoke();
        }
        public bool Continue(string text, Sprite avatarLeft, Sprite avatarRight, bool skipCurrent)
        {
            bool result = Continue(text, skipCurrent);

            if (result)
            {
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

        public void SetPostion(Vector2 pos)
        {
            _container.style.left = pos.x;
            _container.style.top = pos.y;
        }
    }
}