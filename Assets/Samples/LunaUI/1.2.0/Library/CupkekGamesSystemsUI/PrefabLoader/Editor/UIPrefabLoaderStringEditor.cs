#if UNITY_EDITOR
using CupkekGames.Luna;
using CupkekGames.Systems.Editor;
using UnityEditor;

namespace CupkekGames.Systems.UI.Editor
{
    [CustomEditor(typeof(UIPrefabLoaderString), true)]
    public class UIPrefabLoaderStringEditor : PrefabLoaderEditor<string, UIViewComponent>
    {
        public override string GetKeyFromFileName(string name)
        {
            return name;
        }
    }
}
#endif