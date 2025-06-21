using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using CupkekGames.InventorySystem;

namespace CupkekGames.Luna.Library
{
    public class Potion : InventoryItem
    {
        public Potion() : base()
        {
            // Empty constructor for serializer
            // Don't use this constructor
        }
        public Potion(InventoryItemReference reference) : base(reference)
        {
        }
        public Potion(int itemType, string key) : base(itemType, key)
        {
        }
        public Potion(int itemType, string key, Guid id) : base(itemType, key, id)
        {
        }

        public override InventoryItem Clone(bool sameID)
        {
            if (sameID)
            {
                return new Potion(ItemType, Key, ID)
                {
                    Amount = Amount
                };
            }

            return new Potion(ItemType, Key)
            {
                Amount = Amount
            };
        }

        public override int MaxStackAmount()
        {
            return 10;
        }

        public override VisualElement TooltipBottom(
            InventoryItemDatabase itemDatabase,
            ICollection<string> attributeNames,
            Func<InventoryItem, InventoryItemDefinition, AttributeData> getComparison
        )
        {
            return new VisualElement();
        }
    }
}