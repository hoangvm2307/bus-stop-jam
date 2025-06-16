using System;
using CupkekGames.Core;
using UnityEngine;

namespace CupkekGames.Luna
{
    public class UIViewActionEscape : IUIViewAction
    {
        // Custom Action
        private Action _onEscape;

        // State
        private bool _escaped = true;
        public bool _debug;
        private Guid _key;

        public UIViewActionEscape(Action onEscape = null, bool debug = false)
        {
            _onEscape = onEscape;

            _debug = debug;
            _key = Guid.NewGuid();
        }

        public void OnFadeOut()
        {
            if (_debug)
            {
                Debug.Log("Action Out");
            }
            if (!_escaped)
            {
                if (_debug)
                {
                    Debug.Log("Action Out Pop");
                }
                Dispose();
            }
        }

        private void OnEscape()
        {
            _onEscape?.Invoke();
            _escaped = true;
        }

        public void Dispose()
        {
            InputEscapeManager.PopWithoutExecute(_key);
            _escaped = true;
        }

        public void OnFadeInStart()
        {
            if (_debug)
            {
                Debug.Log("Action In");
            }
            if (_escaped)
            {
                if (_debug)
                {
                    Debug.Log("Action In Push");
                }
                InputEscapeManager.Push(OnEscape, _key);
                _escaped = false;
            }
        }
    }
}