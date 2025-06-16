using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna;
using CupkekGames.Luna.Library;
using CupkekGames.InventorySystem;

namespace CupkekGames.Luna.Mobile
{
    public class MobileInventoryItemSlotController : InventoryItemSlotController, IInventoryItemSlotWithStars
    {
        private List<VisualElement> _stars;
        public MobileInventoryItemSlotController(InventoryItemDatabase itemDatabase, GameObject owner, VisualElement parent, TooltipController tooltipController,
            TooltipPosition tooltipPosition) : base(itemDatabase, owner, parent, tooltipController, tooltipPosition)
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

        public override void UpdateItemDisplay()
        {
            base.UpdateItemDisplay();

            MobileUpdateItemDisplay(this, Item, ItemDefinition);
        }

        public override void ClearItemDisplay()
        {
            base.ClearItemDisplay();

            MobileClearItemDisplay(this);
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

        public static void MobileUpdateItemDisplay(InventoryItemSlotController controller, InventoryItem item, InventoryItemDefinition itemDefinition)
        {
            // Clear
            MobileClearItemDisplay(controller);

            if (item == null)
            {
                return;
            }

            // Add
            if (itemDefinition is EquipmentDefinition equipmentDefinition)
            {
                if (controller.Background != null)
                {
                    controller.Background.AddToClassList(equipmentDefinition.Tier.ToString());
                }
            }

            if (controller is IInventoryItemSlotWithStars withStars)
            {
                if (item is Equipment equipment)
                {
                    withStars.ShowStars(equipment.Level);
                }
            }
        }

        public static void MobileClearItemDisplay(InventoryItemSlotController controller)
        {
            foreach (var tier in Enum.GetValues(typeof(InventoryItemTier)))
            {
                if (controller.Background != null)
                {
                controller.Background.RemoveFromClassList(tier.ToString());
                }
            }

            if (controller is IInventoryItemSlotWithStars withStars)
            {
                withStars.HideStars();
            }
        }
    }
}
