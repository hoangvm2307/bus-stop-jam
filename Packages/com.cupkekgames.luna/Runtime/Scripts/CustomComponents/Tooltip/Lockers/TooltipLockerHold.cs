using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using CupkekGames.Core;
using System;

namespace CupkekGames.Luna
{
  [RequireComponent(typeof(UIDocument))]
  public class TooltipLockerHold : TooltipLocker
  {
    [SerializeField] private float _holdTime = 0.5f;
    [SerializeField] private int _containerMinSize = 48;
    [SerializeField] private int _containerMarginTop = 16;
    [SerializeField] private string _progressBarClassName = "lockingProgressBar";
    [SerializeField] private int _progressBarSize = 48;
    [SerializeField] private int _progressBarMargin = 16;
    [SerializeField] private int _progressBarThickness = 8;
    [SerializeField] private int _pinnedIconSize = 48;
    [SerializeField] private Sprite _pinnedIconSprite;

    // Input Escape Manager Key
    private Guid _key;
    // State
    private Dictionary<RadialProgressBar, IVisualElementScheduledItem> _progressSchedules = new Dictionary<RadialProgressBar, IVisualElementScheduledItem>();
    protected override void Awake()
    {
      base.Awake();

      _key = Guid.NewGuid();
    }
    protected override void OnDisable()
    {
      base.OnDisable();

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

      InputEscapeManager.Push(OnEscape, _key);
    }

    protected override void OnFadeOutStart(TooltipItem item)
    {
      base.OnFadeOutStart(item);
      StopHoldAnimation();
      if (_openCount != 0)
      {
        // Unregister input when all tooltips are closed
        return;
      }

      InputEscapeManager.PopWithoutExecute(_key);
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
        VisualElement parent = container.Container.Q<VisualElement>("LockingHoldParent");
        RadialProgressBar progressBar = container.Container.Q<RadialProgressBar>("LockingProgressBar");
        VisualElement pinnedIcon = container.Container.Q<VisualElement>("LockingPinnedIcon");
        if (parent == null)
        {
          StyleLength length = new StyleLength(new Length(_containerMinSize, LengthUnit.Pixel));

          parent = new VisualElement()
          {
            name = "LockingHoldParent",
            pickingMode = PickingMode.Ignore,
          };
          parent.style.marginTop = new StyleLength(new Length(_containerMarginTop, LengthUnit.Pixel));
          parent.style.minHeight = length;
          parent.style.minWidth = length;
          container.Container.Add(parent);

          progressBar = new RadialProgressBar()
          {
            name = "LockingProgressBar",
            pickingMode = PickingMode.Ignore,
          };
          progressBar.AddToClassList(_progressBarClassName);
          progressBar.Thickness = _progressBarThickness;
          progressBar.SecondDelay = 0;
          progressBar.Instant = true;
          progressBar.TargetValue = 0;
          progressBar.TargetIndicator = 0;
          progressBar.style.position = Position.Absolute;
          progressBar.style.right = 0;
          length = new StyleLength(new Length(_progressBarSize, LengthUnit.Pixel));
          progressBar.style.width = length;
          progressBar.style.height = length;
          StyleLength margin = new StyleLength(new Length(_progressBarMargin, LengthUnit.Pixel)); 
          progressBar.style.marginLeft = margin;
          progressBar.style.marginTop = margin;
          progressBar.style.marginBottom = margin;
          progressBar.style.marginRight = margin;
          parent.Add(progressBar);

          pinnedIcon = new VisualElement()
          {
            name = "LockingPinnedIcon",
            pickingMode = PickingMode.Ignore,
          };
          pinnedIcon.style.position = Position.Absolute;
          pinnedIcon.style.right = 0;
          length = new StyleLength(new Length(_pinnedIconSize, LengthUnit.Pixel));
          pinnedIcon.style.width = length;
          pinnedIcon.style.height = length;
          pinnedIcon.AddToClassList("rounded-lg");
          pinnedIcon.style.backgroundColor = new Color(0, 0, 0, 0.5f);
          pinnedIcon.style.backgroundImage = new StyleBackground(_pinnedIconSprite);
          parent.Add(pinnedIcon);
        }

        parent.SendToBack();

        if (item.IsLocked)
        {
          progressBar.style.display = DisplayStyle.None;
          pinnedIcon.style.display = DisplayStyle.Flex;

          StopHoldAnimation();
          progressBar.TargetValue = 1;
        }
        else
        {
          progressBar.style.display = DisplayStyle.Flex;
          pinnedIcon.style.display = DisplayStyle.None;

          progressBar.TargetValue = 0;
          StartHoldAnimation(progressBar);
        }
      }
    }

    public void StartHoldAnimation(RadialProgressBar progressBar)
    {
        StopHoldAnimation();

        float elapsedTime = 0f;

        // Start scheduling an update every frame
        _progressSchedules[progressBar] = progressBar.schedule.Execute(() =>
        {
            // Update elapsed time
            elapsedTime += Time.deltaTime;

            progressBar.TargetValue = elapsedTime / _holdTime;
        })
        .Every(1) // must be 1 for deltaTime to work correctly
        .Until(() => {
          if (progressBar.TargetValue >= 1f)
          {
            _tooltip.ToggleLock();
            return true;
          }
          return false;
        });
    }
    public void StopHoldAnimation()
    {
        foreach (System.Collections.Generic.KeyValuePair<RadialProgressBar, IVisualElementScheduledItem> schedule in _progressSchedules)
        {
            schedule.Value?.Pause();
        }
    }
  }
}
