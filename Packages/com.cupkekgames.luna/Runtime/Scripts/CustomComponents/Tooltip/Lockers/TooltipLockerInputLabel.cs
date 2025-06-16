using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using CupkekGames.Core;
using System;



#if UNITY_INPUT
using UnityEngine.InputSystem;
#endif

namespace CupkekGames.Luna
{
  [RequireComponent(typeof(UIDocument))]
  public class TooltipLockerInputLabel : TooltipLocker
  {
#if UNITY_INPUT
    [SerializeField] private PlayerInput _playerInput;
    public PlayerInput PlayerInput => _playerInput;
    [SerializeField] private List<string> _lockActionNames = new List<string>() { "UI/Lock" };

    // Input Escape Manager Key
    private Guid _key;
    protected override void Awake()
    {
      base.Awake();

      _key = Guid.NewGuid();
    }
    protected override void OnDisable()
    {
      base.OnDisable();

      if (_playerInput != null)
      {
        foreach (string actionName in _lockActionNames)
        {
          InputAction action = _playerInput.actions[actionName];
          if (action != null)
          {
            action.performed -= OnLockAction;
          }
        }
      }
      
      InputEscapeManager.PopWithoutExecute(_key);
    }

    protected override void OnFadeInStart(TooltipItem item)
    {
      base.OnFadeInStart(item);
      UpdateLockingElements(item);
      if (_openCount != 1)
      {
        // Only register input if the tooltip is open for the first time
        return;
      }

      if (_playerInput != null)
      {
        foreach (string actionName in _lockActionNames)
        {
          InputAction action = _playerInput.actions[actionName];
          if (action != null)
          {
            action.performed += OnLockAction;
          }
        }
      }

      InputEscapeManager.Push(OnEscape, _key);
    }

    protected override void OnFadeOutStart(TooltipItem item)
    {
      base.OnFadeOutStart(item);
      if (_openCount != 0)
      {
        // Unregister input when all tooltips are closed
        return;
      }

      if (_playerInput != null)
      {
        foreach (string actionName in _lockActionNames)
        {
          InputAction action = _playerInput.actions[actionName];
          if (action != null)
          {
            action.performed -= OnLockAction;
          }
        }
      }

      InputEscapeManager.PopWithoutExecute(_key);
    }
    private void OnLockAction(InputAction.CallbackContext context)
    {
      _tooltip.ToggleLock();
    }

    private void OnEscape()
    {
      _tooltip.UnlockAll();
    }
    protected override void OnLockStateChanged(bool isLocked, TooltipItem item)
    {
      base.OnLockStateChanged(isLocked, item);
      UpdateLockingElements(item);
    }
    protected void UpdateLockingElements(TooltipItem item)
    {
      foreach (TooltipContainer container in item.Containers)
      {
        Label label = container.Container.Q<Label>("LockingLabel");
        if (label == null)
        {
          label = new Label()
          {
            name = "LockingLabel",
            pickingMode = PickingMode.Ignore,
            text = GetLockingLabelText(item),
          };
          container.Container.Add(label);
        }
        else
        {
          label.text = GetLockingLabelText(item);
        }
      }
    }
    protected string GetLockingLabelText(TooltipItem item)
    {
      LunaUIManager uiManager = LunaUIManager.Instance;
      string inputText = uiManager.IconDatabase.GetInputPrompt(_playerInput.actions[_lockActionNames[0]], true).Text;

      string escapeText = uiManager.IconDatabase.GetInputPrompt(uiManager.PlayerInput.actions[uiManager.EscapeActionNames[0]], true).Text;
      if (item.IsLocked)
      {
        return $"{RichTextColor.RED}Press {inputText} to unlock (or {escapeText}){RichTextColor.CLOSING_TAG}";
      }
      else
      {
        return $"{RichTextColor.AQUA}Press {inputText} to lock{RichTextColor.CLOSING_TAG}";
      }
    }
#endif
  }
}
