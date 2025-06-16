using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    public class TransitionToggleRepeat
    {
        private VisualElement _ve;
        private string _ussClassName;
        private int _delayMsInterval;
        private IVisualElementScheduledItem _schedule;

        public TransitionToggleRepeat(VisualElement ve, string ussClassName, int delayMsInterval)
        {
            _ve = ve;
            _ussClassName = ussClassName;
            _delayMsInterval = delayMsInterval;
        }

        public void Start(long delayMs)
        {
            if (_schedule != null)
            {
                // already playing
                return;
            }

            _ve.RegisterCallback<TransitionEndEvent>(OnTransitionEnd);
            _schedule = _ve.schedule.Execute(() => _ve.ToggleInClassList(_ussClassName)).StartingIn(delayMs);
        }

        private void OnTransitionEnd(TransitionEndEvent evt)
        {
            _ve.schedule.Execute(() => _ve.ToggleInClassList(_ussClassName)).StartingIn(_delayMsInterval);
        }

        public void Pause()
        {
            if (_schedule != null)
            {
                _schedule.Pause();
                _ve.UnregisterCallback<TransitionEndEvent>(OnTransitionEnd);
                _schedule = null;
            }
        }
    }
}