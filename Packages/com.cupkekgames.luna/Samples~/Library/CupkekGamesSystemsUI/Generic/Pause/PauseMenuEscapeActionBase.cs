using CupkekGames.Core;
using CupkekGames.Luna;
using UnityEngine;

namespace CupkekGames.Systems.UI
{
    public abstract class PauseMenuEscapeActionBase
    {
        private EscapeAction _escapeAction;
        private UIViewComponent _pauseMenuView;
        public PauseMenuEscapeActionBase(GameObject parent)
        {
            _escapeAction = new(parent, false);
            _escapeAction.OnEscape += OnEscape;
        }

        public void Dispose()
        {
            if (_pauseMenuView != null)
            {
                _pauseMenuView.Fade.OnFadeOutStart -= OnFadeOutStart;
            }

            _escapeAction.Dispose();
        }

        private void OnEscape()
        {
            _pauseMenuView = InstantiatePauseMenu();

            if (_pauseMenuView == null)
            {
                _escapeAction.Push();
                return;
            }

            _pauseMenuView.Fade.OnFadeOutStart += OnFadeOutStart;
        }

        private void OnFadeOutStart()
        {
            _pauseMenuView.Fade.OnFadeOutStart -= OnFadeOutStart;
            _escapeAction.Push();
        }


        /// <summary>
        /// Instantiate the pause menu GameObject and return the UI component on it.
        /// </summary>
        /// <returns>
        /// A the <see cref="UIViewComponent"/> of the pause menu.
        /// Returning null will re-push the Action that opens the pause menu.
        /// </returns>
        protected abstract UIViewComponent InstantiatePauseMenu();
    }
}