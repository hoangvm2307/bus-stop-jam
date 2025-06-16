#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace CupkekGames.Luna.Editor
{
    public static class BuildSettingsUtility
    {
        public static bool IsDesiredSceneAtIndex(int index, string desiredScenePath)
        {
            var scenes = EditorBuildSettings.scenes;

            if (index <= scenes.Length)
            {
                return scenes[index].path.Contains(desiredScenePath, System.StringComparison.OrdinalIgnoreCase);
            }

            return false; // No scenes in Build Settings
        }

        /// <summary>
        /// Removes specific scenes from the Build Settings.
        /// </summary>
        /// <param name="scenePathsToRemove">An array of scene paths to remove.</param>
        public static void RemoveScenesFromBuildSettings(string[] scenePathsToRemove)
        {
            var currentScenes = EditorBuildSettings.scenes;
            var updatedScenes = new List<EditorBuildSettingsScene>();

            foreach (var scene in currentScenes)
            {
                bool shouldRemove = false;

                foreach (var partialPath in scenePathsToRemove)
                {
                    if (scene.path.Contains(partialPath, System.StringComparison.OrdinalIgnoreCase))
                    {
                        shouldRemove = true;
                        break;
                    }
                }

                if (!shouldRemove)
                {
                    updatedScenes.Add(scene);
                }
            }

            EditorBuildSettings.scenes = updatedScenes.ToArray();
        }
        /// <summary>
        /// Adds scenes to the Build Settings if their paths contain any part of the specified strings, restricted to a specific folder.
        /// The scenes will be added at the start of the Build Settings, but will not be added if they already exist at the same index.
        /// </summary>
        /// <param name="partialPathsToAdd">An array of partial strings to match scene paths against.</param>
        /// <param name="searchFolder">The folder to restrict the search to (e.g., "Assets/Scenes").</param>
        public static void AddScenesToBuildSettingsInFolderAtStartWithIndexCheck(string[] partialPathsToAdd, string searchFolder)
        {
            var scenesToAdd = GetSceneFullPaths(partialPathsToAdd, searchFolder);

            if (scenesToAdd.Count == 0)
            {
                Debug.LogWarning($"No scenes matching the specified partial paths were found in {searchFolder}.");
                return;
            }

            // Get current Build Settings scenes
            var currentScenes = EditorBuildSettings.scenes.ToList();

            // Add new scenes at the start of the Build Settings, checking if they are already at the same index
            for (int i = 0; i < scenesToAdd.Count; i++)
            {
                string scenePath = scenesToAdd[i];
                bool alreadyExists = false;

                // Check if the scene is already at the same index
                if (i < currentScenes.Count && currentScenes[i].path.Equals(scenePath, System.StringComparison.OrdinalIgnoreCase))
                {
                    alreadyExists = true;
                }

                // If the scene does not already exist at the same index, insert it at the start
                if (!alreadyExists)
                {
                    currentScenes.Insert(i, new EditorBuildSettingsScene(scenePath, true));
                }
            }

            // Update Build Settings
            EditorBuildSettings.scenes = currentScenes.ToArray();
            // Debug.Log($"Added scenes to the start of Build Settings from {searchFolder}: " + string.Join(", ", scenesToAdd));
        }

        public static List<string> GetSceneFullPaths(string[] partialPaths, string searchFolder)
        {
            // Get all scenes in the specified folder
            var allSceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { searchFolder });
            var allScenePaths = allSceneGUIDs
                .Select(AssetDatabase.GUIDToAssetPath)
                .ToArray();

            // Create a list to store scenes to add in the correct order
            var result = new List<string>();

            // Loop through each partial path in the order they are provided
            foreach (var partialPath in partialPaths)
            {
                var matchedScenes = allScenePaths
                    .Where(scenePath => scenePath.Contains(partialPath, System.StringComparison.OrdinalIgnoreCase))
                    .Distinct()
                    .ToArray();

                result.AddRange(matchedScenes); // Add matching scenes for the current partial path
            }

            return result;
        }
        public static List<string> GetSceneGUIDs(string[] partialPaths, string searchFolder)
        {
            // Get all scene GUIDs in the specified folder
            var allSceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { searchFolder });

            // Convert GUIDs to paths for matching purposes
            var guidToPathMap = allSceneGUIDs.ToDictionary(
                guid => guid,
                guid => AssetDatabase.GUIDToAssetPath(guid)
            );

            // Create a list to store matching GUIDs in the correct order
            var result = new List<string>();

            // Loop through each partial path in the order they are provided
            foreach (var partialPath in partialPaths)
            {
                var matchedGUIDs = guidToPathMap
                    .Where(kvp => kvp.Value.Contains(partialPath, System.StringComparison.OrdinalIgnoreCase))
                    .Select(kvp => kvp.Key)
                    .Distinct()
                    .ToArray();

                result.AddRange(matchedGUIDs); // Add matching GUIDs for the current partial path
            }

            return result;
        }
    }
}
#endif