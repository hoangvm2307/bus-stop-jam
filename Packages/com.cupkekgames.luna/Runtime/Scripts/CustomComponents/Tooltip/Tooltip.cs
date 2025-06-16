using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Core.Pool;

namespace CupkekGames.Luna
{
  [UxmlElement]
  public partial class Tooltip : VisualElement
  {
    [UxmlAttribute] public float FadeDuration { get; private set; } = 0.2f;
    [UxmlAttribute] public EasingMode FadeEasingMode { get; private set; } = EasingMode.EaseOutCirc;
    public static readonly string ussClassName = "tooltip_container";
    public static readonly string ussClassNameItem = "tooltip_item";
    public static readonly string ussClassNameBackdrop = "backdrop";
    public static readonly string elementNameItem = "TooltipItem";
    protected Button _backdrop;
    // Pool
    private TooltipItemPool _tooltipItemPool;
    // State
    private List<TooltipItem> _items = new List<TooltipItem>();
    // Events
    public event Action<TooltipItem> OnGetFromPool;

    public Tooltip()
    {
      AddToClassList(ussClassName);
      pickingMode = PickingMode.Ignore;

      style.position = Position.Absolute;
      
      _backdrop = new Button();
      _backdrop.name = "Backdrop";
      _backdrop.AddToClassList(ussClassNameBackdrop);
      _backdrop.style.display = DisplayStyle.None;
      Insert(0, _backdrop);
    }
    public void OnEnable(MonoBehaviour coroutineRunner)
    {
      _backdrop.clicked += OnBackdropClicked;
      _tooltipItemPool = new TooltipItemPool(coroutineRunner, this, ussClassNameItem, elementNameItem, FadeDuration, FadeEasingMode);
    }

    public void OnDisable()
    {
      _backdrop.clicked -= OnBackdropClicked;
      CloseAll();
    }

    public TooltipItem GetTooltipItemFromPool()
    {
      var tooltipItem = _tooltipItemPool.Pool.Get();

      _items.Add(tooltipItem);

      tooltipItem.OnFadeOut += OnFadeOut;
      tooltipItem.OnFadeInStart += OnFadeInStart;

      OnGetFromPool?.Invoke(tooltipItem);

      // Debug.Log($"CREATED tooltip item, count: {_items.Count}");

      return tooltipItem;

    }
    public void DeactivateTooltipItem(TooltipItem item)
    {
      _items.Remove(item);
      item.OnFadeOut -= OnFadeOut;
      item.OnFadeInStart -= OnFadeInStart;
      item.SetActive(false);

      // Debug.Log($"DELETED tooltip item, count: {_items.Count}");
    }
    public void CloseAll()
    {
      // Debug.Log($"Closing all tooltip items, count: {_items.Count}");
      foreach (var item in _items)
      {
        item.Close();
      }
    }
    public void UnlockAll()
    {
      foreach (var item in _items)
      {
        item.Unlock();
      }
      AfterLockStateChanged(false);
    }
    public void Lock()
    {
      // Lock last item
      if (_items.Count > 0)
      {
        _items[_items.Count - 1].Lock();
        AfterLockStateChanged(true);
      }
    }
    public void ToggleLock()
    {
      // Find if any item is unlocked
      bool anyUnlocked = false;
      foreach (var item in _items)
      {
        if (!item.IsLocked)
        {
          anyUnlocked = true;
          break;
        }
      }

      if (anyUnlocked)
      {
        Lock();
      }
      else
      {
        UnlockAll();
      }
    }
    protected virtual void OnBackdropClicked()
    {
      UnlockAll();
    }
    
    protected virtual void AfterLockStateChanged(bool isLocked)
    {
      _backdrop.style.display = isLocked ? DisplayStyle.Flex : DisplayStyle.None;
    }

    protected virtual void OnFadeOut(TooltipItem item)
    {
      DeactivateTooltipItem(item);
    }
    protected virtual void OnFadeInStart(TooltipItem item)
    {
      UnlockHigherLevelTooltips(item.Level - 1, item);
    }
    public void UnlockHigherLevelTooltips(int level, TooltipItem except)
    {
      foreach (var tooltipItem in _items)
      {
        if (tooltipItem.Level > level && tooltipItem != except)
        {
          tooltipItem.Unlock();
        }
      }
    }
  }
}

