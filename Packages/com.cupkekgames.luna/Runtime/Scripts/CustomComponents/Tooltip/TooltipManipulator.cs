using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  public class TooltipManipulator : Manipulator
  {
    // References
    protected TooltipController _tooltipController;
    // Settings
    public List<TooltipContainerSetup> Setups;
    public Func<List<TooltipContainerSetup>> SetupProviders;
    // State
    private TooltipItem _tooltipItem;
    public TooltipItem TooltipItem => _tooltipItem;
    public TooltipManipulator(GameObject parent, TooltipController tooltipController, List<TooltipContainerSetup> setups) 
    {
      _tooltipController = tooltipController;
      Setup(parent, tooltipController);
      Setups = setups;
    }
    public TooltipManipulator(GameObject parent, TooltipController tooltipController)
    {
      _tooltipController = tooltipController;
      Setup(parent, tooltipController);
    }

    private void Setup(GameObject parent, TooltipController tooltipController)
    {
      _tooltipController = tooltipController;

      if (parent == null)
      {
        return;
      }

      if (!parent.TryGetComponent<TooltipReference>(out var tooltipRef))
      {
        tooltipRef = parent.AddComponent<TooltipReference>();
      }

      tooltipRef.Add(tooltipController.Tooltip);
    }

    protected override void RegisterCallbacksOnTarget()
    {
      target.RegisterCallback<MouseEnterEvent>(MouseEnterEvent);
      target.RegisterCallback<MouseOutEvent>(MouseOutEvent);

      target.RegisterCallback<FocusEvent>(FocusEvent);
      target.RegisterCallback<BlurEvent>(BlurEvent);

      target.RegisterCallback<MouseMoveEvent>(MouseMoveEvent);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
      target.UnregisterCallback<MouseEnterEvent>(MouseEnterEvent);
      target.UnregisterCallback<MouseOutEvent>(MouseOutEvent);

      target.UnregisterCallback<FocusEvent>(FocusEvent);
      target.UnregisterCallback<BlurEvent>(BlurEvent);

      target.UnregisterCallback<MouseMoveEvent>(MouseMoveEvent);
    }

    protected virtual void MouseEnterEvent(MouseEnterEvent e)
    {
      Show(e.mousePosition);
    }

    protected virtual void MouseOutEvent(MouseOutEvent e)
    {
      if (Close())
      {
        OnAfterClose();
      }
    }

    protected virtual void FocusEvent(FocusEvent e)
    {
      Show(null);
    }

    protected virtual void BlurEvent(BlurEvent e)
    {
      if (Close())
      {
        OnAfterClose();
      }
    }

    protected virtual void MouseMoveEvent(MouseMoveEvent e)
    {
      if (_tooltipItem == null || !_tooltipItem.IsActive)
      {
        return;
      }

      if (_tooltipItem.IsLocked)
      {
        return;
      }

      _tooltipItem.UpdatePosition(target, e.mousePosition);
    }
    protected virtual void OnBeforeShow()
    {
      if (_tooltipItem.IsLocked)
      {
        return;
      }

      UpdateDisplay(true);
    }
    protected virtual void OnAfterClose()
    {

    }

    public void Show(Vector2? mousePosition = null)
    {
      if (_tooltipItem == null || !_tooltipItem.IsActive)
      {
        _tooltipItem = _tooltipController.Tooltip.GetTooltipItemFromPool();
        _tooltipItem.OnActiveChanged += OnActiveChangedInner;
      }

      OnBeforeShow();

      _tooltipItem.Show(target, mousePosition);
    }
    protected virtual void OnActiveChangedInner(bool isActive, TooltipItem item)
    {
      if (!isActive)
      {
        _tooltipItem.OnActiveChanged -= OnActiveChangedInner;
        _tooltipItem = null;
      }
    }
    public bool Close()
    {
      if (_tooltipItem == null)
      {
        return false;
      }

      if (_tooltipItem.LastTarget == target)
      {
        return _tooltipItem.Close();
      }

      return false;
    }
    public void SetSetups(List<TooltipContainerSetup> setups)
    {
      Setups = setups;
    }

    public void SetSetup(TooltipContainerSetup setup)
    {
      Setups = new List<TooltipContainerSetup>() { setup };
    }

    public void SetSetupProviders(Func<List<TooltipContainerSetup>> setupProviders)
    {
      SetupProviders = setupProviders;
    }

    public virtual bool UpdateDisplay(bool force = false)
    {
      if (_tooltipItem == null)
      {
        return false;
      }

      if 
      (
        !force && 
        _tooltipItem.LastTarget != null && 
        _tooltipItem.LastTarget != target
      )
      {
        return false;
      }

      List<TooltipContainerSetup> setups = new();

      if (Setups != null)
      {
        setups.AddRange(Setups);
      }

      if (SetupProviders != null)
      {
        setups.AddRange(SetupProviders());
      }

      _tooltipItem.SetupContainers(setups);

      return true;
    }
  }
}
