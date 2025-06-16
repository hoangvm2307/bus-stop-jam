#if UNITY_LOCALIZATION
using UnityEngine.Localization;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  public class LocalizedVisualElement
  {
    private VisualElement _visualElement;
    public LocalizedVisualElement(VisualElement visualElement, string table, string entry)
    {
      _visualElement = visualElement;

      SetLocalizedString(table, entry);
    }

    public void SetLocalizedString(string table, string entry)
    {
      LocalizedString localizedString = new LocalizedString(table, entry);

      _visualElement.SetBinding("text", localizedString);
    }


    public LocalizedString GetLocalizedString()
    {
      return _visualElement.GetBinding("text") as LocalizedString;
    }
  }
}
#endif
