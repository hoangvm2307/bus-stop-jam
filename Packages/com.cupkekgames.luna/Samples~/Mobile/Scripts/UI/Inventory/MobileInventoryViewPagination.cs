using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna;
using CupkekGames.InventorySystem;
using System.Collections.Generic;

namespace CupkekGames.Luna.Mobile
{
  public class MobileInventoryViewPagination : InventoryViewPagination
  {
    public MobileInventoryViewPagination(
      InventoryItemDatabase itemDatabase,
      InventoryBase inventory,
      VisualTreeAsset itemSlotTemplate,
      GameObject parent,
      VisualElement parentElement,
      VisualElement slotContainer,
      TooltipController tooltipController,
      UIStartVisibility startVisibility = UIStartVisibility.Visible,
      VisualElement focusElement = null,
      float fadeDuration = 0.5F,
      EasingMode easingMode = EasingMode.EaseOutCirc,
      bool debug = false) :
      base(
        itemDatabase,
        inventory,
        itemSlotTemplate,
        parent,
        parentElement,
        slotContainer,
        tooltipController,
        startVisibility,
        focusElement,
        fadeDuration,
        easingMode,
        debug)
    {

    }
    protected override void OnItemCreate(VisualElement itemSlot)
    {
      InventoryItemSlotController controller = new MobileInventoryItemSlotController(_itemDatabase, Parent, itemSlot, _tooltipController, _tooltipPosition);
      controller.SetAttributeNames(_attributes);
      controller.SetEmptyIcon(_emptyIcon);
      controller.SetEmptyTooltip(_emptyTooltipManipulator);
      itemSlot.userData = controller;

      controller.OnClick += OnItemSlotClickInner;
    }
    protected override ItemDragAndDrop CreateDragAndDrop(int slotIndex, InventoryItemSlotController slotController, InventoryItem item)
    {
      List<VisualElement> dropSlots = GetDropSlots(item);
      if (dropSlots.Count == 0)
      {
        return null;
      }

      InventoryItemSlotController dragSlotController = new MobileInventoryItemSlotController(_itemDatabase, Parent, null, null, _tooltipPosition);

      var itemDragAndDrop = new ItemDragAndDrop(LunaUIManager, _dragArea, slotIndex, dropSlots, null, _itemSlotTemplate, slotController, dragSlotController);
      itemDragAndDrop.OnItemDrop += _onDrop;

      itemDragAndDrop.SetDropSlotClasses(new List<string> { "highlight" });

      return itemDragAndDrop;
    }
  }
}