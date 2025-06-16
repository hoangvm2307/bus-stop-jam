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
  public abstract class GridViewBase<TItem> : UIView
  {
    // Fields
    protected List<int> _currentFilters;
    protected int _slotPerLine = 3;
    protected int _itemWidth = 100;
    protected bool _hideEmpty;
    protected UIColorName _selectedFilterColor = UIColorName.PRIMARY;
    protected string _selectedFilterClass = "selected";
    protected string _filterClass = "filter";
    protected string _filterClearClass = "filter-clear";
    // Properties
    protected virtual List<TItem> _itemList { get; set; }

    // UI Elements
    protected VisualTreeAsset _itemSlotTemplate;
    protected VisualElement _slotContainer;
    public VisualElement SlotContainer => _slotContainer;
    public List<VisualElement> ItemSlots = new();
    protected List<List<Button>> _filterButtons = new();
    protected List<List<Button>> _filterClearButtons = new();
    protected List<Func<List<TItem>, int, List<TItem>>> _filters = new();
    protected InputPrompt _filterItemPrevious;
    protected InputPrompt _filterItemNext;
    protected TooltipController _tooltipController;

#if UNITY_INPUT
    protected InputAction _prevAction;
    protected InputAction _nextAction;
#endif

    // State
    protected HashSet<TItem> _selectedItems = new();
    public HashSet<TItem> SelectedItems => _selectedItems;
    public GridViewBase(
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
      base(parent, parentElement, startVisibility, focusElement, fadeDuration, easingMode, disableOtherViewsOnFadeIn, debug)
    {
      _itemList = itemList;
      _itemSlotTemplate = itemSlotTemplate;
      _slotContainer = slotContainer;
      _tooltipController = tooltipController;
    }

    public void RegisterDynamicItemPerLineUpdate()
    {
      _slotContainer.RegisterCallback<GeometryChangedEvent>(DynamicItemPerLineUpdate);
    }
    public void UnregisterDynamicItemPerLineUpdate()
    {
      _slotContainer.UnregisterCallback<GeometryChangedEvent>(DynamicItemPerLineUpdate);
    }

    protected abstract void DynamicItemPerLineUpdate(GeometryChangedEvent evt);



    protected VisualElement MakeLine()
    {
      VisualElement line = new VisualElement()
      {
        name = "GridLine",
        pickingMode = PickingMode.Position
      };

      line.AddToClassList("grid-line");
      line.AddToClassList("w-full");
      line.AddToClassList("flex-row");
      line.AddToClassList("items-center");
      line.AddToClassList("justify-evenly");

      for (int i = 0; i < _slotPerLine; i++)
      {
        VisualElement itemSlot = _itemSlotTemplate.Instantiate();

        line.Add(itemSlot);
        ItemSlots.Add(itemSlot);

        OnItemCreate(itemSlot);

        if (_hideEmpty)
        {
          itemSlot.style.visibility = Visibility.Hidden;
        }
      }

      return line;
    }

    public void SetInputPrompt(InputPrompt filterItemPrevious, InputPrompt filterItemNext)
    {
      _filterItemPrevious = filterItemPrevious;
      _filterItemNext = filterItemNext;

      RegisterInput();
    }

    public void SetFilters(List<Func<List<TItem>, int, List<TItem>>> filters)
    {
      _filters = filters;

      if (_filters != null)
      {
        _currentFilters = new();
        for (int i = 0; i < _filters.Count; i++)
        {
          _currentFilters.Add(-1);
        }
      }
    }
    public int GetFilter(int index)
    {
      if (_currentFilters == null || _currentFilters.Count <= index)
      {
        return -1;
      }

      return _currentFilters[index];
    }
    public void SetFilterButtons(List<List<Button>> filterButtons)
    {
      _filterButtons = filterButtons;

      if (_filterButtons != null)
      {
        for (int i = 0; i < _filterButtons.Count; i++)
        {
          List<Button> buttonList = _filterButtons[i];
          for (int y = 0; y < buttonList.Count; y++)
          {
            Button button = buttonList[y];
            if (button == null)
            {
              continue;
            }
            button.AddToClassList(_filterClass);

            int cacheFilterType = i;
            int cacheFilter = y;

            button.clicked += () =>
            {
              if (_currentFilters[cacheFilterType] == cacheFilter)
              {
                ClearFilter(cacheFilterType);
              }
              else
              {
                SetFilter(cacheFilterType, cacheFilter);
              }
            };
          }
        }
      }
    }

    public void SetFilterClearButtons(List<List<Button>> filterClearButtons)
    {
      _filterClearButtons = filterClearButtons;

      if (_filterClearButtons != null)
      {
        for (int i = 0; i < _filterClearButtons.Count; i++)
        {
          List<Button> buttonList = _filterClearButtons[i];

          for (int y = 0; y < buttonList.Count; y++)
          {
            Button button = buttonList[y];
            button.AddToClassList(_filterClass);
            button.AddToClassList(_filterClearClass);

            int cacheFilterType = i;
            int cacheFilter = y;

            button.clicked += () =>
            {
              ClearFilter(cacheFilterType);
            };
          }
        }
      }
    }

    public void SetHideEmpty(bool hideEmpty)
    {
      _hideEmpty = hideEmpty;
    }

    public void SetSelectedFilterColor(UIColorName color)
    {
      _selectedFilterColor = color;
    }
    public void SetSelectedFilterClass(string selectedFilterClass)
    {
      _selectedFilterClass = selectedFilterClass;
    }
    public void SetFilterClass(string filterClass)
    {
      _filterClass = filterClass;
    }

    public abstract void CreateSlots();
    protected abstract void RefreshItems();

    public void SetFilter(int filterIndex, int filter)
    {
      if (_currentFilters[filterIndex] == filter)
      {
        return;
      }

      _currentFilters[filterIndex] = filter;
      FilterList();

      OnFilterChange(filterIndex);

      RefreshItems();

      UpdateFilterButtons(filterIndex);
    }

    protected abstract void OnFilterChange(int filterIndex);

    public void ClearFilter(int filterIndex)
    {
      SetFilter(filterIndex, -1);
    }

    public void SetEnabledFilterButtons(int filterIndex, bool enabled)
    {
      var buttons = _filterButtons.Count > 0 ? _filterButtons[filterIndex] : new List<Button>();
      foreach (Button button in buttons)
      {
        if (button == null)
        {
          continue;
        }
        button.SetEnabled(enabled);
      }
    }
    public void UpdateFilterButtons(int filterIndex)
    {
      var buttons = _filterButtons.Count > 0 ? _filterButtons[filterIndex] : new List<Button>();
      var clearButtons = _filterClearButtons.Count > 0 ? _filterClearButtons[filterIndex] : new List<Button>();

      string selectedClass = _selectedFilterColor.ToString().ToLowerInvariant();
      int currentFilter = _currentFilters[filterIndex];

      foreach (var button in buttons)
      {
        if (button == null)
        {
          continue;
        }
        button.RemoveFromClassList(selectedClass);
        button.RemoveFromClassList(_selectedFilterClass);
      }

      if (currentFilter >= 0)
      {
        Button selectedButton = buttons[currentFilter];
        if (selectedButton != null)
        {
          selectedButton.AddToClassList(selectedClass);
          selectedButton.AddToClassList(_selectedFilterClass);
        }

        foreach (var button in clearButtons)
        {
          if (button == null)
          {
            continue;
          }
          button.RemoveFromClassList(selectedClass);
          button.RemoveFromClassList(_selectedFilterClass);
        }
      }
      else
      {
        foreach (var button in clearButtons)
        {
          if (button == null)
          {
            continue;
          }
          button.AddToClassList(selectedClass);
          button.AddToClassList(_selectedFilterClass);
        }
      }
    }

    public void SetItemList(List<TItem> itemList)
    {
      _itemList = itemList;
      FilterList();
      RefreshItems();
    }

    public void Refresh()
    {
      FilterList();
      RefreshItems();
    }

    protected override void OnFadeInStart()
    {
      base.OnFadeInStart();

      RegisterInput();
    }

    public void RegisterInput()
    {
      if (_filterItemPrevious == null)
      {
        return;
      }

#if UNITY_INPUT
      PlayerInput playerInput = InputDeviceManager.PlayerInput;

      if (playerInput != null)
      {
        // Register input actions
        _prevAction = playerInput.actions[_filterItemPrevious.InputActionName];

        if (_prevAction != null)
        {
          _prevAction.performed += PreviousTabInput;
        }

        _nextAction = playerInput.actions[_filterItemNext.InputActionName];
        if (_nextAction != null)
        {
          _nextAction.performed += NextTabInput;
        }
      }
#endif

      if (_filterItemPrevious != null)
      {
        _filterItemPrevious.clicked += PreviousTab;
      }
      if (_filterItemNext != null)
      {
        _filterItemNext.clicked += NextTab;
      }
    }

    protected override void OnFadeOut()
    {
      base.OnFadeOut();

      UnregisterInput();
    }
    public void UnregisterInput()
    {
#if UNITY_INPUT
      if (_prevAction != null)
      {
        _prevAction.performed -= PreviousTabInput;
      }

      if (_nextAction != null)
      {
        _nextAction.performed -= NextTabInput;
      }
#endif

      if (_filterItemPrevious != null)
      {
        _filterItemPrevious.clicked -= PreviousTab;
      }
      if (_filterItemNext != null)
      {
        _filterItemNext.clicked -= NextTab;
      }
    }

#if UNITY_INPUT
    private void PreviousTabInput(InputAction.CallbackContext context)
    {
      PreviousTab();
    }

    private void NextTabInput(InputAction.CallbackContext context)
    {
      NextTab();
    }
#endif

    public void PreviousTab()
    {
      // Only first filter is controlled via input
      PreviousTab(0);
    }
    public void PreviousTab(int filterIndex)
    {
      int filter = _currentFilters.Count > 0 ? _currentFilters[filterIndex] : -1;
      if (filter < 0)
      {
        var buttons = _filterButtons.Count > 0 ? _filterButtons[filterIndex] : new List<Button>();
        // No current filter, start from right, so max index
        filter = buttons.Count - 1;
      }
      else
      {
        filter--;
      }

      SetFilter(0, filter);
    }
    public void NextTab()
    {
      // Only first filter is controlled via input
      NextTab(0);
    }

    public void NextTab(int filterIndex)
    {
      int filter = _currentFilters.Count > 0 ? _currentFilters[filterIndex] : -1;
      if (filter < 0)
      {
        // No current filter, start from left
        filter = 0;
      }
      else
      {
        filter++;
        if (filter == _filterButtons[filterIndex].Count)
        {
          filter = -1;
        }
      }

      SetFilter(0, filter);
    }

    protected abstract void FilterList();
    public virtual List<TItem> GetFilteredList(List<int> filters)
    {
      if (filters == null)
      {
        return _itemList;
      }

      List<TItem> result = _itemList;

      for (int i = 0; i < _filters.Count; i++)
      {
        result = _filters.Count > 0 ? _filters[i](result, _currentFilters[i]) : result;
      }

      return result;
    }
    protected abstract void OnItemCreate(VisualElement slot);
    protected abstract void UnbindItem(VisualElement slot);
    protected abstract void BindItem(VisualElement slot, TItem item, int slotIndex, bool selected);
    public abstract void Select(int slotIndex);

    public abstract void Deselect(int slotIndex);

    public void DeselectAll()
    {
      _selectedItems.Clear();
      RefreshItems();
    }
  }
}