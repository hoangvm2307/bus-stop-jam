#if UNITY_EDITOR
using UnityEditor;
using CupkekGames.Core.Editor;

namespace CupkekGames.Luna.Demo.Newtonsoft.Editor
{
    [CustomEditor(typeof(EquipmentDatabase), true)]
    public class EquipmentDatabaseEditor : KeyValueDatabaseMonoSOEditor<string, EquipmentDefinitionSO>
    {
        public override string GetKeyFromFileName(string name)
        {
            return name;
        }
    }
}
#endif
