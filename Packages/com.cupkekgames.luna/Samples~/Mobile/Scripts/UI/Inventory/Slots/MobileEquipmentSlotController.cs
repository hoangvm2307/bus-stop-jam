using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna;
using CupkekGames.InventorySystem;

namespace CupkekGames.Luna.Mobile
{
    public class MobileEquipmentSlotController : EquipmentSlotController, IInventoryItemSlotWithStars
    {
        private List<VisualElement> _stars;
        public MobileEquipmentSlotController(InventoryItemDatabase itemDatabase, GameObject owner, VisualElement parent,
            TooltipController tooltipController, TooltipPosition tooltipPosition, Func<InventoryItem, InventoryItemDefinition, bool> condition) :
            base(itemDatabase, owner, parent, tooltipController, tooltipPosition, condition)
        {
            
        }

        public override void ResetVisualElements(VisualElement parent)
        {
            base.ResetVisualElements(parent);

            if (parent == null)
            {
                _stars = new();
                return;
            }

            _stars = parent.Query<VisualElement>("Star").ToList();
            HideStars();
        }

        public override bool BindItem(InventoryItem item, InventoryItemDefinition itemDefinition, int index, bool selected,
            ItemDragAndDrop dragAndDrop, Func<InventoryItem, InventoryItemDefinition, AttributeData> getComparison, Func<List<TooltipContainerSetup>> getSetups)
        {
            bool result = base.BindItem(item, itemDefinition, index, selected, dragAndDrop, getComparison, getSetups);

            if (result)
            {
                MobileInventoryItemSlotController.MobileUpdateItemDisplay(this, Item, ItemDefinition);
            }

            return result;
        }

        public override void UnbindItem()
        {
            base.UnbindItem();

            MobileInventoryItemSlotController.MobileClearItemDisplay(this);
        }

        public void HideStars()
        {
            if (_stars == null)
            {
                return;
            }

            foreach (var star in _stars)
            {
                star.style.visibility = Visibility.Hidden;
            }
        }

        public void ShowStars(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                if (i >= _stars.Count)
                {
                    break;
                }
                _stars[i].style.visibility = Visibility.Visible;
            }
        }
    }
}