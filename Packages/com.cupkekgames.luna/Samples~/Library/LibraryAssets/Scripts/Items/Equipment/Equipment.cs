using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using CupkekGames.InventorySystem;

namespace CupkekGames.Luna.Library
{
    public class Equipment : InventoryItem
    {
        public int Level;
        public Equipment() : base()
        {
            // Empty constructor for serializer
            // Don't use this constructor
        }
        public Equipment(InventoryItemReference reference) : base(reference)
        {
        }
        public Equipment(int itemType, string key) : base(itemType, key)
        {
        }
        public Equipment(int itemType, string key, Guid id) : base(itemType, key, id)
        {
        }
        public Equipment(int itemType, string key, int level) : base(itemType, key)
        {
            Level = level;
        }

        public override InventoryItem Clone(bool sameID)
        {
            if (sameID)
            {
                return new Equipment(ItemType, Key, ID)
                {
                    Amount = Amount
                };
            }

            return new Equipment(ItemType, Key)
            {
                Amount = Amount
            };
        }

        public override int MaxStackAmount()
        {
            return 1;
        }
        public override bool CanStackWith(InventoryItem other)
        {
            return false;
        }
        public override VisualElement TooltipBottom(
            InventoryItemDatabase itemDatabase,
            ICollection<string> attributeNames,
            Func<InventoryItem, InventoryItemDefinition, AttributeData> getComparison
        )
        {
            VisualElement bottom = new VisualElement();

            EquipmentDefinition definition = (EquipmentDefinition)itemDatabase.GetItemDefinition(this);

            AttributeData comparison = null;
            if (getComparison != null)
            {
                comparison = getComparison(this, definition);

                // Debug.Log(Key + ": " + (comparison != null));
            }

            new AttributeDataController(attributeNames, bottom, definition.AttributeData, comparison, true, 1, true, false);

            return bottom;
        }

        public AttributeData AttributeData(InventoryItemDatabase itemDatabase)
        {
            EquipmentDefinition definition = (EquipmentDefinition)itemDatabase.GetItemDefinition(this);

            // definition.AttributeData is base stats
            // You can add bonus stats here if needed

            AttributeData attr = new AttributeData();
            attr.Add(definition.AttributeData);
            attr.MultiplyAll(Level + 1);

            return attr;
        }
    }
}