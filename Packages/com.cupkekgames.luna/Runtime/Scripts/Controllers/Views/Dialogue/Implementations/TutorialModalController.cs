using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Luna
{
    public class TutorialModalController : DialogueController
    {
        protected Button _buttonRestart;
        protected InputPrompt _buttonNext;
        protected InputPrompt _buttonPrevious;
        protected List<Button> _returnButtons;
        protected Label _title;
        protected Label _subtitle;
        protected VisualElement _image;

        // Events
        public event Action OnNext;
        public event Action OnPrevious;
#if UNITY_INPUT
        // Input
        private InputAction _inputNext;
        private InputAction _inputPrevious;
#endif
        protected override void Awake()
        {
            base.Awake();

            _buttonNext = ParentElement.Q<InputPrompt>("ContinueButton");
            _buttonPrevious = ParentElement.Q<InputPrompt>("PreviousButton");
            _returnButtons = ParentElement.Query<Button>("ReturnButton").ToList();

            _title = ParentElement.Q<Label>("Title");
            SetTitle("");
            _subtitle = ParentElement.Q<Label>("Subtitle");
            SetSubtitle("");

            _image = ParentElement.Q<VisualElement>("Image");

            HideImage();

            UIView.AddAction(new UIViewActionEscape(ReturnClicked));

#if UNITY_INPUT
            _inputNext = LunaUIManager.Instance.PlayerInput.actions[_buttonNext.InputActionName];
            _inputPrevious = LunaUIManager.Instance.PlayerInput.actions[_buttonPrevious.InputActionName];
#endif
        }

        private void OnEnable()
        {
            _buttonNext.clicked += ButtonContinue;
            _buttonPrevious.clicked += ButtonPrevious;

            foreach (Button button in _returnButtons)
            {
                button.clicked += ReturnClicked;
            }

#if UNITY_INPUT
            _inputNext.performed += InputContinue;
            _inputPrevious.performed += InputPrevious;
#endif
        }


        private void OnDisable()
        {
            _buttonNext.clicked -= ButtonContinue;
            _buttonPrevious.clicked -= ButtonPrevious;

            foreach (Button button in _returnButtons)
            {
                button.clicked -= ReturnClicked;
            }

#if UNITY_INPUT
            _inputNext.performed -= InputContinue;
            _inputPrevious.performed -= InputPrevious;
#endif
        }

#if UNITY_INPUT
        private void InputContinue(InputAction.CallbackContext context)
        {
            ButtonContinue();
        }

        private void InputPrevious(InputAction.CallbackContext context)
        {
            ButtonPrevious();
        }
#endif

        private void ButtonContinue()
        {
            OnNext?.Invoke();
        }

        private void ButtonPrevious()
        {
            OnPrevious?.Invoke();
        }
        public bool Continue(string text, string title, string subtitle, Sprite image, bool skipCurrent)
        {
            bool result = Continue(text, skipCurrent);

            if (result)
            {
                SetTitle(title);
                SetSubtitle(subtitle);

                if (image != null)
                {
                    ShowImage(image);
                }
                else
                {
                    HideImage();
                }
            }

            return result;
        }

        public void ShowImage(Sprite sprite)
        {
            _image.style.backgroundImage = new StyleBackground(sprite);
            _image.style.display = DisplayStyle.Flex;
        }

        public void HideImage()
        {
            _image.style.display = DisplayStyle.None;
        }

        public void SetTitle(string name)
        {
            _title.text = name;
        }
        public void SetSubtitle(string name)
        {
            _subtitle.text = name;
        }
        public void ReturnClicked()
        {
            foreach (Button button in _returnButtons)
            {
                button.SetEnabled(false);
            }

            FadeOutThenDestroy();
        }

        public void SetEnabledNext(bool enabled)
        {
            _buttonNext.SetEnabled(enabled);
        }
        public void SetEnabledPrevious(bool enabled)
        {
            _buttonPrevious.SetEnabled(enabled);
        }
    }
}