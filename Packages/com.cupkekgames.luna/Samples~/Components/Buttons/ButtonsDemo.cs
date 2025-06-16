using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
  public class ButtonsDemo : MonoBehaviour
  {
    private UIDocument _uiDocument;

    private void Awake()
    {
      _uiDocument = GetComponent<UIDocument>();

      List<VisualElement> pairs = _uiDocument.rootVisualElement.Query<VisualElement>("ButtonPair").ToList();

      int colorIndex = 0;

      // Iterate through each color line and assign a tooltip manipulator
      foreach (var line in pairs)
      {
        foreach (var e in line.Children())
        {
          // Assign color names based on the index
          UIColorName colorName = (UIColorName)colorIndex;
          string colorClass = colorName.ToString().ToLower();

          e.AddToClassList(colorClass);

          ((Button)e).text = colorClass;
        }

        colorIndex++;
      }
    }
  }
}
