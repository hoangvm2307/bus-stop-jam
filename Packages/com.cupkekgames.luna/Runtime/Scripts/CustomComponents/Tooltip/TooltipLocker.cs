using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  [RequireComponent(typeof(UIDocument))]
  public abstract class TooltipLocker : MonoBehaviour
  {
    protected UIDocument _uiDocument;
    protected Tooltip _tooltip;
    public Tooltip Tooltip => _tooltip;
    private List<TooltipItem> _items = new();
    // State
    protected int _openCount = 0;

    protected virtual void Awake()
    {
      _uiDocument = GetComponent<UIDocument>();
      _tooltip = _uiDocument.rootVisualElement.Q<Tooltip>();
    }

    protected virtual void OnEnable()
    {
      _tooltip.OnGetFromPool += OnGetFromPool;
    }

    protected virtual void OnDisable()
    {
      _tooltip.OnGetFromPool -= OnGetFromPool;
      foreach (var item in _items)
      {
        item.OnFadeInStart -= OnFadeInStart;
        item.OnFadeOutStart -= OnFadeOutStart;
        item.OnLockStateChanged -= OnLockStateChanged;
      }
    }
    protected virtual void OnGetFromPool(TooltipItem item)
    {
      _items.Add(item);
      item.OnFadeInStart += OnFadeInStart;
      item.OnFadeOutStart += OnFadeOutStart;
      item.OnLockStateChanged += OnLockStateChanged;
    }
    protected virtual void OnFadeInStart(TooltipItem item)
    {
      _openCount++;
    }
    protected virtual void OnFadeOutStart(TooltipItem item)
    {
      _items.Remove(item);
      item.OnFadeInStart -= OnFadeInStart;
      item.OnFadeOutStart -= OnFadeOutStart;
      _openCount--;
    }
    protected virtual void OnLockStateChanged(bool isLocked, TooltipItem item)
    {
      
    }
  }
}
