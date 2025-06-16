using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CupkekGames.Core;
using UnityEngine;

namespace CupkekGames.Luna
{
    [Serializable]
    public class NotificationHistory
    {
        [SerializeField] private List<NotificationData> _history = new List<NotificationData>();
        public ReadOnlyCollection<NotificationData> History => _history.AsReadOnly();
        [SerializeField] public int MaxNotifications = 20;
        public event Action<Guid> OnPushNotification;
        public void PushNotification(NotificationData notificationData)
        {
            _history.Add(notificationData);

            if (_history.Count > MaxNotifications)
            {
                _history.RemoveAt(0);
            }

            OnPushNotification?.Invoke(notificationData.Id);
        }

        public List<NotificationData> GetLastNonDismissed(int amount)
        {
            List<NotificationData> notifications = new();

            for (int i = _history.Count - 1; i >= 0; i--)
            {
                if (_history[i].IsDismissed)
                {
                    continue;
                }

                notifications.Add(_history[i]);

                if (notifications.Count >= amount)
                {
                    break;
                }
            }

            return notifications;
        }

        public NotificationData GetNotification(Guid id)
        {
            return _history.Find(x => x.Id == id);
        }

        public void ClearHistory()
        {
            _history.Clear();
        }
    }
}