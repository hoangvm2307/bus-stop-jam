using CupkekGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
    public class InputPromptDemoModal : UIViewComponent
    {
        private Button _previous;
        private Button _openModalButtonReference;

        protected override void Awake()
        {
            base.Awake();

            // Add an escape action to the modal to close it when triggered
            UIView.AddAction(new UIViewActionEscape(CloseModal));

            // Retrieve button elements from the UI and assign them to variables
            _previous = ParentElement.Q<Button>("Prev");
        }

        private void OnEnable()
        {
            // Register button click event handlers
            _previous.clicked += InputEscapeManager.OnEscape; // Trigger the escape action
        }

        private void OnDisable()
        {
            // Unregister button click event handlers to avoid memory leaks
            _previous.clicked -= InputEscapeManager.OnEscape;
        }

        public void CloseModal()
        {
            _openModalButtonReference.SetEnabled(true);

            FadeOutThenDestroy();
        }

        public void OnOpenModal(Button openModalButtonReference)
        {
            _openModalButtonReference = openModalButtonReference;
        }
    }
}