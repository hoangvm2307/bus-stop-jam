using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
  public class TooltipDemo : MonoBehaviour
  {
    [SerializeField] private TooltipController _tooltipController;
    private UIDocument _uiDocument;

    private void Start()
    {
      _uiDocument = GetComponent<UIDocument>();

      // Setup tooltips for the "Alignment" visual elements
      VisualElement alignment = _uiDocument.rootVisualElement.Q<VisualElement>("Alignment");

      foreach (var e in alignment.Children())
      {
        TooltipContainerSetup containerSetup = CreateContainerSetup(UIColorName.SLATE);

        TooltipManipulator manipulator = new TooltipManipulator(
          gameObject,
          _tooltipController
        );
        manipulator.SetSetup(containerSetup);

        // Add the manipulator to the element
        e.AddManipulator(manipulator);
      }

      // Setup tooltips for the "FollowMouse" visual elements
      VisualElement followMouse = _uiDocument.rootVisualElement.Q<VisualElement>("FollowMouse");

      foreach (var e in followMouse.Children())
      {
        TooltipContainerSetup containerSetup = CreateContainerSetup(UIColorName.SLATE);

        TooltipManipulator manipulator = new TooltipManipulator(
          gameObject,
          _tooltipController
        );
        manipulator.SetSetup(containerSetup);

        // Add the manipulator to the element
        e.AddManipulator(manipulator);
      }

      // Setup tooltips for the "FollowMouse" visual elements
      VisualElement multipleColumns = _uiDocument.rootVisualElement.Q<VisualElement>("Multiple");

      foreach (var e in multipleColumns.Children())
      {
        List<TooltipContainerSetup> containerSetup = new()
        {
          CreateContainerSetup(UIColorName.SLATE),
          CreateContainerSetup(UIColorName.FUCHSIA)
        };

        TooltipManipulator manipulator = new TooltipManipulator(
          gameObject,
          _tooltipController,
          containerSetup
        );

        // Add the manipulator to the element
        e.AddManipulator(manipulator);
      }

      // Setup tooltips for the "Colors" visual elements
      List<VisualElement> colors = _uiDocument.rootVisualElement.Query<VisualElement>("Colors").ToList();

      int colorIndex = 8;

      // Iterate through each color line and assign a tooltip manipulator
      foreach (var line in colors)
      {
        foreach (var e in line.Children())
        {
          UIColorName colorName = (UIColorName)colorIndex;

          TooltipContainerSetup containerSetup = CreateContainerSetup(colorName);

          TooltipManipulator manipulator = new TooltipManipulator(
            gameObject,
            _tooltipController
          );
          manipulator.SetSetup(containerSetup);

          // Add the manipulator to the element
          e.AddManipulator(manipulator);

          e.Q<Label>().text = colorName.ToString();

          // button color
          e.AddToClassList(colorName.ToString().ToLowerInvariant());

          colorIndex++;
        }
      }
    }

    private TooltipContainerSetup CreateContainerSetup(UIColorName colorName)
    {
      // Create a new VisualElement to represent an image
      VisualElement image = new VisualElement();
      image.AddToClassList("size-128"); // Add size class
      image.AddToClassList("rounded-lg"); // Add border radius class

      // Apply background color classes based on the color name
      string colorClass = new UIColor(colorName, UIColorValue.V_400).GetUssClassBG();
      image.AddToClassList(colorClass);

      // Create UIColor instances for both light and dark variations
      UIColor color = new UIColor(colorName, UIColorValue.V_400);
      UIColor colorDark = new UIColor(colorName, UIColorValue.V_900);

      // Create a new tooltip manipulator for the element
      return new TooltipContainerSetup(
          image,
          new Label("Title Slot"),
          new Label("Body Slot"),
          new Label("Bottom Slot"),
          colorDark,
          color
        );
    }
  }
}
