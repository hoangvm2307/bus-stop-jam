using CupkekGames.Core;
using CupkekGames.Luna;
using UnityEngine;

namespace CupkekGames.Systems.UI
{
    public class PauseMenuEscapeAction : PauseMenuEscapeActionBase
    {
        string _pauseMenuPrefabKey = "Pause";

        public PauseMenuEscapeAction(GameObject parent, string pauseMenuPrefabKey) : base(parent)
        {
            _pauseMenuPrefabKey = pauseMenuPrefabKey;
        }

        protected override UIViewComponent InstantiatePauseMenu()
        {
            GameObject gameObject = UIPrefabLoaderString.Instance.Instantiate(_pauseMenuPrefabKey);

            if (gameObject == null)
            {
                return null;
            }

            return gameObject.GetComponent<UIViewComponent>();
        }
    }
}