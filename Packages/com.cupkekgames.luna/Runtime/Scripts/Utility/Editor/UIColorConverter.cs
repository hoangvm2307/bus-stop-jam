#if UNITY_EDITOR
using UnityEditor.UIElements;
using System;

namespace CupkekGames.Luna.Editor
{
  public class UIColorConverter : UxmlAttributeConverter<UIColor>
  {
    public override UIColor FromString(string value)
    {
      var split = value.Split('|');

      string colorName = split[0];
      string colorValue = split[1];

      return new UIColor((UIColorName)Enum.Parse(typeof(UIColorName), colorName, true),
        (UIColorValue)Enum.Parse(typeof(UIColorValue), colorValue, true));
    }

    public override string ToString(UIColor value) => $"{value.Name}|{value.Value}";
  }
}
#endif