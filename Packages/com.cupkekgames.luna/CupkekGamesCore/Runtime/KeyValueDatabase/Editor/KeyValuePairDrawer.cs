#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Core.Editor
{
  [CustomPropertyDrawer(typeof(KeyValuePair<,>))]
  public class KeyValuePairDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      // Begin property drawing
      EditorGUI.BeginProperty(position, label, property);

      var indent = EditorGUI.indentLevel;
      EditorGUI.indentLevel = 0;

      // Calculate rects
      float quarterWidth = (position.width - 10) / 4;

      // Draw properties
      SerializedProperty keyProp = property.FindPropertyRelative("Key");
      SerializedProperty valueProp = property.FindPropertyRelative("Value");

      float keyWidth;
      float valueWidth;

      keyWidth = quarterWidth * 2 - 5;
      valueWidth = quarterWidth * 2 - 5;

      Rect keyRect = new Rect(position.x, position.y, keyWidth, position.height);
      Rect valueRect = new Rect(position.x + keyWidth + 10, position.y, valueWidth, position.height);

      // Ensure the fields can handle children correctly
      float keyHeight = EditorGUI.GetPropertyHeight(keyProp, GUIContent.none, true);
      float valueHeight = EditorGUI.GetPropertyHeight(valueProp, GUIContent.none, true);

      // Adjust position height based on the largest property
      float totalHeight = Mathf.Max(keyHeight, valueHeight);
      position.height = totalHeight;

      // Draw the properties again with the adjusted height
      keyRect.height = keyHeight;
      valueRect.height = valueHeight;

      DrawPropertyWithoutLabel(keyRect, keyProp);
      DrawPropertyWithoutLabel(valueRect, valueProp);

      // End property drawing
      EditorGUI.indentLevel = indent;
      EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      SerializedProperty keyProp = property.FindPropertyRelative("Key");
      SerializedProperty valueProp = property.FindPropertyRelative("Value");

      // Calculate heights for the properties
      float keyHeight = EditorGUI.GetPropertyHeight(keyProp, GUIContent.none, true);
      float valueHeight = EditorGUI.GetPropertyHeight(valueProp, GUIContent.none, true);

      // Return the total height needed, including spacing between properties
      return Mathf.Max(keyHeight, valueHeight) + 2f; // Adjust 2f to desired spacing
    }

    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
      // Create property container element.
      var container = new VisualElement();

      container.style.flexDirection = FlexDirection.Row;

      // Create property fields.
      var propertyKey = property.FindPropertyRelative("Key");
      var keyField = DrawPropertyWithoutLabelUITK(propertyKey);
      keyField.style.flexGrow = 1;
      keyField.style.minWidth = new StyleLength(new Length(30, LengthUnit.Percent));

      var propertyValue = property.FindPropertyRelative("Value");
      var valueField = DrawPropertyWithoutLabelUITK(propertyValue);
      valueField.style.flexGrow = 1;
      valueField.style.minWidth = new StyleLength(new Length(30, LengthUnit.Percent));

      // Add fields to the container.
      container.Add(keyField);
      container.Add(valueField);

      return container;
    }

    private void DrawPropertyWithoutLabel(Rect rect, SerializedProperty property)
    {
      switch (property.propertyType)
      {
        case SerializedPropertyType.Generic:
          EditorGUI.PropertyField(rect, property, new GUIContent(property.displayName), true);
          break;
        default:
          EditorGUI.PropertyField(rect, property, GUIContent.none);
          break;
      }
    }
    private PropertyField DrawPropertyWithoutLabelUITK(SerializedProperty property)
    {
      switch (property.propertyType)
      {
        case SerializedPropertyType.Generic:
          return new PropertyField(property);
        default:
          return new PropertyField(property, "");
      }
    }
  }
}
#endif
