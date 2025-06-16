using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  public class TooltipItem
  {
    // References
    private MonoBehaviour _coroutineRunner;
    private TooltipItemPool _pool;
    private VisualElement _container;
    private VisualElement _parentElement;
    public VisualElement ParentElement => _parentElement;
    private string _ussClassName;
    private string _elementName;
    // Settings
    private float _fadeDuration;
    private EasingMode _easingMode;
    // State
    private FadeUIElement _fade;
    private VisualElement _lastTarget;
    public VisualElement LastTarget => _lastTarget;
    private bool _isLocked;
    public bool IsLocked => _isLocked;
    private List<TooltipContainer> _containers = new();
    public ReadOnlyCollection<TooltipContainer> Containers => _containers.AsReadOnly();
    private bool _isActive = false;
    public bool IsActive => _isActive;
    private int _level = 0;
    public int Level => _level;
    // Events
    public event Action<bool, TooltipItem> OnLockStateChanged;
    public event Action<TooltipItem> OnFadeInStart;
    public event Action<TooltipItem> OnFadeOutStart;
    public event Action<TooltipItem> OnFadeIn;
    public event Action<TooltipItem> OnFadeOut;
    public event Action<bool, TooltipItem> OnActiveChanged;

    public TooltipItem(
      MonoBehaviour coroutineRunner,
      TooltipItemPool pool,
      VisualElement container,
      string ussClassName,
      string elementName,
      float fadeDuration = 0.5f,
      EasingMode easingMode = EasingMode.EaseOutCirc
    )
    {
      _coroutineRunner = coroutineRunner;
      _pool = pool;
      _container = container;
      _ussClassName = ussClassName;
      _elementName = elementName;
      _fadeDuration = fadeDuration;
      _easingMode = easingMode;

      CreateTooltipElement();
      AddContainer();
    }

    private void CreateTooltipElement()
    {
      _parentElement = new VisualElement(){
        name = _elementName,
        pickingMode = PickingMode.Ignore,
        style = {
          position = Position.Absolute,
        }
      };
      _parentElement.AddToClassList(_ussClassName);
      _container.Add(_parentElement);

      _fade = new FadeUIElement(_coroutineRunner, _parentElement);
      _fade.SetEasing(_easingMode);
      _fade.SetDuration(_fadeDuration);
      _fade.SetTransitionProperty();
    }

    public void SetActive(bool active)
    {
      SetActiveInner(active, false);
    }
    private void SetActiveInner(bool active, bool ignorePool)
    {
      if (_isActive == active) return;

      // Debug.Log($"Setting active from {_isActive} to {active}");
      _isActive = active;

      if (active) {
        // Push element to last index of parent
        _parentElement.BringToFront();
        
        _fade.OnFadeIn += OnFadeInInner;
        _fade.OnFadeOut += OnFadeOutInner;
        _fade.OnFadeInStart += OnFadeInStartInner;
        _fade.OnFadeOutStart += OnFadeOutStartInner;
      } else {
        _fade.Stop();

        _fade.OnFadeIn -= OnFadeInInner;
        _fade.OnFadeOut -= OnFadeOutInner;
        _fade.OnFadeInStart -= OnFadeInStartInner;
        _fade.OnFadeOutStart -= OnFadeOutStartInner;

        if (!ignorePool)
        {
          _pool.Pool.Release(this);
        }
      }

      OnActiveChanged?.Invoke(active, this);
    }

    private void OnFadeInInner()
    {
      OnFadeIn?.Invoke(this);
    }

    private void OnFadeOutInner()
    {
      OnFadeOut?.Invoke(this);
    }

    private void OnFadeInStartInner()
    {
      OnFadeInStart?.Invoke(this);
    }

    private void OnFadeOutStartInner()
    {
      OnFadeOutStart?.Invoke(this);
    }

    public void OnPoolDestroy()
    {
      SetActiveInner(false, true);
      _parentElement.RemoveFromHierarchy();
    }

    public virtual bool Show(VisualElement target, Vector2? mousePosition = null)
    {
      if (target == null) return false;
      if (_isLocked) return false;
      _level = GetTargetLevel(target) + 1;
      _lastTarget = target;

      _parentElement.ClearClassList();
      _parentElement.AddToClassList(_ussClassName);
      _parentElement.userData = _level;

      _fade.FadeIn();

      _parentElement.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

      return true;
    }
    private int GetTargetLevel(VisualElement target)
    {
      VisualElement tooltipItem = UITKQueryUtility.FindParentWithName(target, Tooltip.elementNameItem);
      if (tooltipItem == null)
      {
        return 0;
      }

      return (int) tooltipItem.userData;
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
      UpdatePosition(_lastTarget);
      _parentElement.UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    public void UpdatePosition(VisualElement target, Vector2? mousePosition = null)
    {
      // check if there is position hint in tooltip
      TooltipPosition targetPosition;

      if (target.ClassListContains("tooltip-left"))
      {
        targetPosition = TooltipPosition.Left;
        _parentElement.style.flexDirection = FlexDirection.RowReverse;
      }
      else if (target.ClassListContains("tooltip-right"))
      {
        targetPosition = TooltipPosition.Right;
        _parentElement.style.flexDirection = FlexDirection.Row;
      }
      else if (target.ClassListContains("tooltip-bottom"))
      {
        targetPosition = TooltipPosition.Bottom;
        _parentElement.style.flexDirection = FlexDirection.Row;
      }
      else // if (target.ClassListContains("tooltip-top"))
      {
        targetPosition = TooltipPosition.Top;
        _parentElement.style.flexDirection = FlexDirection.Row;
      }

      bool followMouse = false;
      if (target.ClassListContains("tooltip-follow"))
      {
        followMouse = true;
      }
      
      (TooltipCoordinates coordinates, bool didSwitch) = TooltipCoordinateCalculator.CalculateOptimalPosition(
        targetPosition,
        target,
        _parentElement,
        _parentElement.parent,
        mousePosition,
        followMouse
      );
      
      // Update flex direction if position was switched
      if (didSwitch)
      {
        // For left position, use RowReverse, otherwise Row
        _parentElement.style.flexDirection = targetPosition == TooltipPosition.Left ? 
          FlexDirection.RowReverse : FlexDirection.Row;
      }

      // Apply coordinates
      _parentElement.style.right = coordinates.right.HasValue ? coordinates.right.Value : StyleKeyword.Auto;
      _parentElement.style.left = coordinates.left.HasValue ? coordinates.left.Value : StyleKeyword.Auto;
      _parentElement.style.top = coordinates.top.HasValue ? coordinates.top.Value : StyleKeyword.Auto;
      _parentElement.style.bottom = coordinates.bottom.HasValue ? coordinates.bottom.Value : StyleKeyword.Auto;
    }

    public virtual bool Close()
    {
      if (_isLocked) return false;

      _fade.FadeOut();

      return true;
    }

    public bool IsOpen()
    {
      return _parentElement.style.visibility == Visibility.Visible;
    }

    public void Lock()
    {
      _isLocked = true;
      OnLockStateChanged?.Invoke(true, this);
    }

    public void Unlock()
    {
      _isLocked = false;
      OnLockStateChanged?.Invoke(false, this);
      Close();
    }

    public void ToggleLock()
    {
      if (_isLocked)
        Unlock();
      else
        Lock();
    }
    public void SetupContainers(List<TooltipContainerSetup> setups)
    {
      while (_containers.Count < setups.Count)
      {
        AddContainer();
      }

      for (int i = 0; i < _containers.Count; i++)
      {
        if (i < setups.Count)
        {
          _containers[i].OnBeforeShow(setups[i]);
        }
        else
        {
          _containers[i].Hide();
        }
      }
    }

    protected virtual void AddContainer()
    {
      VisualElement container = new() {
        name = "TooltipContainer",
        pickingMode = PickingMode.Ignore,
      };

      VisualElement containerTop = new();
      containerTop.AddToClassList("tooltip_top_container");
      container.Add(containerTop);

      VisualElement containerImage = new();
      containerImage.name = "TooltipImageContainer";
      containerImage.AddToClassList("tooltip_image_container");
      containerTop.Add(containerImage);

      VisualElement containerTopText = new();
      containerTopText.AddToClassList("tooltip_top_text_container");
      containerTop.Add(containerTopText);

      VisualElement title = new();
      title.name = "TooltipTitle";
      title.AddToClassList("tooltip_title");
      containerTopText.Add(title);

      VisualElement body = new();
      body.name = "TooltipBody";
      body.AddToClassList("tooltip_body");
      containerTopText.Add(body);

      VisualElement bottom = new();
      bottom.name = "TooltipBottom";
      bottom.AddToClassList("tooltip_bottom_container");
      container.Add(bottom);

      _containers.Add(new TooltipContainer(container));
      _parentElement.Add(container);
    }

    private void RemoveContainer()
    {
      int index = _containers.Count - 1;
      if (index < 0)
      {
        return;
      }

      _containers.RemoveAt(index);
      _parentElement.RemoveAt(index);
    }
    public void RemoveAllContainers()
    {
      while (_containers.Count > 0)
      {
        RemoveContainer();
      }
    }
  }
}
