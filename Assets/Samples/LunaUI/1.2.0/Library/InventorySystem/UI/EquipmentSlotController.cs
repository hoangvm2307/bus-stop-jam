using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna;

namespace CupkekGames.InventorySystem
{
    public class EquipmentSlotController : InventoryItemSlotController
    {
        protected Func<InventoryItem, InventoryItemDefinition, bool> _condition;
        public Func<InventoryItem, InventoryItemDefinition, bool> Condition => _condition;
        public EquipmentSlotController(InventoryItemDatabase itemDatabase, GameObject owner, VisualElement parent,
            TooltipController tooltipController, TooltipPosition tooltipPosition, Func<InventoryItem, InventoryItemDefinition, bool> condition) :
            base(itemDatabase, owner, parent, tooltipController, tooltipPosition)
        {
            _condition = condition;
        }

        public override bool BindItem(InventoryItem item, InventoryItemDefinition itemDefinition, int index, bool selected,
            ItemDragAndDrop dragAndDrop, Func<InventoryItem, InventoryItemDefinition, AttributeData> getComparison, Func<List<TooltipContainerSetup>> getSetups)
        {
            if (_condition != null)
            {
                if (!_condition(item, itemDefinition))
                {
                    return false;
                }
            }

            base.BindItem(item, itemDefinition, index, selected, dragAndDrop, getComparison, getSetups);

            return true;
        }
    }
}