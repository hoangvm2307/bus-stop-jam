using UnityEngine;
using CupkekGames.InventorySystem;
using CupkekGames.Systems;

namespace CupkekGames.Luna.Library
{
    public class InventoryItemDatabaseExample : InventoryItemDatabase
    {
        [SerializeField] private EquipmentDatabase _equipments;
        public EquipmentDatabase Equipments => _equipments;
        [SerializeField] private PotionDatabase _potions;
        public PotionDatabase Potions => _potions;
        private void Awake()
        {
            ServiceLocator.Register(this, typeof(InventoryItemDatabase));
        }
        private void OnDestroy()
        {
            ServiceLocator.Remove(this);
        }
        public override InventoryItemDefinition GetItemDefinition(int itemType, string key)
        {
            return itemType switch
            {
                0 => _equipments.GetValue(key).ItemDefinition,
                1 => _potions.GetValue(key).ItemDefinition,
                _ => null,
            };
        }

        public override InventoryItem CreateItem(InventoryItemReference itemReference)
        {
            return itemReference.ItemType switch
            {
                0 => new Equipment(itemReference),
                1 => new Potion(itemReference),
                _ => null,
            };
        }
    }
}