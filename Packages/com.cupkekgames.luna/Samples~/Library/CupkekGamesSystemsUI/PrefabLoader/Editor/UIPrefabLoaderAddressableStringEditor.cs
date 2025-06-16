
#if UNITY_ADDRESSABLES && UNITY_EDITOR
using CupkekGames.Luna;
using CupkekGames.Systems.Editor;
using UnityEditor;

namespace CupkekGames.Systems.UI.Editor
{
    [CustomEditor(typeof(UIPrefabLoaderAddressableString), true)]
    public class UIPrefabLoaderAddressableStringEditor : PrefabLoaderAddressableEditor<string, UIViewComponent>
    {
        public override string GetKeyFromFileName(string name)
        {
            return name;
        }
    }
}
#endif