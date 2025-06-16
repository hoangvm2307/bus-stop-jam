using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_INPUT
using CupkekGames.Core;
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Luna
{
  public abstract class GridViewPagination<TItem> : GridViewBase<TItem>
  {
    // Settings
    private int _lineCount = 3;
    // State
    private PaginationController<TItem> _pagination;
    public PaginationController<TItem> Pagination => _pagination;
    public GridViewPagination(
      List<TItem> itemList,
      VisualTreeAsset itemSlotTemplate,
      GameObject parent,
      VisualElement parentElement,
      VisualElement slotContainer,
      TooltipController tooltipController = null,
      UIStartVisibility startVisibility = UIStartVisibility.Visible,
      VisualElement focusElement = null,
      float fadeDuration = 0.5F,
      EasingMode easingMode = EasingMode.EaseOutCirc,
      bool disableOtherViewsOnFadeIn = false,
      bool debug = false) :
      base(itemList, 
      itemSlotTemplate, 
      parent, 
      parentElement, 
      slotContainer, 
      tooltipController, 
      startVisibility, 
      focusElement, 
      fadeDuration, 
      easingMode, 
      disableOtherViewsOnFadeIn, 
      debug)
    {
      List<TItem> list = GetFilteredList(_currentFilters);

      _pagination = new PaginationController<TItem>(list, _lineCount * _slotPerLine, null,
        UIColorName.BASE, UIColorName.PRIMARY, 5);

      _pagination.OnPageChange += OnPageChange;
    }
    protected override void DynamicItemPerLineUpdate(GeometryChangedEvent evt)
    {
      int slotPerLineOld = _slotPerLine;
      SetDynamicItemPerLine(_lineCount, _itemWidth);
      if (slotPerLineOld != _slotPerLine)
      {
        CreateSlots();
      }
    }
    public void SetPagination(int lineCount, int perLine)
    {
      _lineCount = lineCount;
      _slotPerLine = perLine;

      _pagination.ItemsPerPage = _lineCount * _slotPerLine;
    }
    public void SetDynamicItemPerLine(int lineCount, int itemWidth)
    {
      _lineCount = lineCount;
      _itemWidth = itemWidth;
      float width = _slotContainer.worldBound.width;
      
      _slotPerLine = (int)(width / itemWidth);

      _pagination.ItemsPerPage = _lineCount * _slotPerLine;
    }
    public void SetPaginationUI(VisualElement parent, UIColorName normal, UIColorName active, int maxButtonAmount)
    {
      _pagination.SetUI(parent, normal, active, maxButtonAmount);
    }

    public override void CreateSlots()
    {
      FilterList();
      
      _slotContainer.Clear();
      ItemSlots.Clear();

      for (int i = 0; i < _lineCount; i++)
      {
        _slotContainer.Add(MakeLine());
      }

      // Unlike list view, pagination need to refresh items when create slots
      RefreshItems();
    }
    private void OnPageChange(int page)
    {
      List<TItem> currentPage = _pagination.GetCurrentPageElements();

      RenderPage(currentPage);
    }
    private void RenderPage(List<TItem> currentPage)
    {
      if (_tooltipController != null)
      {
        _tooltipController.Tooltip.CloseAll();
      }

      for (int slotIndex = 0; slotIndex < ItemSlots.Count; slotIndex++)
      {
        VisualElement itemSlot = ItemSlots[slotIndex];

        UnbindItem(itemSlot);

        if (slotIndex < currentPage.Count)
        {
          TItem item = currentPage[slotIndex];

          BindItem(itemSlot, item, slotIndex, _selectedItems.Contains(item));

          itemSlot.style.visibility = Visibility.Visible;
        }
        else if (_hideEmpty)
        {
          itemSlot.style.visibility = Visibility.Hidden;
        }
      }
    }

    protected override void OnFilterChange(int filterIndex)
    {
      _pagination.GoToPage(0);
      _pagination.UpdateUI();
    }

    protected override void RefreshItems()
    {
      _pagination.GoToPage(_pagination.CurrentPage);
    }
    protected override void FilterList()
    {
      _pagination.Data = GetFilteredList(_currentFilters);
    }

    public override void Select(int slotIndex)
    {
      if (slotIndex >= ItemSlots.Count)
      {
        return;
      }

      List<TItem> currentPage = _pagination.GetCurrentPageElements();

      VisualElement itemSlot = ItemSlots[slotIndex];

      UnbindItem(itemSlot);

      if (slotIndex < currentPage.Count)
      {
        TItem item = currentPage[slotIndex];

        _selectedItems.Add(item);

        BindItem(itemSlot, item, slotIndex, true);

        itemSlot.style.visibility = Visibility.Visible;
      }
      else if (_hideEmpty)
      {
        itemSlot.style.visibility = Visibility.Hidden;
      }
    }

    public override void Deselect(int slotIndex)
    {
      List<TItem> currentPage = _pagination.GetCurrentPageElements();

      if (slotIndex >= currentPage.Count)
      {
        return;
      }

      TItem item = currentPage[slotIndex];

      if (_selectedItems.Remove(item))
      {
        VisualElement itemSlot = ItemSlots[slotIndex];

        UnbindItem(itemSlot);

        if (slotIndex < currentPage.Count)
        {
          BindItem(itemSlot, item, slotIndex, false);

          itemSlot.style.visibility = Visibility.Visible;
        }
        else if (_hideEmpty)
        {
          itemSlot.style.visibility = Visibility.Hidden;
        }
      }
    }
  }
}