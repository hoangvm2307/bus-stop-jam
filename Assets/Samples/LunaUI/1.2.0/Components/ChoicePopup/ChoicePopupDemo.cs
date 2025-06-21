using CupkekGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
    public class ConfirmationPopupDemo : UIViewComponent
    {
        // Views
        private ChoicePopupController _confirmationPopupController;

        // Buttons
        private Button _openModal;

        protected override void Awake()
        {
            base.Awake();

            _confirmationPopupController = GetComponent<ChoicePopupController>();

            _openModal = ParentElement.Q<Button>("OpenModal");
        }

        private void OnEnable()
        {
            // Register button click event handlers
            _openModal.clicked += OpenModal;

            _confirmationPopupController.OnButtonClick += OnButtonClick;
        }

        private void OnDisable()
        {
            // Unregister button click event handlers to avoid memory leaks
            _openModal.clicked -= OpenModal;

            _confirmationPopupController.OnButtonClick -= OnButtonClick;
        }

        public void OpenModal()
        {
            _confirmationPopupController.Fade.FadeIn();
        }

        public void CloseModal()
        {
            _confirmationPopupController.Fade.FadeOut();
        }

        public void OnButtonClick(int i)
        {
            if (i == 0)
            {
                Debug.Log("POPUP ACCEPT");
            }
            else
            if (i == 1)
            {
                Debug.Log("POPUP DECLINE");
            }
        }
    }
}