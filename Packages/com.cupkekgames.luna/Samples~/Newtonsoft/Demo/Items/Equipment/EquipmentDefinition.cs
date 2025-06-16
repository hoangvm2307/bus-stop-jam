using System;
using CupkekGames.InventorySystem;

namespace CupkekGames.Luna.Demo.Newtonsoft
{
    [Serializable]
    public class EquipmentDefinition : InventoryItemDefinition
    {
        public int EquipmentType;
        public AttributeData AttributeData;
    }
}