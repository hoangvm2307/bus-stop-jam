using UnityEngine;

#if UNITY_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
#endif

namespace CupkekGames.Systems
{
  [CreateAssetMenu(fileName = "SectionLocalization", menuName = "CupkekGames/Settings/SectionLocalization")]
  public class SettingsDataSectionLocalization : SettingsDataSection
  {
#if UNITY_LOCALIZATION
    [SerializeField] private LocaleIdentifier _localeIdentifier = new LocaleIdentifier("en");
    public LocaleIdentifier LocaleIdentifier
    {
      get
      {
        return _localeIdentifier;
      }
      set
      {
        _localeIdentifier = value;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(_localeIdentifier);
      }
    }

    public Locale Locale => LocalizationSettings.AvailableLocales.GetLocale(LocaleIdentifier);

    public int LocaleIndex => LocalizationSettings.AvailableLocales.Locales.IndexOf(Locale);
#endif

    public override bool Equals(object obj)
    {
      // Check for null and compare types
      if (obj == null || GetType() != obj.GetType())
        return false;

#if UNITY_LOCALIZATION
      // Cast and compare properties
      SettingsDataSectionLocalization b = (SettingsDataSectionLocalization)obj;

      return LocaleIdentifier == b.LocaleIdentifier;
#else
      return true;
#endif
    }

    public override int GetHashCode()
    {
#if UNITY_LOCALIZATION
      return LocaleIdentifier.GetHashCode();
#else
      return "REQUIRES UNITY_LOCALIZATION".GetHashCode();
#endif
    }
    public override void SaveToPlayerPrefs(string key)
    {
#if UNITY_LOCALIZATION
      PlayerPrefs.SetString($"{key}_LocaleIdentifier", _localeIdentifier.ToString());
      PlayerPrefs.Save();
#endif
    }
    public override void LoadFromPlayerPrefs(string key)
    {
#if UNITY_LOCALIZATION
      if (PlayerPrefs.HasKey($"{key}_LocaleIdentifier"))
      {
        string localeCode = PlayerPrefs.GetString($"{key}_LocaleIdentifier");
        _localeIdentifier = new LocaleIdentifier(localeCode);
      }
#endif
    }

    public override void CopyValuesFrom(SettingsDataSection section)
    {
#if UNITY_LOCALIZATION
      if (section is SettingsDataSectionLocalization copy)
      {
        _localeIdentifier = copy.LocaleIdentifier;
      }
#endif
    }

    public override void ApplySettings(SettingsDataSection settingsData)
    {
#if UNITY_LOCALIZATION
      if (settingsData is SettingsDataSectionLocalization copy)
      {
        LocaleIdentifier = copy.LocaleIdentifier;
      }
#endif
    }
  }
}