using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
  public class TooltipUpdateDemo : MonoBehaviour
  {
    [SerializeField] private TooltipController _tooltipController;
    private UIDocument _uiDocument;
    private TooltipManipulator _manipulatorUpdate;
    private VisualElement _displayToggle;
    private VisualElement _switch1;
    private VisualElement _switch2;
    private VisualElement _switchContainer;
    private bool _switched = false;

    private void Start()
    {
      _uiDocument = GetComponent<UIDocument>();

      VisualElement eStatic = _uiDocument.rootVisualElement.Q<VisualElement>("TooltipStatic");
      TooltipManipulator manipulatorStatic = new TooltipManipulator(
        gameObject,
        _tooltipController
      );
      manipulatorStatic.SetSetup(CreateContainerSetup(UIColorName.RED));
      eStatic.AddManipulator(manipulatorStatic);

      VisualElement eUpdate = _uiDocument.rootVisualElement.Q<VisualElement>("TooltipUpdate");

      _manipulatorUpdate = new TooltipManipulator(
        gameObject,
        _tooltipController
      );
      _manipulatorUpdate.SetSetup(CreateContainerSetup(UIColorName.RED));

      // Add the manipulator to the element
      eUpdate.AddManipulator(_manipulatorUpdate);

      // Start the update coroutine
      StartCoroutine(UpdateTooltips());

      _displayToggle = _uiDocument.rootVisualElement.Q<VisualElement>("TooltipToggle");
      TooltipManipulator manipulatorDisplay = new TooltipManipulator(
        gameObject,
        _tooltipController
      );
      manipulatorDisplay.SetSetup(CreateContainerSetup(UIColorName.RED));
      _displayToggle.AddManipulator(manipulatorDisplay);

      _switch1 = _uiDocument.rootVisualElement.Q<VisualElement>("TooltipSwitch1");
      TooltipManipulator manipulatorSwitch1 = new TooltipManipulator(
        gameObject,
        _tooltipController
      );
      manipulatorSwitch1.SetSetup(CreateContainerSetup(UIColorName.BLUE));
      _switch1.AddManipulator(manipulatorSwitch1);

      _switch2 = _uiDocument.rootVisualElement.Q<VisualElement>("TooltipSwitch2");
      TooltipManipulator manipulatorSwitch2 = new TooltipManipulator(
        gameObject,
        _tooltipController
      );
      manipulatorSwitch2.SetSetup(CreateContainerSetup(UIColorName.LIME));
      _switch2.AddManipulator(manipulatorSwitch2);

      _switchContainer = _uiDocument.rootVisualElement.Q<VisualElement>("SwitchContainer");
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

      string randomTitle = "Title " + Random.Range(0, 100);
      string randomBody = "Body " + Random.Range(0, 100);
      string randomBottom = "Bottom " + Random.Range(0, 100);

      // Create a new tooltip manipulator for the element
      return new TooltipContainerSetup(
          image,
          new Label(randomTitle),
          new Label(randomBody),
          new Label(randomBottom),
          colorDark,
          color
        );
    }

    private IEnumerator UpdateTooltips()
    {
      while (true)
      {
        yield return new WaitForSeconds(2f);

        // Update the tooltip setup
        UIColorName randomColor = (UIColorName)Random.Range(0, 10);
        _manipulatorUpdate.SetSetup(CreateContainerSetup(randomColor));
        _manipulatorUpdate.UpdateDisplay();

        _displayToggle.style.display = _displayToggle.style.display == DisplayStyle.Flex ? DisplayStyle.None : DisplayStyle.Flex;
        _switchContainer.style.scale = _switched ? new StyleScale(new Vector2(1, 1)) : new StyleScale(new Vector2(-1, 1));
        _switched = !_switched;
      }
    }
  }
}
