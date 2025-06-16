using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna;

namespace CupkekGames.InventorySystem
{
  public class InventoryViewList : GridViewList<InventoryItem>
  {
    // References
    protected InventoryItemDatabase _itemDatabase;
    // Fields
    protected InventoryBase _inventory;
    protected ICollection<string> _attributes;
    protected Sprite _emptyIcon;
    protected TooltipManipulator _emptyTooltipManipulator;
    protected Func<InventoryItem, InventoryItemDefinition, AttributeData> _getAttributeComparison;
    // Properties
    protected override List<InventoryItem> _itemList
    {
      get
      {
        if (_inventory == null)
        {
          return new List<InventoryItem>();
        }

        return _inventory.Items;
      }
      set
      {

      }
    }
    // Tooltip
    protected TooltipPosition _tooltipPosition;
    protected Func<InventoryItem, InventoryItemDefinition, Func<List<TooltipContainerSetup>>> _getTooltipSetups;
    // Drag & Drop
    protected List<VisualElement> _dropSlots;
    protected Func<InventoryItem, List<VisualElement>> _getDropSlots;
    protected Action<int, int, VisualElement, InventoryItemSlotController> _onDrop;
    protected VisualElement _dragArea;
    protected bool _allowDropInventory = false;
    // Events
    public event Action<InventoryItemSlotController> OnItemSlotClick;

    public InventoryViewList(
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
        null,
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
      _itemDatabase = itemDatabase;
      _inventory = inventory;
    }

    public void SetAttributes(ICollection<string> attributes)
    {
      _attributes = attributes;
    }
    public void SetEmptyIcon(Sprite emptyIcon)
    {
      _emptyIcon = emptyIcon;
    }
    public void SetEmptyTooltip(TooltipManipulator tooltipManipulator)
    {
      _emptyTooltipManipulator = tooltipManipulator;
    }
    public void SetTooltipPosition(TooltipPosition tooltipPosition)
    {
      _tooltipPosition = tooltipPosition;
    }
    public void SetDragAndDrop(
      List<VisualElement> dropSlots,
      Func<InventoryItem, List<VisualElement>> getDropSlots,
      Action<int, int, VisualElement, InventoryItemSlotController> onDrop,
      VisualElement dragArea,
      bool allowDropInventory
    )
    {
      _dropSlots = dropSlots;
      _getDropSlots = getDropSlots;
      _onDrop = onDrop;
      _dragArea = dragArea;
      _allowDropInventory = allowDropInventory;
    }

    public void SetGetTooltipSetups(Func<InventoryItem, InventoryItemDefinition, Func<List<TooltipContainerSetup>>> getTooltipSetups)
    {
      _getTooltipSetups = getTooltipSetups;
    }

    public void SetGetAttributeComparison(Func<InventoryItem, InventoryItemDefinition, AttributeData> getAttributeComparison)
    {
      _getAttributeComparison = getAttributeComparison;
    }

    protected void OnItemSlotClickInner(InventoryItemSlotController controller)
    {
      OnItemSlotClick?.Invoke(controller);
    }
    protected override void OnItemCreate(VisualElement itemSlot)
    {
      InventoryItemSlotController controller = new InventoryItemSlotController(_itemDatabase, Parent, itemSlot, _tooltipController, _tooltipPosition);
      controller.SetAttributeNames(_attributes);
      controller.SetEmptyIcon(_emptyIcon);
      controller.SetEmptyTooltip(_emptyTooltipManipulator);
      itemSlot.userData = controller;

      controller.OnClick += OnItemSlotClickInner;
    }
    protected override void BindItem(VisualElement slot, InventoryItem item, int slotIndex, bool selected)
    {
      InventoryItemSlotController slotController = (InventoryItemSlotController)slot.userData;

      InventoryItemDefinition itemDefinition = _itemDatabase.GetItemDefinition(item);

      var itemDragAndDrop = CreateDragAndDrop(slotIndex, slotController, item);

      Func<List<TooltipContainerSetup>> getSetups = null;
      if (_getTooltipSetups != null)
      {
        getSetups = _getTooltipSetups(item, itemDefinition);
      }

      slotController.BindItem(item, itemDefinition, slotIndex, selected, itemDragAndDrop, _getAttributeComparison, getSetups);
    }

    protected override void UnbindItem(VisualElement slot)
    {
      InventoryItemSlotController slotController = (InventoryItemSlotController)slot.userData;

      slotController.UnbindItem();
    }

    protected virtual ItemDragAndDrop CreateDragAndDrop(int slotIndex, InventoryItemSlotController slotController, InventoryItem inventoryItem)
    {
      List<VisualElement> dropSlots = GetDropSlots(inventoryItem);
      if (dropSlots.Count == 0)
      {
        return null;
      }

      InventoryItemSlotController dragSlotController = new InventoryItemSlotController(_itemDatabase, Parent, null, null, _tooltipPosition);

      var itemDragAndDrop = new ItemDragAndDrop(LunaUIManager, _dragArea, slotIndex, dropSlots, null, _itemSlotTemplate, slotController, dragSlotController);
      itemDragAndDrop.OnItemDrop += _onDrop;

      return itemDragAndDrop;
    }

    protected List<VisualElement> GetDropSlots(InventoryItem inventoryItem)
    {
      List<VisualElement> dropSlots = new List<VisualElement>();
      if (_dropSlots != null)
      {
        dropSlots.AddRange(_dropSlots);
      }
      if (_getDropSlots != null)
      {
        dropSlots.AddRange(_getDropSlots(inventoryItem));
      }
      if (_allowDropInventory)
      {
        dropSlots.AddRange(ItemSlots);
      }

      return dropSlots;
    }
  }
}