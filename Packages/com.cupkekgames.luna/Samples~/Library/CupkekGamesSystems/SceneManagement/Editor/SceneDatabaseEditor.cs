#if UNITY_EDITOR && UNITY_ADDRESSABLES
using UnityEditor;
using CupkekGames.Core.Editor;

namespace CupkekGames.Systems.Editor
{
    [CustomEditor(typeof(SceneDatabase))]
    public class SceneDatabaseEditor : KeyValueDatabaseMonoSOEditor<string, SceneSO>
    {
        public override string GetKeyFromFileName(string name)
        {
            return name;
        }
    }
}
#endif
