using CupkekGames.Systems.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Library
{
    [RequireComponent(typeof(NotificationView))]
    public class BaseView : UIViewComponent
    {
        // Settings
        [SerializeField] private Sprite _notificationIcon;
        [SerializeField] private NotificationSize _notificationSize;
        // References
        [SerializeField] private GameSaveManagerExample _saveManager;
        private NotificationView _notificationView;
        // UI
        private Button _sendNotification;
        private Button _visualNovel;
        private Button _tutorialModal;
        private Button _speechBubble;
        private Button _inventory;
        private Button _autosave;
        // State
        private int _notificationCount;
        protected override void Awake()
        {
            base.Awake();

            UIDocument.rootVisualElement.dataSource = _saveManager.CurrentSave.Data;

            _sendNotification = ParentElement.Q<Button>("SendNotification");
            _visualNovel = ParentElement.Q<Button>("VisualNovelButton");
            _tutorialModal = ParentElement.Q<Button>("TutorialModalButton");
            _speechBubble = ParentElement.Q<Button>("SpeechBubbleButton");
            _inventory = ParentElement.Q<Button>("InventoryButton");
            _autosave = ParentElement.Q<Button>("Autosave");
        }

        private void OnEnable()
        {
            _sendNotification.clicked += SendNotification;
            _visualNovel.clicked += VisualNovelButton;
            _tutorialModal.clicked += TutorialModalButton;
            _speechBubble.clicked += SpeechBubbleButton;
            _inventory.clicked += InventoryButton;
            _autosave.clicked += Autosave;
        }

        private void OnDisable()
        {
            _sendNotification.clicked -= SendNotification;
            _visualNovel.clicked -= VisualNovelButton;
            _tutorialModal.clicked -= TutorialModalButton;
            _speechBubble.clicked -= SpeechBubbleButton;
            _inventory.clicked -= InventoryButton;
            _autosave.clicked -= Autosave;
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
        private void InventoryButton()
        {
            UIPrefabLoaderString.Instance.Instantiate("Inventory");
        }
        private void VisualNovelButton()
        {
            UIPrefabLoaderString.Instance.Instantiate("VisualNovel");
        }
        private void TutorialModalButton()
        {
            UIPrefabLoaderString.Instance.Instantiate("TutorialModal");
        }
        private void SpeechBubbleButton()
        {
            UIPrefabLoaderString.Instance.Instantiate("SpeechBubble");
        }

        private void Autosave()
        {
            _saveManager.Autosave(_saveManager.CurrentSave.Data);
        }
    }
}