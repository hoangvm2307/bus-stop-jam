#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Core.Editor
{
    public abstract class KeyValueDatabaseMonoSOEditor<TKey, TValue> : UnityEditor.Editor where TValue : ScriptableObject
    {
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            // Reference to the target script
            KeyValueDatabaseMonoSO<TKey, TValue> scriptableObjectDatabase = (KeyValueDatabaseMonoSO<TKey, TValue>)target;

            // if (scriptableObjectDatabase.EditorHasDuplicateKeys())
            // {
            //     EditorGUILayout.HelpBox("Duplicate keys, please ensure all keys are unique.", MessageType.Warning);
            // }

            // Add a button to the inspector
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };

            GUILayout.Label(
                $"Search for ScriptableObjects of type {typeof(TValue).Name} in folder and add them to the list.\n" +
                "This usually takes less than 5 seconds, but may vary with project size." +
                "You may need to click elsewhere and then click this object again to refresh it.",
                labelStyle
            );

            if (GUILayout.Button("Find All ScriptableObjects In Folder"))
            {
                // Call the method when the button is pressed
                SerializedProperty folder = serializedObject.FindProperty("_searchFolder");
                string guid = folder.FindPropertyRelative("GUID").stringValue;
                string searchFolder = AssetDatabase.GUIDToAssetPath(guid);

                Debug.Log($"Searching folder {searchFolder}...");

                FindScriptableObjects(scriptableObjectDatabase, searchFolder);
            }

            if (GUILayout.Button("Clear"))
            {
                scriptableObjectDatabase.EditorClear();

                EditorUtility.SetDirty(scriptableObjectDatabase);
                Repaint();
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            // Create a container element
            VisualElement container = new VisualElement();

            // Default inspector elements
            InspectorElement.FillDefaultInspector(container, serializedObject, this);

            // Reference to the target script
            KeyValueDatabaseMonoSO<TKey, TValue> scriptableObjectDatabase = (KeyValueDatabaseMonoSO<TKey, TValue>)target;

            // Display the search label with word wrapping
            Label searchLabel = new Label(
                $"Search for ScriptableObjects of type {typeof(TValue).Name} in folder and add them to the list.\n" +
                "This usually takes less than 5 seconds, but may vary with project size." +
                "You may need to click elsewhere and then click this object again to refresh it."
            );
            searchLabel.style.whiteSpace = WhiteSpace.Normal;  // Enable word wrapping
            container.Add(searchLabel);

            // Add the "Find All ScriptableObjects In Folder" button
            Button findButton = new Button(() =>
            {
                SerializedProperty folder = serializedObject.FindProperty("_searchFolder");
                string guid = folder.FindPropertyRelative("GUID").stringValue;
                string searchFolder = AssetDatabase.GUIDToAssetPath(guid);

                Debug.Log($"Searching folder {searchFolder}...");

                FindScriptableObjects(scriptableObjectDatabase, searchFolder);
            })
            {
                text = "Find All ScriptableObjects In Folder"
            };
            findButton.style.flexGrow = 1;
            container.Add(findButton);

            // Add the "Clear" button
            Button clearButton = new Button(() =>
            {
                scriptableObjectDatabase.EditorClear();
                EditorUtility.SetDirty(scriptableObjectDatabase);
                serializedObject.ApplyModifiedProperties();
                Debug.Log("Cleared scriptable object database.");
            })
            {
                text = "Clear"
            };
            clearButton.style.flexGrow = 1;
            container.Add(clearButton);

            return container;
        }

        private void FindScriptableObjects(KeyValueDatabaseMonoSO<TKey, TValue> scriptableObjectDatabase, string searchFolder)
        {
            List<TValue> scriptableObjects = new List<TValue>();

            // Get all ScriptableObject paths in the project
            string[] scriptableObjectPaths = AssetDatabase.FindAssets($"t:{typeof(TValue).Name}", new[] { searchFolder });

            foreach (string scriptableObjectPath in scriptableObjectPaths)
            {
                string fullPath = AssetDatabase.GUIDToAssetPath(scriptableObjectPath);
                TValue scriptableObject = AssetDatabase.LoadAssetAtPath<TValue>(fullPath);

                if (scriptableObject != null)
                {
                    scriptableObjects.Add(scriptableObject);
                }
            }

            if (scriptableObjects.Count > 0)
            {
                Debug.Log($"Found {scriptableObjects.Count} ScriptableObjects of type {typeof(TValue).Name}:");
                foreach (TValue scriptableObject in scriptableObjects)
                {
                    scriptableObjectDatabase.EditorAdd(GetKeyFromFileName(scriptableObject.name), scriptableObject);
                }

                EditorUtility.SetDirty(scriptableObjectDatabase);
                Repaint();
            }
            else
            {
                Debug.Log($"No ScriptableObjects found of type {typeof(TValue).Name}.");
            }
        }

        public abstract TKey GetKeyFromFileName(string name);
    }
}
#endif
