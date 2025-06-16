

using System;

namespace CupkekGames.InventorySystem
{
    [Serializable]
    public class InventoryItemReference
    {
        public int ItemType;
        public string Key;
        public int Amount;
        public InventoryItemReference(int itemType, string key, int amount)
        {
            ItemType = itemType;
            Key = key;
            Amount = amount;
        }
    }
}