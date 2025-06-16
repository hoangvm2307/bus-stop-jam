#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Core.Editor
{
  [CustomPropertyDrawer(typeof(KeyValueDatabase<,>), true)]
  public class KeyValueDatabaseDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      // Begin property drawing
      EditorGUI.BeginProperty(position, label, property);

      var indent = EditorGUI.indentLevel;
      EditorGUI.indentLevel = 0;

      SerializedProperty listProperty = property.FindPropertyRelative("_list");

      // Draw the list first
      EditorGUI.PropertyField(position, listProperty, new GUIContent(property.displayName), true);

      // Check if there are duplicate keys and draw warning if needed
      if (CanCheckDuplicate(listProperty) && HasDuplicateKeys(listProperty))
      {
        // Adjust the position to below the list
        Rect warningRect = new Rect(position.x, position.y + EditorGUI.GetPropertyHeight(listProperty, true) + EditorGUIUtility.standardVerticalSpacing, position.width, EditorGUIUtility.singleLineHeight * 3);
        EditorGUI.HelpBox(warningRect, "Warning: Duplicate keys detected in the list.", MessageType.Warning);
      }

      // End property drawing
      EditorGUI.indentLevel = indent;
      EditorGUI.EndProperty();
    }
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
      // Create property container element.
      var container = new VisualElement();

      // Create property fields.
      SerializedProperty listProperty = property.FindPropertyRelative("_list");

      var propertyField = new PropertyField(property.FindPropertyRelative("_list"), property.displayName);

      // Add fields to the container.
      container.Add(propertyField);

      if (CanCheckDuplicate(listProperty) && HasDuplicateKeys(listProperty))
      {
        Label warning = new Label("Warning: Duplicate keys detected in the list.");

        if (ColorUtility.TryParseHtmlString("#dc2626", out Color bg))
        {
          warning.style.backgroundColor = bg;
        }
        warning.style.borderTopLeftRadius = 8f;
        warning.style.borderTopRightRadius = 8f;
        warning.style.borderBottomLeftRadius = 8f;
        warning.style.borderBottomRightRadius = 8f;
        warning.style.paddingTop = 8f;
        warning.style.paddingBottom = 8f;
        warning.style.paddingLeft = 8f;
        warning.style.paddingRight = 8f;
        warning.style.marginTop = 8f;
        warning.style.marginBottom = 8f;
        warning.style.unityFontStyleAndWeight = FontStyle.Bold;

        container.Add(warning);
      }

      return container;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
      SerializedProperty listProperty = property.FindPropertyRelative("_list");

      float height = EditorGUI.GetPropertyHeight(listProperty, true);

      if (CanCheckDuplicate(listProperty) && HasDuplicateKeys(listProperty))
      {
        height += EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing; // Warning box height and spacing
      }

      // Return the height of the list property field
      return height;
    }

    public bool CanCheckDuplicate(SerializedProperty listProperty)
    {
      if (listProperty.isArray && listProperty.arraySize > 0)
      {
        SerializedProperty firstElement = listProperty.GetArrayElementAtIndex(0);
        SerializedProperty keyProperty = firstElement.FindPropertyRelative("Key");

        object keyValue = GetValueFromProperty(keyProperty); // Method to get the value of the property

        return keyValue != null;
      }

      return false;
    }

    public bool HasDuplicateKeys(SerializedProperty listProperty)
    {
      HashSet<object> keys = new HashSet<object>();

      for (int i = 0; i < listProperty.arraySize; i++)
      {
        SerializedProperty element = listProperty.GetArrayElementAtIndex(i);

        SerializedProperty keyProperty = element.FindPropertyRelative("Key");

        if (keyProperty != null)
        {
          object keyValue = GetValueFromProperty(keyProperty); // Method to get the value of the property

          // If the key is already in the set, it's a duplicate
          if (!keys.Add(keyValue))
          {
            return true; // Duplicate found
          }
        }
      }

      return false; // No duplicates
    }

    private object GetValueFromProperty(SerializedProperty property)
    {
      switch (property.propertyType)
      {
        case SerializedPropertyType.Integer:
          return property.intValue;
        case SerializedPropertyType.Float:
          return property.floatValue;
        case SerializedPropertyType.String:
          return property.stringValue;
        case SerializedPropertyType.Boolean:
          return property.boolValue;
        case SerializedPropertyType.Enum:
          return property.enumValueIndex;
        case SerializedPropertyType.ObjectReference:
          return property.objectReferenceValue;
        case SerializedPropertyType.Color:
          return property.colorValue;
        case SerializedPropertyType.LayerMask:
          return property.intValue; // Layer masks are stored as integers
        case SerializedPropertyType.Vector2:
          return property.vector2Value;
        case SerializedPropertyType.Vector3:
          return property.vector3Value;
        case SerializedPropertyType.Vector4:
          return property.vector4Value;
        case SerializedPropertyType.Rect:
          return property.rectValue;
        case SerializedPropertyType.ArraySize:
          return property.intValue;
        case SerializedPropertyType.Character:
          return property.intValue; // Character is stored as an integer (ASCII value)
        case SerializedPropertyType.AnimationCurve:
          return property.animationCurveValue;
        case SerializedPropertyType.Bounds:
          return property.boundsValue;
        case SerializedPropertyType.Quaternion:
          return property.quaternionValue;
        case SerializedPropertyType.ExposedReference:
          return property.exposedReferenceValue;
        case SerializedPropertyType.FixedBufferSize:
          return property.fixedBufferSize;
        case SerializedPropertyType.Vector2Int:
          return property.vector2IntValue;
        case SerializedPropertyType.Vector3Int:
          return property.vector3IntValue;
        case SerializedPropertyType.RectInt:
          return property.rectIntValue;
        case SerializedPropertyType.BoundsInt:
          return property.boundsIntValue;
        case SerializedPropertyType.Hash128:
          return property.hash128Value;
        default:
          return null;
      }
    }
  }
}
#endif