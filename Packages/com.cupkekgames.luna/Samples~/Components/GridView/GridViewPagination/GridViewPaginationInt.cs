using System;
using System.Collections.Generic;
using CupkekGames.Luna;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Systems;

namespace CupkekGames.Luna.Demo.Components
{
  public class GridViewPaginationInt : GridViewPagination<int>
  {
    public GridViewPaginationInt(
      List<int> itemList,
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
        itemList,
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
      GridViewIntSlotController controller = new GridViewIntSlotController(itemSlot);
      
      itemSlot.userData = controller;
    }
    protected override void BindItem(VisualElement slot, int item, int slotIndex, bool selected)
    {
      GridViewIntSlotController slotController = (GridViewIntSlotController)slot.userData;

      slotController.BindItem(item, slotIndex);
    }

    protected override void UnbindItem(VisualElement slot)
    {
      GridViewIntSlotController slotController = (GridViewIntSlotController)slot.userData;

      slotController.UnbindItem();
    }
  }
}