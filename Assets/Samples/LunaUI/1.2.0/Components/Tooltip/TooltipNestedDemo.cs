using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Samples
{
  [RequireComponent(typeof(UIDocument))]
  public class TooltipNestedDemo : MonoBehaviour
  {
    [SerializeField] private TooltipController _tooltipController;
    
    private UIDocument _document;
    
    private void Awake()
    {
      _document = GetComponent<UIDocument>();
    }
    
    private void OnEnable()
    {
      // Set up the demo tooltips
      SetupTooltips();
    }
    
    private void SetupTooltips()
    {
      // Example: Nested tooltips
      var nestedTooltipButton = _document.rootVisualElement.Q<Button>("NestedTooltipButton");
      if (nestedTooltipButton != null)
      {
        SetupNestedTooltip(nestedTooltipButton);
      }
    }
    
    private void SetupNestedTooltip(Button button)
    {
      // Create a container for nested buttons
      var container = new VisualElement();
      container.AddToClassList("nested-buttons-container");
      
      // Create the main tooltip setup with title, description and the container as bottom content
      var mainTooltipSetup = new TooltipContainerSetup(
        null,
        new Label("Nested Tooltip"),
        new Label("This tooltip demonstrates multiple levels of nested tooltips."),
        container
      );
      
      // Create multiple buttons for different nested tooltips
      for (int i = 1; i <= 3; i++)
      {
        var nestedButton = new Button
        {
          text = $"Nested Level {i}"
        };
        nestedButton.AddToClassList("nested-tooltip-button");
        container.Add(nestedButton);
        
        // Create a container for sub-nested buttons if needed
        VisualElement subContainer = null;
        if (i < 3)
        {
          subContainer = new VisualElement();
          subContainer.AddToClassList("nested-buttons-container");
        }
        
        // Each button gets its own tooltip with further nested buttons
        var nestedTooltipSetup = new TooltipContainerSetup(
          null,
          new Label($"Level {i} Tooltip"),
          new Label($"This is a level {i} nested tooltip."),
          subContainer
        );
        
        // Add sub-nested buttons for level 2+
        if (i < 3 && subContainer != null)
        {
          for (int j = 1; j <= 2; j++)
          {
            var subNestedButton = new Button
            {
              text = $"Sub-level {j}"
            };
            subNestedButton.AddToClassList("nested-tooltip-button");
            subContainer.Add(subNestedButton);
            
            // Setup sub-nested tooltips
            var subNestedTooltipSetup = new TooltipContainerSetup(
              null,
              new Label($"Level {i}.{j} Tooltip"),
              new Label($"This is a sub-nested tooltip at level {i}.{j}."),
              null
            );
            
            var subNestedManipulator = new TooltipManipulator(gameObject, _tooltipController, 
              new List<TooltipContainerSetup> { subNestedTooltipSetup });
            subNestedButton.AddManipulator(subNestedManipulator);
          }
        }
        
        var nestedManipulator = new TooltipManipulator(gameObject, _tooltipController, 
          new List<TooltipContainerSetup> { nestedTooltipSetup });
        nestedButton.AddManipulator(nestedManipulator);
      }
      
      // Setup the tooltip manipulator for the main button
      var mainManipulator = new TooltipManipulator(gameObject, _tooltipController, 
        new List<TooltipContainerSetup> { mainTooltipSetup });
      button.AddManipulator(mainManipulator);
    }
  }
} 