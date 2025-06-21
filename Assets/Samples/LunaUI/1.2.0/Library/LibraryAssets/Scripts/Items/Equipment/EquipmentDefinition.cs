using System;
using CupkekGames.InventorySystem;

namespace CupkekGames.Luna.Library
{
    [Serializable]
    public class EquipmentDefinition : InventoryItemDefinition
    {
        public EquipmentType EquipmentType;
        public InventoryItemTier Tier;
        public AttributeData AttributeData;
    }
}