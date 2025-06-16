#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace CupkekGames.Luna.Editor
{
  [CustomEditor(typeof(UIViewComponent), true)]
  public class UIViewComponentEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      // Draw the default inspector first
      DrawDefaultInspector();

      // Reference to the target script
      UIViewComponent myScript = (UIViewComponent)target;

      if (GUILayout.Button("Fade In"))
      {
        myScript.Fade.FadeIn();
      }

      if (GUILayout.Button("Fade Out"))
      {
        myScript.Fade.FadeOut();
      }
    }
    public override VisualElement CreateInspectorGUI()
    {
      // Create a container for the UI elements
      var container = new VisualElement();

      // Fill the default inspector elements
      InspectorElement.FillDefaultInspector(container, serializedObject, this);

      // Reference to the target script
      UIViewComponent myScript = (UIViewComponent)target;

      // Create a horizontal layout for the buttons
      var buttonContainer = new VisualElement();
      buttonContainer.style.flexDirection = FlexDirection.Row; // Arrange buttons horizontally

      // Create and add the "Fade In" button
      var fadeInButton = new Button(() => myScript.Fade.FadeIn())
      {
        text = "Fade In"
      };
      fadeInButton.style.flexGrow = 1;
      buttonContainer.Add(fadeInButton);

      // Create and add the "Fade Out" button
      var fadeOutButton = new Button(() => myScript.Fade.FadeOut())
      {
        text = "Fade Out"
      };
      fadeOutButton.style.flexGrow = 1;
      buttonContainer.Add(fadeOutButton);

      // Add the button container to the main container
      container.Add(buttonContainer);

      return container;
    }

  }
}
#endif