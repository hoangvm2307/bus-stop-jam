using CupkekGames.Core;
using UnityEngine;

#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Systems
{
  [CreateAssetMenu(fileName = "SectionRebind", menuName = "CupkekGames/Settings/SectionRebind")]
  public class SettingsDataSectionRebind : SettingsDataSection
  {
    [SerializeField] private string _bindingOverridesJson;
    public string BindingOverridesJson
    {
      get
      {
        return _bindingOverridesJson;
      }
      set
      {
        _bindingOverridesJson = value;
      }
    }

    public override bool Equals(object obj)
    {
      // Check for null and compare types
      if (obj == null || GetType() != obj.GetType())
        return false;

      // Cast and compare properties
      SettingsDataSectionRebind b = (SettingsDataSectionRebind)obj;

      return BindingOverridesJson == b.BindingOverridesJson;
    }

    public override int GetHashCode()
    {
      if (string.IsNullOrEmpty(BindingOverridesJson))
      {
        return 0;
      }

      return BindingOverridesJson.GetHashCode();
    }

    public override void SaveToPlayerPrefs(string key)
    {
      PlayerPrefs.SetString($"{key}_Rebind", BindingOverridesJson);
      PlayerPrefs.Save();
    }

    public override void LoadFromPlayerPrefs(string key)
    {
      if (PlayerPrefs.HasKey($"{key}_Rebind"))
      {
        _bindingOverridesJson = PlayerPrefs.GetString($"{key}_Rebind");
      }
    }

    public override void CopyValuesFrom(SettingsDataSection section)
    {
      if (section is SettingsDataSectionRebind copy)
      {
        BindingOverridesJson = copy.BindingOverridesJson;
      }
    }

    public override void ApplySettings(SettingsDataSection settingsData)
    {
#if UNITY_INPUT
      if (settingsData is SettingsDataSectionRebind copy)
      {
        BindingOverridesJson = copy.BindingOverridesJson;

        PlayerInput playerInput = InputDeviceManager.PlayerInput;

        if (playerInput == null)
        {
          Debug.LogError("PlayerInput not found");
          return;
        }

        if (!string.IsNullOrEmpty(BindingOverridesJson))
        {
          playerInput.actions.LoadBindingOverridesFromJson(BindingOverridesJson);
        }
        else
        {
          playerInput.actions.RemoveAllBindingOverrides();
        }
      }
#endif
    }
  }
}