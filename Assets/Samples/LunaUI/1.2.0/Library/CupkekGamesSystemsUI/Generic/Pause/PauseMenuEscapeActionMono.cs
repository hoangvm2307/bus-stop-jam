using UnityEngine;

namespace CupkekGames.Systems.UI
{
    public class PauseMenuEscapeActionMono : MonoBehaviour
    {
        [SerializeField] string _pauseMenuPrefabKey = "Pause";

        private PauseMenuEscapeAction _escapeAction;
        private void OnEnable()
        {
            _escapeAction = new PauseMenuEscapeAction(gameObject, _pauseMenuPrefabKey);
        }

        private void OnDisable()
        {
            _escapeAction.Dispose();
        }
    }
}