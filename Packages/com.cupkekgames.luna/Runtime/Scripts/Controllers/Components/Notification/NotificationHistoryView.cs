using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  public class NotificationHistoryView : UIViewComponent
  {
    // References
    private NotificationHistory _notificationHistory;
    // UI References
    [SerializeField] private VisualTreeAsset _notificationStaticTemplate;
    private ListView _listView;
    private List<NotificationData> _history;
    private Button _buttonReturn;

    public void Initialize(NotificationHistory notificationHistory)
    {
      _notificationHistory = notificationHistory;

      _listView = ParentElement.Q<ListView>("NotificationList");

      _history = new List<NotificationData>(_notificationHistory.History);
      _history.Reverse();

      _buttonReturn = ParentElement.Q<Button>("ReturnButton");
      _buttonReturn.clicked += FadeOutThenDestroy;

      FillListView();
    }
    private void FillListView()
    {
      _listView.makeItem = () =>
      {
        return _notificationStaticTemplate.Instantiate();
      };

      _listView.bindItem = (item, index) =>
      {
        NotificationData notificationData = _history[index];

        Label titleLabel = item.Q<Label>("Title");
        if (notificationData.Title != null)
        {
          titleLabel.text = notificationData.Title;
        }
        else
        {
          titleLabel.style.display = DisplayStyle.None;
        }

        Label bodyLabel = item.Q<Label>("Body");
        if (notificationData.Body != null)
        {
          bodyLabel.text = notificationData.Body;
        }
        else
        {
          bodyLabel.style.display = DisplayStyle.None;
        }

        VisualElement imageElement = item.Q<VisualElement>("Image");
        Sprite image = notificationData.GetImage();
        if (image != null)
        {
          imageElement.style.backgroundImage = new StyleBackground(image);
        }
        else
        {
          imageElement.style.display = DisplayStyle.None;
        }
      };

      _listView.itemsSource = _history;
    }
  }
}