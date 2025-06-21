using UnityEngine;

namespace CupkekGames.InventorySystem
{
    public abstract class InventoryItemDatabase : MonoBehaviour
    {
        public InventoryItemDefinition GetItemDefinition(InventoryItemReference item)
        {
            return GetItemDefinition(item.ItemType, item.Key);
        }
        public InventoryItemDefinition GetItemDefinition(InventoryItem item)
        {
            return GetItemDefinition(item.ItemType, item.Key);
        }
        public abstract InventoryItemDefinition GetItemDefinition(int itemType, string key);
        public abstract InventoryItem CreateItem(InventoryItemReference itemReference);
    }
}