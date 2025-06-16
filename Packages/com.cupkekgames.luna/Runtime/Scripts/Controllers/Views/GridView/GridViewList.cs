using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;

#if UNITY_INPUT
using CupkekGames.Core;
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Luna
{
  public abstract class GridViewList<TItem> : GridViewBase<TItem>
  {
    public ListView ListView => (ListView)_slotContainer;
    private List<TItem> _filteredItemList;
    public GridViewList(
      List<TItem> itemList,
      VisualTreeAsset itemSlotTemplate,
      GameObject parent,
      VisualElement parentElement,
      ListView slotContainer,
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
      SetupListView();
    }
    protected virtual void SetupListView()
    {
      ListView listView = ListView;

      listView.makeItem = () =>
      {
        return MakeLine();
      };

      listView.bindItem = (line, lineIndex) =>
      {
        int startIndex = lineIndex * _slotPerLine;
        int count = Math.Min(_slotPerLine, _filteredItemList.Count - startIndex);
        
        if (startIndex < 0 || startIndex >= _filteredItemList.Count || count <= 0)
          return;
          
        List<TItem> items = _filteredItemList.GetRange(startIndex, count);

        IEnumerable<VisualElement> children = line.Children();

        int i = 0;
        foreach (VisualElement child in children)
        {
          if (i >= items.Count)
          {
            if (_hideEmpty)
            {
              child.style.visibility = Visibility.Hidden;
            }
            continue;
          }
            
          TItem item = items[i];
          bool selected = _selectedItems.Contains(item);

          BindItem(child, item, i + startIndex, selected);

          child.style.visibility = Visibility.Visible;
          i++;
        }
      };

      listView.unbindItem = (line, lineIndex) =>
      {
        int startIndex = lineIndex * _slotPerLine;
        int count = Math.Min(_slotPerLine, _filteredItemList.Count - startIndex);
        
        if (startIndex < 0 || startIndex >= _filteredItemList.Count || count <= 0)
          return;
          
        List<TItem> items = _filteredItemList.GetRange(startIndex, count);

        IEnumerable<VisualElement> children = line.Children();

        foreach (VisualElement child in children)
        {
          UnbindItem(child);
        }
      };
    }

    protected override void DynamicItemPerLineUpdate(GeometryChangedEvent evt)
    {
      int slotPerLineOld = _slotPerLine;
      SetDynamicItemPerLine(_itemWidth);
      if (slotPerLineOld != _slotPerLine)
      {
        CreateSlots();
      }
    }
    public void SetDynamicItemPerLine(int itemWidth)
    {
      _itemWidth = itemWidth;
      float width = _slotContainer.worldBound.width;
      
      _slotPerLine = (int)(width / itemWidth);
    }
    public void SetSlotPerLine(int perLine)
    {
      _slotPerLine = perLine;
    }

    protected override void OnFilterChange(int filterIndex)
    {
      
    }

    protected override void RefreshItems()
    {
      ListView listView = ListView;
      listView.RefreshItems();
    }
    public override void CreateSlots()
    {
      FilterList();
      ItemSlots.Clear();

      ListView listView = ListView;
      listView.Rebuild();
    }
    protected override void FilterList()
    {
      _filteredItemList = GetFilteredList(_currentFilters);
      
      ListView listView = ListView;

      int lineCount = Mathf.CeilToInt((float)_filteredItemList.Count / _slotPerLine);

      if (lineCount <= 0)
      {
        listView.itemsSource = new List<int>();
        return;
      }

      listView.itemsSource = new List<int>(Enumerable.Range(0, lineCount));
    }

    public override void Select(int slotIndex)
    {
      
    }

    public override void Deselect(int slotIndex)
    {
      
    }


  }
}