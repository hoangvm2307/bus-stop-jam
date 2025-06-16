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
  public class TooltipLockerInputPrompt : TooltipLocker
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
        VisualElement locking = container.Container.Q<VisualElement>("LockingParent");
        VisualElement unlocking = container.Container.Q<VisualElement>("UnlockingParent");

        if (item.IsLocked)
        {
          if (unlocking == null)
          {
            container.Container.Add(CreateUnlockingPrompt());
          }
          else
          {
            unlocking.style.display = DisplayStyle.Flex;
          }
          if (locking != null)
          {
            locking.style.display = DisplayStyle.None;
          }
        }
        else
        {
          if (locking == null)
          {
            container.Container.Add(CreateLockingPrompt());
          }
          else
          {
            locking.style.display = DisplayStyle.Flex;
          }
          if (unlocking != null)
          {
            unlocking.style.display = DisplayStyle.None;
          }
        }
      }
    }
    protected VisualElement CreateLockingPrompt()
    {
      VisualElement parent = new VisualElement()
      {
        name = "LockingParent",
        pickingMode = PickingMode.Ignore,
      };

      InputPrompt prompt = new InputPrompt()
      {
        name = "LockingPrompt",
        InputActionName = _lockActionNames[0],
        LabelText = "Lock",
      };
      parent.Add(prompt);

      return parent;
    }
    protected VisualElement CreateUnlockingPrompt()
    {
      VisualElement parent = new VisualElement()
      {
        name = "UnlockingParent",
      };
      parent.style.flexDirection = FlexDirection.Row;

      InputPrompt prompt = new InputPrompt()
      {
        name = "UnlockingPrompt",
        InputActionName = _lockActionNames[0],
        LabelText = "or",
      };
      parent.Add(prompt);
      prompt.clicked += OnEscape;

      LunaUIManager uiManager = LunaUIManager.Instance;
      InputPrompt escapePrompt = new InputPrompt()
      {
        name = "EscapePrompt",
        InputActionName = uiManager.EscapeActionNames[0],
        LabelText = " to unlock",
      };
      parent.Add(escapePrompt);
      escapePrompt.clicked += OnEscape;

      return parent;
    }
#endif
  }
}
