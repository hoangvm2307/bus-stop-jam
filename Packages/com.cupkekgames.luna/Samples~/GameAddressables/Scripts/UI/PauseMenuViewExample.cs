#if UNITY_ADDRESSABLES
using UnityEngine;
using CupkekGames.Systems;
using CupkekGames.Systems.UI;

namespace CupkekGames.Luna.Demo.Game.Addressables
{
  public class PauseMenuViewExample : PauseMenuView
  {
    [Header("Prefab Keys")]
    [SerializeField] string _settingsMenuKey = "Settings";
    [SerializeField] string _loadMenuKey = "SaveLoad";
    [Header("Main Menu Scene")]
    [SerializeField] string _mainMenuSceneKey = "MainMenu";

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
      SceneSO mainMenu = SceneDatabase.Instance.GetValue(_mainMenuSceneKey);

      SceneLoaderAddressable.Instance.LoadSceneAndUnLoadCurrent(mainMenu, SceneTransitionDatabase.Instance.Transitions.GetValue("Fade"));
    }

    protected override void OnButtonQuitGameClicked()
    {
      Application.Quit();
    }
  }
}
#endif