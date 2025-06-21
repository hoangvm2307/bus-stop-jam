using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna;
using CupkekGames.Systems.UI;
public class MyMainMenu : UIViewComponent
{
    private Button _startButton;
    private Button _settingsButton;
    private Button _quitButton;

    protected override void Awake()
    {
        base.Awake();

        _startButton = UIDocument.rootVisualElement.Q<Button>("start-button");
        _settingsButton = UIDocument.rootVisualElement.Q<Button>("settings-button");
        _quitButton = UIDocument.rootVisualElement.Q<Button>("quit-button");
    }

    void OnEnable()
    {
        _startButton.RegisterCallback<ClickEvent>(OnStartClicked);
        _settingsButton.RegisterCallback<ClickEvent>(OnSettingsClicked);
        _quitButton.RegisterCallback<ClickEvent>(OnQuitClicked);
    }

    void OnDisable()
    {
        _startButton.UnregisterCallback<ClickEvent>(OnStartClicked);
        _settingsButton.UnregisterCallback<ClickEvent>(OnSettingsClicked);
        _quitButton.UnregisterCallback<ClickEvent>(OnQuitClicked);
    }
    private void OnStartClicked(ClickEvent e)
    {
        // Logic to start the game
        Debug.Log("Start button clicked");
        // You can load a new scene or start the game logic here
    }

    private void OnSettingsClicked(ClickEvent e)
    {
        UIPrefabLoaderString.Instance.Instantiate("Settings");
    }

    private void OnQuitClicked(ClickEvent e)
    {
        // Logic to quit the game
        Debug.Log("Quit button clicked");
        Application.Quit();
    }
}
