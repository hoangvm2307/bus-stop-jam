using System;
using CupkekGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    public class ChoicePopupController : UIViewComponent
    {
        // Settings
        [SerializeField] protected string _textHeader = "Save Changes?";
        [SerializeField] protected string _textBody = "If you don't save, it will revert back to previous settings.";
        [SerializeField]
        protected ChoicePopupChoice[] _choices = new ChoicePopupChoice[] {
            new ChoicePopupChoice("Accept", UIColorName.LIME),
            new ChoicePopupChoice("Decline", UIColorName.RED),
        };
        [SerializeField] protected string[] _buttonClasses = new string[] {
            "btn-xl",
        };
        public ChoicePopupChoice[] Choices
        {
            get
            {
                return _choices;
            }
            set
            {
                _choices = value;
            }
        }
        // UI
        protected Label _header;
        protected Label _body;
        protected VisualElement _buttonContainer;
        protected Button[] _buttons;
        public string TextHeader {
            get {
                return _header.text;
            }
            set {
                _header.text = value;
            }
        }
        public string TextBody {
            get {
                return _body.text;
            }
            set {
                _body.text = value;
            }
        }
        // Event
        public event Action<int> OnButtonClick;
        protected override void Awake()
        {
            base.Awake();

            UIView.AddAction(new UIViewActionEscape(UIView.Fade.FadeOut));

            _header = ParentElement.Q<Label>("Header");
            _body = ParentElement.Q<Label>("Body");

            _buttonContainer = ParentElement.Q<VisualElement>("ButtonContainer");

            if (_header != null)
            {
                _header.text = _textHeader;
            }

            if (_body != null)
            {
                _body.text = _textBody;
            }
        }

        protected virtual void OnEnable()
        {
            Fade.OnFadeInStart += OnFadeInStart;
            Fade.OnFadeOut += OnFadeOut;
        }

        protected virtual void OnDisable()
        {
            Fade.OnFadeInStart -= OnFadeInStart;
            Fade.OnFadeOut -= OnFadeOut;
        }

        protected virtual void OnFadeInStart()
        {
            RebuildButtons();

            _buttons[0].Focus();
        }

        public void RebuildButtons()
        {
            _buttonContainer.Clear();

            int count = _choices.Length;
            _buttons = new Button[count];

            for (int i = 0; i < count; i++)
            {
                Button button = new Button();
                button.AddToClassList("btn");

                ChoicePopupChoice choice = _choices[i];
                button.text = choice.Text;
                button.AddToClassList(choice.Color.ToString().ToLowerInvariant());

                foreach (string buttonClass in _buttonClasses)
                {
                    button.AddToClassList(buttonClass);
                }

                int index = i;
                button.clicked += () => OnButtonClickInner(index);

                _buttons[i] = button;
                _buttonContainer.Add(button);
            }
        }

        protected virtual void OnFadeOut()
        {
            _buttonContainer.Clear();
        }

        protected void OnButtonClickInner(int i)
        {
            OnButtonClick?.Invoke(i);
            Fade.FadeOut();

            foreach (Button button in _buttons)
            {
                button.SetEnabled(false);
            }
        }
    }
}