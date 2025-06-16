using System;
using UnityEngine;

namespace CupkekGames.Luna
{
    [Serializable]
    public class NotificationData
    {
        public Guid Id = Guid.NewGuid();
        public string Title;
        public string Body;
        public INotificationImageSource ImageSource;
        public bool IsDismissed = false;
        public NotificationSize Size = NotificationSize.Default;
        public NotificationAction OnClickAction;

        public NotificationData(string title, string body, INotificationImageSource imageSource, NotificationSize size)
        {
            Title = title;
            Body = body;
            ImageSource = imageSource;
            Size = size;
        }

        public Sprite GetImage()
        {
            if (ImageSource == null)
            {
                return null;
            }

            return ImageSource.GetImage();
        }

        public void OnClick()
        {
            if (OnClickAction == null)
            {
                return;
            }

            OnClickAction.Execute(Title, Body);
        }
    }
}