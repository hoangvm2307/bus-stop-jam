using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna;
using CupkekGames.Luna.Library;
using System;
using System.Collections.Generic;
using CupkekGames.Systems;
using CupkekGames.InventorySystem;

namespace CupkekGames.Luna.Mobile
{
  public class MobileInventoryViewList : InventoryViewList
  {
    public MobileInventoryViewList(
      InventoryItemDatabase itemDatabase,
      InventoryBase inventory,
      VisualTreeAsset itemSlotTemplate,
      GameObject parent,
      VisualElement parentElement,
      ListView slotContainer,
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

    public override List<InventoryItem> GetFilteredList(List<int> filters)
    {
      if (_itemDatabase == null)
      {
        _itemDatabase = ServiceLocator.Get<InventoryItemDatabase>();
      }

      List<InventoryItem> result = base.GetFilteredList(filters);

      // Sort by equipment tier, then by equipment type
      result.Sort((a, b) =>
      {
        if (a == null && b == null) return 0;
        if (a == null) return 1;
        if (b == null) return -1;
        // Only sort Equipment by tier, then by type, others go last
        var aEquipment = _itemDatabase.GetItemDefinition(a.ItemType, a.Key) as EquipmentDefinition;
        var bEquipment = _itemDatabase.GetItemDefinition(b.ItemType, b.Key) as EquipmentDefinition;
        if (aEquipment != null && bEquipment != null)
        {
          int tierCompare = bEquipment.Tier.CompareTo(aEquipment.Tier);
          if (tierCompare != 0)
            return tierCompare;
          // If same tier, sort by EquipmentType
          return aEquipment.EquipmentType.CompareTo(bEquipment.EquipmentType);
        }
        if (aEquipment != null) return -1; // Equipment comes before non-equipment
        if (bEquipment != null) return 1;
        return 0;
      });

      return result;
    }
  }
}