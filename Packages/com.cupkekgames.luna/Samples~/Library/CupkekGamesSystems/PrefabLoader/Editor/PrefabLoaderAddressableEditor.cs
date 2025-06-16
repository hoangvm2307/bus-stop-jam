#if UNITY_ADDRESSABLES && UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.UIElements;

namespace CupkekGames.Systems.Editor
{
    public abstract class PrefabLoaderAddressableEditor<TKey, TValue> : UnityEditor.Editor
    {
        bool _buttonEnabled = true;
        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            DrawDefaultInspector();

            // Reference to the target script
            PrefabLoaderAddressable<TKey> prefabLoader = (PrefabLoaderAddressable<TKey>)target;

            // if (prefabLoader.EditorHasDuplicateKeys())
            // {
            //     EditorGUILayout.HelpBox("Duplicate keys, please ensure all keys are unique.", MessageType.Warning);
            // }

            // Add a button to the inspector
            GUIStyle labelStyle = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true
            };

            GUILayout.Label(
                $"Search for Addressable Prefabs with Label containing {typeof(TValue).Name} and add them to the list.\n" +
                "This usually takes less than 5 seconds, but may vary with project size." +
                "You may need to click elsewhere and then click this object again to refresh it.",
                labelStyle
            );

            GUI.enabled = _buttonEnabled;
            string buttonName = _buttonEnabled ? "Find All Addressable Prefabs" : "Searching...";

            if (GUILayout.Button(buttonName))
            {
                _buttonEnabled = false;
                // Call the method when the button is pressed
                string label = serializedObject.FindProperty("_searchLabel").stringValue;

                Debug.Log($"Searching Addressable Prefabs with label {label} containing {typeof(TValue).Name}...");

                FindAddressablePrefabs(prefabLoader, label);
            }

            GUI.enabled = true;

            if (GUILayout.Button("Clear"))
            {
                prefabLoader.EditorClear();

                EditorUtility.SetDirty(prefabLoader);
                Repaint();
            }
        }

        public override VisualElement CreateInspectorGUI()
        {
            // Create a container for the UI elements
            VisualElement container = new VisualElement();

            // Default inspector elements
            InspectorElement.FillDefaultInspector(container, serializedObject, this);

            // Reference to the target script
            PrefabLoaderAddressable<TKey> prefabLoader = (PrefabLoaderAddressable<TKey>)target;

            // Label with word wrapping
            Label searchLabel = new Label(
                $"Search for Addressable Prefabs with Label containing {typeof(TValue).Name} and add them to the list.\n" +
                "This usually takes less than 5 seconds, but may vary with project size." +
                "You may need to click elsewhere and then click this object again to refresh it."
            );
            searchLabel.style.whiteSpace = WhiteSpace.Normal;  // Enable word wrapping
            container.Add(searchLabel);

            // Add the "Find All Addressable Prefabs" button
            Button findButton = new Button(() =>
            {
                _buttonEnabled = false; // Disable the button when searching starts
                string label = serializedObject.FindProperty("_searchLabel").stringValue;

                Debug.Log($"Searching Addressable Prefabs with label {label} containing {typeof(TValue).Name}...");

                FindAddressablePrefabs(prefabLoader, label);
                _buttonEnabled = true; // Enable the button after search is complete
            })
            {
                text = _buttonEnabled ? "Find All Addressable Prefabs" : "Searching...",
                enabledSelf = _buttonEnabled
            };
            findButton.style.flexGrow = 1;
            container.Add(findButton);

            // Add the "Clear" button
            Button clearButton = new Button(() =>
            {
                prefabLoader.EditorClear();
                EditorUtility.SetDirty(prefabLoader);
                serializedObject.ApplyModifiedProperties();  // Save changes to the serialized object
                Debug.Log("Cleared addressable prefab loader.");
            })
            {
                text = "Clear"
            };
            clearButton.style.flexGrow = 1;
            container.Add(clearButton);

            return container;
        }


        private void FindAddressablePrefabs(PrefabLoaderAddressable<TKey> prefabLoader, string label)
        {
            Dictionary<TKey, string> prefabsWithScript = new Dictionary<TKey, string>();

            // Search for all addressable assets with the label "Prefab"
            Addressables.LoadResourceLocationsAsync(label, typeof(GameObject)).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    int handleCount = 0;

                    foreach (IResourceLocation location in handle.Result)
                    {
                        handleCount++;

                        Addressables.LoadAssetAsync<GameObject>(location).Completed += prefabHandle =>
                        {
                            if (prefabHandle.Status == AsyncOperationStatus.Succeeded)
                            {
                                GameObject prefab = prefabHandle.Result;
                                if (prefab != null && prefab.GetComponent<TValue>() != null)
                                {
                                    string guid;
                                    long file;
                                    if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(prefab, out guid, out file))
                                    {
                                        prefabsWithScript.Add(GetKeyFromFileName(prefab.name), guid);
                                    }
                                }
                            }

                            handleCount--;
                            if (handleCount == 0)
                            {
                                AfterLoad(prefabLoader, prefabsWithScript);
                            }
                        };
                    }
                }
                else
                {
                    Debug.LogError("Failed to load addressable locations.");
                }
            };
        }

        private void AfterLoad(PrefabLoaderAddressable<TKey> prefabLoader, Dictionary<TKey, string> prefabsWithScript)
        {
            if (prefabsWithScript.Count > 0)
            {
                Debug.Log($"Found {prefabsWithScript.Count} addressable prefabs with the script {typeof(TValue).Name}:");
                foreach (var kvp in prefabsWithScript)
                {
                    prefabLoader.EditorAdd(kvp.Key, new AssetReference(kvp.Value));
                }

                EditorUtility.SetDirty(prefabLoader);
                Repaint();
            }
            else
            {
                Debug.Log($"No addressable prefabs found with the script {typeof(TValue).Name}.");
            }

            _buttonEnabled = true;
        }

        public abstract TKey GetKeyFromFileName(string name);

    }
}
#endif