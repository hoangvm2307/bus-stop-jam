#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace CupkekGames.Luna.Editor
{
  [CustomEditor(typeof(LoadingViewController))]
  public class LoadingViewControllerEditor : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      // Draw the default inspector first
      DrawDefaultInspector();

      // Reference to the target script
      LoadingViewController myScript = (LoadingViewController)target;

      if (GUILayout.Button("Fade In"))
      {
        myScript.OnLoadingScreenToggle(true, 2f);
      }

      if (GUILayout.Button("Fade Out"))
      {
        myScript.OnLoadingScreenToggle(false, 2f);
      }
    }

    public override VisualElement CreateInspectorGUI()
    {
      // Create a container for the UI elements
      var container = new VisualElement();

      // Fill the default inspector elements
      InspectorElement.FillDefaultInspector(container, serializedObject, this);

      // Reference to the target script
      LoadingViewController myScript = (LoadingViewController)target;

      // Create and add the "Fade In" button
      var fadeInButton = new Button(() => myScript.OnLoadingScreenToggle(true, 2f))
      {
        text = "Fade In"
      };
      fadeInButton.style.flexGrow = 1;
      container.Add(fadeInButton);

      // Create and add the "Fade Out" button
      var fadeOutButton = new Button(() => myScript.OnLoadingScreenToggle(false, 2f))
      {
        text = "Fade Out"
      };
      fadeOutButton.style.flexGrow = 1;
      container.Add(fadeOutButton);

      return container;
    }

  }
}
#endif