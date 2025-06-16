#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Core.Editor
{
  [CustomPropertyDrawer(typeof(InputIconResult))]
  public class InputIconResultDrawer : PropertyDrawer
  {
    // Override OnGUI to handle the drawing of the custom property
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      // Begin property
      EditorGUI.BeginProperty(position, label, property);

      // Draw label
      position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

      // Calculate the rects for the fields
      Rect iconRect = new Rect(position.x, position.y, position.width * 0.75f, position.height);
      Rect sliceRect = new Rect(position.x + position.width * 0.8f, position.y, position.width * 0.2f, position.height);
      Rect squareRect = new Rect(position.x + position.width * 0.8f, position.y, position.width * 0.2f, position.height);

      // Draw Icon field
      SerializedProperty iconProp = property.FindPropertyRelative("Icon");
      EditorGUI.PropertyField(iconRect, iconProp, GUIContent.none);

      // Draw Slice field
      SerializedProperty sliceProp = property.FindPropertyRelative("Slice");
      EditorGUI.PropertyField(sliceRect, sliceProp, GUIContent.none);

      // Draw Square field
      SerializedProperty squareProp = property.FindPropertyRelative("Square");
      EditorGUI.PropertyField(squareRect, squareProp, GUIContent.none);

      // End property
      EditorGUI.EndProperty();
    }

    // Override GetPropertyHeight to return the height of the custom property
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      return 32f;
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
      // Create a container for the fields
      var container = new VisualElement();

      // Get references to the SerializedProperties
      var iconProperty = property.FindPropertyRelative("Icon");
      var sliceProperty = property.FindPropertyRelative("Slice");
      var squareProperty = property.FindPropertyRelative("Square");

      // Create ObjectField for the Icon property
      var iconField = new ObjectField("Icon")
      {
        objectType = typeof(Sprite),
        bindingPath = iconProperty.propertyPath
      };

      // Bind the object field to the serialized property
      iconField.BindProperty(iconProperty);

      // Create Toggle for the Slice property
      var sliceField = new Toggle("Slice")
      {
        bindingPath = sliceProperty.propertyPath
      };

      // Bind the toggle to the serialized property
      sliceField.BindProperty(sliceProperty);

      // Create Toggle for the Square property
      var squareField = new Toggle("Square")
      {
        bindingPath = squareProperty.propertyPath
      };

      // Bind the toggle to the serialized property
      squareField.BindProperty(squareProperty);
      
      // Add fields to the container
      container.Add(iconField);
      container.Add(sliceField);
      container.Add(squareField);
      
      return container;
    }
  }
}
#endif
