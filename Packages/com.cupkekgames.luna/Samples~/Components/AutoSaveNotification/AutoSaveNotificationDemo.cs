using CupkekGames.Core;
using CupkekGames.Luna.Library;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
    public class AutoSaveNotificationDemo : UIViewComponent
    {
        // References
        [SerializeField] private GameSaveManagerExample _saveManager;

        // Buttons
        private Button _autosave;

        protected override void Awake()
        {
            base.Awake();

            _autosave = ParentElement.Q<Button>("Autosave");
        }

        private void OnEnable()
        {
            _autosave.clicked += Autosave;
        }

        private void OnDisable()
        {
            _autosave.clicked -= Autosave;
        }

        private void Autosave()
        {
            _saveManager.Autosave(_saveManager.CurrentSave.Data);
        }
    }
}