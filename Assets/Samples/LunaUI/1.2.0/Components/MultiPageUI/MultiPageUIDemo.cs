using CupkekGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
    public class MultiPageUIDemo : UIViewComponent
    {
        // Views
        private UIView _modal;
        private UIView _modalPageFirst;
        private UIView _modalPageSecond;

        // Buttons
        private Button _openModal;
        private Button _next;
        private Button _previous;

        protected override void Awake()
        {
            base.Awake();

            // Initialize the modal as a hidden UIView
            _modal = new UIView(gameObject, ParentElement.Q<VisualElement>("Modal"), UIStartVisibility.Invisible);

            // Initialize the first page of the modal and make it visible initially
            _modalPageFirst = new UIView(
                gameObject,
                _modal.ParentElement.Q<VisualElement>("Page1"),
                UIStartVisibility.Visible
            );

            // Initialize the second page of the modal and keep it hidden initially
            _modalPageSecond = new UIView(
                gameObject,
                _modal.ParentElement.Q<VisualElement>("Page2"),
                UIStartVisibility.Invisible
            );

            // Add an escape action to the modal to close it when triggered
            _modal.AddAction(new UIViewActionEscape(CloseModal));
            // Add an escape action to the second page to navigate back to the first page
            _modalPageSecond.AddAction(new UIViewActionEscape(() => ModalGoToPage(0)));

            // Retrieve button elements from the UI and assign them to variables
            _openModal = ParentElement.Q<Button>("OpenModal");
            _next = _modal.ParentElement.Q<Button>("Next");
            _previous = _modal.ParentElement.Q<Button>("Prev");


            // Add a UIViewActionEscape with Debug.Log to _modalPageFirst

            // Set the second parameter, bool activate, to false because we don't want Action to register immediately.
            // Because even of _modalPageFirst is visible, it's parent _modal is invisible.
            // Otherwise, this Action will register at the start, when modal is closed.
            _modalPageFirst.AddAction(new UIViewActionEscape(() => Debug.Log("First page: you can't escape from me that easily!")), false);

            _modal.AddChild(_modalPageFirst);
        }

        private void OnEnable()
        {
            // Register button click event handlers
            _openModal.clicked += OpenModal;
            _previous.clicked += InputEscapeManager.OnEscape; // Trigger the escape action
            _next.clicked += NextPage;
        }

        private void OnDisable()
        {
            // Unregister button click event handlers to avoid memory leaks
            _openModal.clicked -= OpenModal;
            _previous.clicked -= InputEscapeManager.OnEscape;
            _next.clicked -= NextPage;
        }

        public void OpenModal()
        {
            _modal.Fade.FadeIn();
            _previous.text = "Close Modal";
        }

        public void CloseModal()
        {
            _modal.Fade.FadeOut();
        }

        public void ModalGoToPage(int index)
        {
            if (index == 0)
            {
                _modalPageFirst.Fade.FadeIn();
                _modalPageSecond.Fade.FadeOut();
                _previous.text = "Close Modal";
            }
            else if (index == 1)
            {
                _modalPageFirst.Fade.FadeOut();
                _modalPageSecond.Fade.FadeIn();
                _previous.text = "Previous";
            }
        }

        private void NextPage()
        {
            if (!_modalPageSecond.IsVisible)
            {
                ModalGoToPage(1);
            }
        }
    }
}