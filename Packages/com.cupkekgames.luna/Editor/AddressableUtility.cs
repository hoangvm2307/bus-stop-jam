#if UNITY_EDITOR && UNITY_ADDRESSABLES
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CupkekGames.Luna.Editor
{
    public static class AddressableUtility
    {
        public static bool IsAddressableSettingsCreated()
        {
            return AddressableAssetSettingsDefaultObject.Settings != null;
        }
        public static AssetReference AddAssetToAddressables(string assetGUID)
        {
            if (AddressableAssetSettingsDefaultObject.Settings == null)
            {
                return null;
            }

            return AddressableAssetSettingsDefaultObject.Settings.CreateAssetReference(assetGUID);
        }
        public static AssetReference AddAssetToAddressables(UnityEngine.Object asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);

            return AddAssetToAddressables(assetGUID);
        }

        public static bool IsAssetAddressable(string assetGUID)
        {
            if (AddressableAssetSettingsDefaultObject.Settings == null)
            {
                return false;
            }

            return AddressableAssetSettingsDefaultObject.Settings.FindAssetEntry(assetGUID) != null;
        }
        public static bool IsAssetAddressable(UnityEngine.Object asset)
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);

            return IsAssetAddressable(assetGUID);
        }

        public static bool RemoveAssetFromAddressables(string assetGUID)
        {
            if (AddressableAssetSettingsDefaultObject.Settings == null)
            {
                return false;
            }

            var settings = AddressableAssetSettingsDefaultObject.Settings;

            var entry = settings.FindAssetEntry(assetGUID);
            if (entry != null)
            {
                settings.RemoveAssetEntry(assetGUID);
                return true;
            }
            else
            {
                Debug.LogWarning($"Asset with GUID {assetGUID} not found in Addressables.");
                return false;
            }
        }

        public static bool AreScenesAddressable(string[] partialPathsToAdd, string searchFolder)
        {
            List<string> sceneGuids = BuildSettingsUtility.GetSceneGUIDs(partialPathsToAdd, searchFolder);

            if (sceneGuids.Count == 0)
            {
                return false;
            }

            foreach (string guid in sceneGuids)
            {
                if (!IsAssetAddressable(guid))
                {
                    return false;
                }
            }

            return true;
        }
        public static void MakeScenesAddressable(string[] partialPathsToAdd, string searchFolder)
        {
            List<string> sceneGuids = BuildSettingsUtility.GetSceneGUIDs(partialPathsToAdd, searchFolder);

            foreach (string guid in sceneGuids)
            {
                AddAssetToAddressables(guid);
            }
        }
        public static void RemoveScenesAddressable(string[] partialPathsToAdd, string searchFolder)
        {
            List<string> sceneGuids = BuildSettingsUtility.GetSceneGUIDs(partialPathsToAdd, searchFolder);

            foreach (string guid in sceneGuids)
            {
                RemoveAssetFromAddressables(guid);
            }
        }
    }
}
#endif