using CupkekGames.Core;
using CupkekGames.Luna.Library;
using CupkekGames.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
    public class NotificationsDemo : UIViewComponent
    {
        // Settings
        [SerializeField] private Sprite _notificationIcon;
        [SerializeField] private NotificationSize _notificationSize;
        // References
        [SerializeField] private GameSaveManagerExample _saveManager;
        // Views
        private NotificationView _notificationView;

        // Buttons
        private Button _sendNotification;

        // State
        private int _notificationCount = 0;

        protected override void Awake()
        {
            base.Awake();

            _sendNotification = ParentElement.Q<Button>("SendNotification");
        }

        private void OnEnable()
        {
            _sendNotification.clicked += SendNotification;
        }

        private void OnDisable()
        {
            _sendNotification.clicked -= SendNotification;
        }
        private void Start()
        {
            _saveManager.CurrentSave.Data.NotificationHistory.ClearHistory();

            for (int i = 0; i < 4; i++)
            {
                _saveManager.CurrentSave.Data.NotificationHistory.PushNotification(CreateNotification());
            }

            _notificationView = GetComponent<NotificationView>();
            _notificationView.RegisterHistory(_saveManager.CurrentSave.Data.NotificationHistory);
        }


        private void SendNotification()
        {
            _saveManager.CurrentSave.Data.NotificationHistory.PushNotification(CreateNotification());
        }

        private NotificationData CreateNotification()
        {
            _notificationCount++;

            return new NotificationData("Title " + _notificationCount, "Body", new NotificationImageSourceExample(_notificationIcon), _notificationSize);
        }
    }
}