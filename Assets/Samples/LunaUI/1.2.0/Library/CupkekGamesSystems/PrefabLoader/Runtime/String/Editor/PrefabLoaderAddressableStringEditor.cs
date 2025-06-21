#if UNITY_ADDRESSABLES && UNITY_EDITOR
using UnityEditor;
using UnityEngine.AddressableAssets;

namespace CupkekGames.Systems.Editor
{
    [CustomEditor(typeof(PrefabLoaderAddressableString), true)]
    public class PrefabLoaderAddressableStringEditor : PrefabLoaderAddressableEditor<string, AssetReference>
    {
        public override string GetKeyFromFileName(string name)
        {
            return name;
        }

    }
}
#endif