using UnityEngine;
using CupkekGames.Core;
using CupkekGames.Systems.UI;
using CupkekGames.Systems;

namespace CupkekGames.Luna.Demo.Game.Standart
{
  public class PauseMenuViewExample : PauseMenuView
  {
    [Header("Prefab Keys")]
    [SerializeField] string _settingsMenuKey = "Settings";
    [SerializeField] string _loadMenuKey = "SaveLoad";

    protected override void OnButtonContinueClicked()
    {
      FadeOutThenDestroy();
    }

    protected override void OnButtonLoadGameClicked()
    {
      UIPrefabLoaderString.Instance.Instantiate(_loadMenuKey);
    }

    protected override void OnButtonSettingsClicked()
    {
      UIPrefabLoaderString.Instance.Instantiate(_settingsMenuKey);
    }

    protected override void OnButtonMainMenuClicked()
    {
      SceneLoader.Instance.LoadScene(1, SceneTransitionDatabase.Instance.Transitions.GetValue("Fade"));
    }

    protected override void OnButtonQuitGameClicked()
    {
      Application.Quit();
    }
  }
}