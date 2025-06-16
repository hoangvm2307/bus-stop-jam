#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CupkekGames.Core.Editor
{
    [CustomPropertyDrawer(typeof(FolderReference))]
    public class FolderReferencePropertyDrawer : PropertyDrawer
    {
        private SerializedProperty guid;
        private Object folderObject;

        private void Initialize(SerializedProperty property)
        {
            guid = property.FindPropertyRelative("GUID");
            folderObject = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(guid.stringValue));
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (guid == null || guid.propertyPath != property.FindPropertyRelative("GUID").propertyPath)
            {
                Initialize(property);
            }

            // Now use the full width for the object field
            Rect objectFieldRect = new Rect(position.x, position.y, position.width, position.height);

            // Draw the object field using the default Unity style
            folderObject = EditorGUI.ObjectField(objectFieldRect, label, folderObject, typeof(DefaultAsset), false);

            // Update GUID if the folder object is changed through the ObjectField
            if (folderObject != null)
            {
                string path = AssetDatabase.GetAssetPath(folderObject);
                if (Directory.Exists(path))
                {
                    guid.stringValue = AssetDatabase.AssetPathToGUID(path);
                }
            }

            HandleDragAndDrop(objectFieldRect, property);
        }

        private void HandleDragAndDrop(Rect dropArea, SerializedProperty property)
        {
            Event evt = Event.current;

            if (!dropArea.Contains(evt.mousePosition))
                return;

            switch (evt.type)
            {
                case EventType.DragUpdated:
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        Object draggedObject = DragAndDrop.objectReferences[0];
                        string draggedPath = AssetDatabase.GetAssetPath(draggedObject);
                        DragAndDrop.visualMode = Directory.Exists(draggedPath) ? DragAndDropVisualMode.Copy : DragAndDropVisualMode.Rejected;
                    }
                    evt.Use();
                    break;

                case EventType.DragPerform:
                    if (DragAndDrop.objectReferences.Length > 0)
                    {
                        Object draggedObject = DragAndDrop.objectReferences[0];
                        string draggedPath = AssetDatabase.GetAssetPath(draggedObject);
                        if (Directory.Exists(draggedPath))
                        {
                            folderObject = draggedObject;
                            property.FindPropertyRelative("GUID").stringValue = AssetDatabase.AssetPathToGUID(draggedPath);
                        }
                    }
                    evt.Use();
                    break;
            }
        }
    }
}
#endif
