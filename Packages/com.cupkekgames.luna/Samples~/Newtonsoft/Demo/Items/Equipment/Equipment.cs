using System;
using System.Collections.Generic;
using UnityEngine.UIElements;
using CupkekGames.InventorySystem;

namespace CupkekGames.Luna.Demo.Newtonsoft
{
    public class Equipment : InventoryItem
    {
        public Equipment() : base()
        {
            // Empty constructor for serializer
            // Don't use this constructor
        }
        public Equipment(InventoryItemReference reference) : base(reference)
        {
        }
        public Equipment(string key) : base(0, key)
        {
        }
        public Equipment(string key, Guid id) : base(0, key, id)
        {
        }

        public override InventoryItem Clone(bool sameID)
        {
            if (sameID)
            {
                return new Equipment(Key, ID)
                {
                    Amount = Amount
                };
            }

            return new Equipment(Key)
            {
                Amount = Amount
            };
        }

        public override int MaxStackAmount()
        {
            return 1;
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

            return definition.AttributeData;
        }
    }
}