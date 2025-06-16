using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Core;
using CupkekGames.Luna;

namespace CupkekGames.Luna
{
  public abstract class PauseMenuView : UIViewComponent
  {
    private Button _buttonContinue;
    private Button _buttonLoadGame;
    private Button _buttonSettings;
    private Button _buttonMainMenu;
    private Button _buttonQuitGame;

    // Start is called before the first frame update
    protected override void Awake()
    {
      base.Awake();

      _buttonContinue = UIDocument.rootVisualElement.Q<Button>("ButtonContinue");
      _buttonLoadGame = UIDocument.rootVisualElement.Q<Button>("ButtonLoad");
      _buttonSettings = UIDocument.rootVisualElement.Q<Button>("ButtonSettings");
      _buttonMainMenu = UIDocument.rootVisualElement.Q<Button>("ButtonMainMenu");
      _buttonQuitGame = UIDocument.rootVisualElement.Q<Button>("ButtonQuit");
    }

    protected virtual void OnEnable()
    {
      _buttonContinue.clicked += OnButtonContinueClicked;
      _buttonLoadGame.clicked += OnButtonLoadGameClicked;
      _buttonSettings.clicked += OnButtonSettingsClicked;
      _buttonMainMenu.clicked += OnButtonMainMenuClicked;
      _buttonQuitGame.clicked += OnButtonQuitGameClicked;
    }

    protected virtual void OnDisable()
    {
      _buttonContinue.clicked -= OnButtonContinueClicked;
      _buttonLoadGame.clicked -= OnButtonLoadGameClicked;
      _buttonSettings.clicked -= OnButtonSettingsClicked;
      _buttonMainMenu.clicked -= OnButtonMainMenuClicked;
      _buttonQuitGame.clicked -= OnButtonQuitGameClicked;
    }

    protected virtual void OnButtonContinueClicked()
    {
      FadeOutThenDestroy();
    }

    protected abstract void OnButtonLoadGameClicked();

    protected abstract void OnButtonSettingsClicked();

    protected abstract void OnButtonMainMenuClicked();
    protected abstract void OnButtonQuitGameClicked();
  }
}