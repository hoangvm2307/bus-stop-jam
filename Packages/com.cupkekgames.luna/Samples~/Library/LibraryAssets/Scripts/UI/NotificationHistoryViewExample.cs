using CupkekGames.Core;
using UnityEngine;

namespace CupkekGames.Luna.Library
{
    [RequireComponent(typeof(NotificationHistoryView))]
    public class NotificationHistoryViewExample : MonoBehaviour
    {
        [SerializeField] GameSaveManagerExample _saveManager;
        private NotificationHistoryView _view;
        public void Awake()
        {
            _view = GetComponent<NotificationHistoryView>();

            _view.Initialize(_saveManager.CurrentSave.Data.NotificationHistory);
        }
    }
}