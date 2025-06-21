using CupkekGames.Core;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
    public class InputPromptDemo : UIViewComponent
    {
        // Views
        [SerializeField] private InputPromptDemoModal _modalPrefab;

        // Buttons
        private Button _openModal;

        protected override void Awake()
        {
            base.Awake();

            // Retrieve button elements from the UI and assign them to variables
            _openModal = ParentElement.Q<Button>("OpenModal");
        }

        private void OnEnable()
        {
            // Register button click event handlers
            _openModal.clicked += OpenModal;
        }

        private void OnDisable()
        {
            // Unregister button click event handlers to avoid memory leaks
            _openModal.clicked -= OpenModal;
        }

        public void OpenModal()
        {
            _openModal.SetEnabled(false); // disable open modal button

            InputPromptDemoModal modal = Instantiate(_modalPrefab);

            modal.OnOpenModal(_openModal);
        }
    }
}