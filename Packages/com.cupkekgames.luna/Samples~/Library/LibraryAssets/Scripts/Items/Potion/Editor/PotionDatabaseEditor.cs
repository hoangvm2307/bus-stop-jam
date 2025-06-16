#if UNITY_EDITOR
using UnityEditor;
using CupkekGames.Core.Editor;

namespace CupkekGames.Luna.Library.Editor
{
    [CustomEditor(typeof(PotionDatabase), true)]
    public class PotionDatabaseEditor : KeyValueDatabaseMonoSOEditor<string, PotionDefinitionSO>
    {
        public override string GetKeyFromFileName(string name)
        {
            return name;
        }
    }
}
#endif
