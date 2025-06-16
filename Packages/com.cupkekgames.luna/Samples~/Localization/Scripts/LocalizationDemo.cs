using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Localization
{
  public class LocalizationDemo : UIViewComponent
  {
    private Button _settings; // Reference to the settings button
    [SerializeField] private GameObject _settingsPrefab; // Prefab to instantiate

    protected override void Awake()
    {
      base.Awake();

      // Locate the button within the ParentElement
      _settings = ParentElement.Q<Button>("Settings");

      // Register the click event
      if (_settings != null)
      {
        _settings.clicked += OnSettingsClicked;
      }
      else
      {
        Debug.LogError("Settings button not found in the UI");
      }
    }

    private void OnSettingsClicked()
    {
      // Check if the settings prefab is assigned
      if (_settingsPrefab != null)
      {
        Instantiate(_settingsPrefab);
      }
      else
      {
        Debug.LogError("Settings prefab is not assigned.");
      }
    }

    private void OnDestroy()
    {
      // Unregister the click event to avoid memory leaks
      if (_settings != null)
      {
        _settings.clicked -= OnSettingsClicked;
      }
    }
  }
}
