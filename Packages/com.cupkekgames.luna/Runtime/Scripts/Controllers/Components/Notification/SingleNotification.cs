

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    public class SingleNotification
    {
        public VisualElement Parent;
        public NotificationData NotificationData;
        public event Action OnDissmiss;
        public Button DismissButton;
        public SingleNotification(VisualElement parent, NotificationData notificationData, UIColorName color)
        {
            Parent = parent;
            Parent.usageHints = UsageHints.DynamicTransform;
            NotificationData = notificationData;

            parent.pickingMode = PickingMode.Ignore;

            Label titleLabel = parent.Q<Label>("Title");
            if (notificationData.Title != null)
            {
                titleLabel.text = notificationData.Title;
            }
            else
            {
                titleLabel.style.display = DisplayStyle.None;
            }

            Label bodyLabel = parent.Q<Label>("Body");
            if (notificationData.Body != null)
            {
                bodyLabel.text = notificationData.Body;
            }
            else
            {
                bodyLabel.style.display = DisplayStyle.None;
            }

            VisualElement imageElement = parent.Q<VisualElement>("Image");
            Sprite image = notificationData.GetImage();
            if (image != null)
            {
                imageElement.style.backgroundImage = new StyleBackground(image);
            }
            else
            {
                imageElement.style.display = DisplayStyle.None;
            }

            DismissButton = parent.Q<Button>("DismissButton");
            DismissButton.clicked += Dismiss;
            DismissButton.AddToClassList(color.ToString().ToLowerInvariant());

            parent.AddToClassList("notification");

            NotificationSize size = notificationData.Size;
            if (size == NotificationSize.Small)
            {
                parent.AddToClassList("notification-sm");
            }
            else if (size == NotificationSize.Large)
            {
                parent.AddToClassList("notification-lg");
            }
        }

        public void Dismiss()
        {
            DismissButton.clicked -= Dismiss;

            OnDissmiss?.Invoke();
        }
    }
}