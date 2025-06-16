using System;
using UnityEngine;

namespace CupkekGames.Luna
{
    [Serializable]
    public abstract class NotificationAction
    {
        [SerializeField] protected string _title;
        [SerializeField] protected string _body;
        public virtual void Execute(string title, string body)
        {
            _title = title;
            _body = body;
        }
    }
}