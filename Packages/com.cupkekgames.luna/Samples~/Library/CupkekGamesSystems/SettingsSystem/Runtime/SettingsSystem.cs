using UnityEngine;
using CupkekGames.Core;

namespace CupkekGames.Systems
{
  public class SettingsSystem : Singleton<SettingsSystem>
  {
    [SerializeField] private SettingsDataSO _currentSettings;
    public SettingsDataSO CurrentSettings => _currentSettings;
    [SerializeField] private SettingsDataSO _defaultSettings;
    public SettingsDataSO DefaultSettings => _defaultSettings;

    private void Start()
    {
      CurrentSettings.CopyValuesFrom(DefaultSettings);
      CurrentSettings.LoadFromPlayerPrefs();
      ApplySettings(CurrentSettings);
    }

    public void SaveAndApplySettings()
    {
      CurrentSettings.SaveToPlayerPrefs();
      ApplySettings(_currentSettings);
    }

    public void ApplySettings(SettingsDataSO settingsData)
    {
      Debug.Log("Applying settings");

      foreach (var key in _currentSettings.Keys)
      {
        _currentSettings.GetValue(key).ApplySettings(settingsData.GetValue(key));
      }
    }
  }
}