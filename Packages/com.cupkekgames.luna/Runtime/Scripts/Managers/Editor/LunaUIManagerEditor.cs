#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace CupkekGames.Luna.Editor
{
  [CustomEditor(typeof(LunaUIManager), true)]
  public class LunaUIManagerEditor : UnityEditor.Editor
  {
    [SerializeField] private VisualTreeAsset _editorTree = default;
    public override VisualElement CreateInspectorGUI()
    {
      // Create a container for the UI elements
      VisualElement container = _editorTree.Instantiate();

      VisualElement firstContainer = container.Q<VisualElement>("FirstContainer");

      Button openDocs = firstContainer.Q<Button>("OpenUrlDocs");
      openDocs.clicked += () =>
      {
        Application.OpenURL("https://docs.cupkek.games/");
      };

      VisualElement settingsContainer = container.Q<VisualElement>("SettingsContainer");


      VisualElement withInput = container.Q<VisualElement>("WithInput");
      VisualElement withoutInput = container.Q<VisualElement>("WithoutInput");

#if UNITY_INPUT
      withoutInput.style.display = DisplayStyle.None;
#else
      withInput.style.display = DisplayStyle.None;
#endif

      // Label settings = new Label("LunaUI Manager");
      // settings.style.unityTextAlign = TextAnchor.MiddleCenter;
      // settings.style.fontSize = new StyleLength(new Length(32, LengthUnit.Pixel));
      // settings.style.unityFontStyleAndWeight = FontStyle.Bold;
      // container.Add(settings);

      // // Fill the default inspector elements
      // InspectorElement.FillDefaultInspector(container, serializedObject, this);

      // // Reference to the target script
      // LunaUIManager myScript = (LunaUIManager)target;

      // // Create a horizontal layout for the buttons
      // var buttonContainer = new VisualElement();
      // buttonContainer.style.flexDirection = FlexDirection.Row; // Arrange buttons horizontally

      // Label debug = new Label("Debug");
      // container.Add(debug);

      // // Create and add the "Enable All UI Elements" button
      // var enableButton = new Button(() => myScript.SetEnabledAll(true))
      // {
      //   text = "Enable All UI Elements"
      // };
      // enableButton.style.flexGrow = 1;
      // buttonContainer.Add(enableButton);

      // // Create and add the "Disable All UI Elements" button
      // var disableButton = new Button(() => myScript.SetEnabledAll(false))
      // {
      //   text = "Disable All UI Elements"
      // };
      // disableButton.style.flexGrow = 1;
      // buttonContainer.Add(disableButton);

      // // Add the button container to the main container
      // container.Add(buttonContainer);

      return container;
    }

  }
}
#endif