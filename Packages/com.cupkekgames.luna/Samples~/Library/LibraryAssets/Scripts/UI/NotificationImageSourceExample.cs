using System;
using UnityEngine;

namespace CupkekGames.Luna.Library
{
    [Serializable]
    public class NotificationImageSourceExample : INotificationImageSource
    {
        public Sprite Sprite;
        public NotificationImageSourceExample(Sprite sprite)
        {
            Sprite = sprite;
        }

        public Sprite GetImage()
        {
            return Sprite;
        }
    }
}