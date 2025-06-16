using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.InventorySystem
{
    [Serializable]
    public abstract class InventoryItem
    {
        public int ItemType { get; protected set; }
        public string Key { get; protected set; }
        public Guid  ID { get; protected set; } = Guid.NewGuid();
        public int Amount { get; protected set; } = 1;
        public InventoryItem()
        {
            // Empty constructor for serializer
            // Don't use this constructor
        }
        public InventoryItem(InventoryItemReference reference)
        {
            ItemType = reference.ItemType;
            Key = reference.Key;
            ID = Guid.NewGuid();
            SetAmount(reference.Amount);
        }
        public InventoryItem(int itemType, string key)
        {
            ItemType = itemType;
            Key = key;
            ID = Guid.NewGuid();
        }
        public InventoryItem(int itemType, string key, Guid id)
        {
            ItemType = itemType;
            Key = key;
            ID = id;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            InventoryItem other = (InventoryItem)obj;
            return ID == other.ID;
        }
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        public virtual bool CanStackWith(InventoryItem other)
        {
            if (other == null)
            {
                return false;
            }

            return ItemType == other.ItemType && Key == other.Key;
        }

        public int AddAmount(int add)
        {
            int max = MaxStackAmount();

            int available = max - Amount;

            if (add > 0)
            {
                if (add <= available)
                {
                    Amount += add;

                    return 0;
                }

                Amount = max;

                return add - available;
            }
            else if (add < 0)
            {
                int removable = Math.Min(-add, Amount); // Ensure we don't go below zero

                Amount -= removable;

                return add + removable; // Return the remaining negative amount, if any
            }

            // If add == 0, do nothing
            return 0;
        }


        public int AvailableStackSpace()
        {
            return MaxStackAmount() - Amount;
        }

        public void SetAmount(int amount)
        {
            Amount = Mathf.Clamp(amount, 0, MaxStackAmount());
        }

        public abstract InventoryItem Clone(bool sameID);
        public abstract int MaxStackAmount();
        public abstract VisualElement TooltipBottom(
            InventoryItemDatabase itemDatabase,
            ICollection<string> attributeNames,
            Func<InventoryItem, InventoryItemDefinition, AttributeData> getComparison
        );
        public virtual string DisplayName(InventoryItemDefinition itemDefinition)
        {
            return itemDefinition.Name;
        }
    }
}