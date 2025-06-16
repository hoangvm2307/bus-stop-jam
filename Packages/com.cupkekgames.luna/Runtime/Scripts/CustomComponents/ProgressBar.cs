using System;
using System.Collections.Generic;
using System.Linq;
using CupkekGames.Core;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  [UxmlElement]
  public partial class ProgressBar : VisualElement
  {
    #region Progress Settings
    [MultiLineHeader("Progress Settings")]
    [UxmlAttribute] public bool InstantPositive { get; set; } = true;
    [UxmlAttribute] public bool InstantNegative { get; set; } = true;
    [UxmlAttribute] public long UpdateFrequency { get; private set; } = 10;
    [UxmlAttribute] public float Step { get; private set; } = 0.01f;
    [UxmlAttribute] public long FirstDelay { get; private set; } = 0;
    [UxmlAttribute] public long SecondDelay { get; private set; } = 500;

    private ProgressBarNode[] _progress = new ProgressBarNode[1] { new ProgressBarNode() };
    [UxmlAttribute]
    public ProgressBarNode[] Progress
    {
      get
      {
        return _progress;
      }
      set
      {
        _progress = value;
      }
    }
    private ProgressBarNode _indicator = new ProgressBarNode();
    [UxmlAttribute]
    public ProgressBarNode Indicator
    {
      get
      {
        return _indicator;
      }
      set
      {
        _indicator = value;
        PlayIndicator();
      }
    }
    public void PlayIndicator()
    {
      bool positive = _indicator.TargetValue > _indicator.CurrentValue;
      bool instant = positive ? InstantPositive : InstantNegative;

      if (instant)
      {
        _indicator.CurrentValue = _indicator.TargetValue;
        if (_indicator.VisualElement != null)
        {
          _indicator.VisualElement.style.width = Length.Percent(_indicator.CurrentValue * 100);
        }
      }
      else
      {
        // if new value is greater than current value, progress goes first
        AnimateIndicator(positive);
      }
    }
    #endregion

    #region Color Settings
    private Color _colorBackground = new Color(0, 0, 0, 1);
    [MultiLineHeader("Color Settings")]
    [UxmlAttribute]
    public Color ColorBackground
    {
      get
      {
        return _colorBackground;
      }
      set
      {
        _colorBackground = value;

        SetColorBackground();
      }
    }
    private void SetColorBackground()
    {
      _background.ClearClassList();
      _background.AddToClassList(_ussBackground);
      _background.style.backgroundColor = _colorBackground;

      _iconElement.ClearClassList();
      _iconElement.AddToClassList(_ussIcon);
      _iconElement.style.backgroundColor = _colorBackground;
      _iconElement.style.unityBackgroundImageTintColor = _iconTint;
    }
    private Color _colorIndicatorPositive = new Color(0.0f, 1.0f, 0.0f, 1.0f);
    [UxmlAttribute]
    public Color ColorIndicatorPositive
    {
      get
      {
        return _colorIndicatorPositive;
      }
      set
      {
        _colorIndicatorPositive = value;

        SetIndicatorStyle(true);
      }
    }
    private Color _colorIndicatorNegative = new Color(1, 0, 0, 1);
    [UxmlAttribute]
    public Color ColorIndicatorNegative
    {
      get
      {
        return _colorIndicatorNegative;
      }
      set
      {
        _colorIndicatorNegative = value;

        SetIndicatorStyle(false);
      }
    }
    #endregion

    #region Other Settings
    private string _title;
    [MultiLineHeader("Other Settings")]
    [UxmlAttribute, CreateProperty]
    public string Title
    {
      get
      {
        return _title;
      }
      set
      {
        _title = value;
        SetTitle();
      }
    }
    private void SetTitle()
    {
      _titleLabel.text = _title;
    }

    private bool _showIcon;
    [UxmlAttribute]
    public bool ShowIcon
    {
      get
      {
        return _showIcon;
      }
      set
      {
        _showIcon = value;
        SetShowIcon();
      }
    }
    private void SetShowIcon()
    {
      if (_showIcon)
      {
        _iconElement.style.display = DisplayStyle.Flex;
      }
      else
      {
        _iconElement.style.display = DisplayStyle.None;
      }
    }
    private int _iconSize = 36;
    [UxmlAttribute]
    public int IconSize
    {
      get
      {
        return _iconSize;
      }
      set
      {
        _iconSize = value;
        SetIconSize();
      }
    }
    private void SetIconSize()
    {
      StyleLength length = new StyleLength(new Length(_iconSize, LengthUnit.Pixel));
      _iconElement.style.width = length;
      _iconElement.style.height = length;
    }
    private Color _iconTint = new Color(1, 1, 1, 1);
    [UxmlAttribute]
    public Color IconTint
    {
      get
      {
        return _iconTint;
      }
      set
      {
        _iconTint = value;

        SetIconTint();
      }
    }
    private void SetIconTint()
    {
      _iconElement.style.unityBackgroundImageTintColor = _iconTint;
    }
    private bool _verticalLine;
    [MultiLineHeader("VerticalLine doesn't work with OverlayImage because it requires overflow: visible")]
    [UxmlAttribute]
    public bool VerticalLine
    {
      get
      {
        return _verticalLine;
      }
      set
      {
        _verticalLine = value;
        SetVerticalLine();
      }
    }
    private void SetVerticalLine()
    {
      if (_progressVerticalLine != null)
      {
        _progressVerticalLine.RemoveFromHierarchy();
        _progressVerticalLine = null;
      }

      if (_verticalLine)
      {
        if (_progressElements == null || _progressElements.Length == 0)
        {
          return;
        }

        _progressVerticalLine = new VisualElement { pickingMode = PickingMode.Ignore };
        _progressVerticalLine.AddToClassList(_ussProgressVerticalLine);

        VisualElement parent = _progressElements[_progressElements.Length - 1].VisualElement;
        parent.Add(_progressVerticalLine);

        _progressContainer.style.overflow = Overflow.Visible;
        parent.style.overflow = Overflow.Visible;
      }
      else
      {
        _progressContainer.style.overflow = Overflow.Hidden;

        if (_progressVerticalLine != null)
        {
          _progressVerticalLine.RemoveFromHierarchy();
          _progressVerticalLine = null;
        }
      }
    }
    #endregion
    #region Overlay Settings
    private VectorImage _overlayImage;
    [MultiLineHeader("Overlay Settings")]
    [UxmlAttribute]
    public VectorImage OverlayImage
    {
      get
      {
        return _overlayImage;
      }
      set
      {
        _overlayImage = value;
        SetOverlayImage();
      }
    }
    private void SetOverlayImage()
    {
      _progressContainer.style.backgroundImage = new StyleBackground(_overlayImage);
    }
    private int _overlayImageWidth;
    [UxmlAttribute]
    public int OverlayImageWidth
    {
      get
      {
        return _overlayImageWidth;
      }
      set
      {
        _overlayImageWidth = value;
        SetOverlayImageWidth();
      }
    }

    private void SetOverlayImageWidth()
    {
      _progressContainer.style.backgroundSize = new BackgroundSize(new Length(_overlayImageWidth, LengthUnit.Pixel), Length.Percent(100));
    }
    private Color _colorOverlay = new Color(1, 1, 1, 1);
    [UxmlAttribute]
    public Color ColorOverlay
    {
      get
      {
        return _colorOverlay;
      }
      set
      {
        _colorOverlay = value;

        SetColorOverlay();
      }
    }

    private void SetColorOverlay()
    {
      _progressContainer.style.unityBackgroundImageTintColor = _colorOverlay;
    }

    #endregion

    #region Visual Elements

    private VisualElement _background;
    private VisualElement _progressContainer;
    private VisualElement _progressElementsContainer;
    private ProgressBarNode[] _progressElements;
    private VisualElement _progressVerticalLine;
    private VisualElement _titleContainer;
    private Label _titleLabel;
    private VisualElement _iconElement;
    public VisualElement IconElement => _iconElement;

    #endregion
    #region USS Classes

    private const string _ussClassName = "ProgressBar";
    private const string _ussBackground = _ussClassName + "__background";
    private const string _ussProgressContainer = _ussClassName + "__progress-container";
    private const string _ussProgressElementsContainer = _ussClassName + "__progress-elements-container";
    private const string _ussProgressElement = _ussClassName + "__progress-element";
    private const string _ussProgressVerticalLine = _ussClassName + "__progress-vertical-line";
    private const string _ussChangeIndicator = _ussClassName + "__change-indicator";
    private const string _ussTitleContainer = _ussClassName + "__title-container";
    private const string _ussTitle = _ussClassName + "__title";
    private const string _ussIcon = _ussClassName + "__icon";
    #endregion
    #region USS Style
    private CustomStyleProperty<Color> s_BgColor = new CustomStyleProperty<Color>("--progress-bar-bg-color");
    private CustomStyleProperty<Color> s_IndicatorColorPositive = new CustomStyleProperty<Color>("--progress-bar-indicator-positive-color");
    private CustomStyleProperty<Color> s_IndicatorColorNegative = new CustomStyleProperty<Color>("--progress-bar-indicator-negative-color");
    private CustomStyleProperty<Color> s_OverlayColor = new CustomStyleProperty<Color>("--progress-bar-overlay-color");
    private CustomStyleProperty<Color>[] s_ProgressColor = new CustomStyleProperty<Color>[] { 
      new CustomStyleProperty<Color>("--progress-bar-progress-color-0"), 
      new CustomStyleProperty<Color>("--progress-bar-progress-color-1"), 
      new CustomStyleProperty<Color>("--progress-bar-progress-color-2"), 
      new CustomStyleProperty<Color>("--progress-bar-progress-color-3"), 
      new CustomStyleProperty<Color>("--progress-bar-progress-color-4"),
      new CustomStyleProperty<Color>("--progress-bar-progress-color-5"),
      new CustomStyleProperty<Color>("--progress-bar-progress-color-6"),
      new CustomStyleProperty<Color>("--progress-bar-progress-color-7"),
      new CustomStyleProperty<Color>("--progress-bar-progress-color-8"),
      new CustomStyleProperty<Color>("--progress-bar-progress-color-9"),
    };

    #endregion

    public ProgressBar()
    {
      AddToClassList(_ussClassName);
      pickingMode = PickingMode.Ignore;

      // background
      _background = new VisualElement { pickingMode = PickingMode.Ignore };
      Add(_background);
      // progress container
      _progressContainer = new VisualElement { pickingMode = PickingMode.Ignore, usageHints = UsageHints.MaskContainer };
      _progressContainer.AddToClassList(_ussProgressContainer);
      _background.Add(_progressContainer);
      // change indicator inner
      _indicator.CreateElement();
      _indicator.VisualElement.AddToClassList(_ussChangeIndicator);
      _progressContainer.Add(_indicator.VisualElement);
      // progress elements container
      _progressElementsContainer = new VisualElement { pickingMode = PickingMode.Ignore };
      _progressElementsContainer.AddToClassList(_ussProgressElementsContainer);
      _progressContainer.Add(_progressElementsContainer);
      // title container
      _titleContainer = new VisualElement { pickingMode = PickingMode.Ignore };
      _titleContainer.AddToClassList(_ussTitleContainer);
      _background.Add(_titleContainer);
      // title
      _titleLabel = new Label { pickingMode = PickingMode.Ignore };
      _titleLabel.AddToClassList(_ussTitle);
      _titleContainer.Add(_titleLabel);
      // icon
      _iconElement = new VisualElement { pickingMode = PickingMode.Ignore };
      _iconElement.ClearClassList();
      _iconElement.AddToClassList(_ussIcon);
      _iconElement.style.backgroundColor = _colorBackground;
      Add(_iconElement);

      RegisterCallback<AttachToPanelEvent>(OnAttach);
      RegisterCallback<CustomStyleResolvedEvent>(CustomStylesResolved);
    }

    private void OnAttach(AttachToPanelEvent evt)
    {
      SetColorBackground();
      SetIconTint();
      SetOverlayImage();
      SetColorOverlay();

      SetTitle();

      SetShowIcon();
      SetIconSize();

      SetIndicatorStyle(true);
      PlayIndicator();

      RebuildProgressSegments();
      InstantSegments();
    }

    private static void CustomStylesResolved(CustomStyleResolvedEvent evt)
    {
        ProgressBar element = (ProgressBar)evt.currentTarget;
        element.UpdateCustomStyles();
    }

    private void UpdateCustomStyles()
    {
      if (customStyle.TryGetValue(s_BgColor, out _colorBackground))
      {
        SetColorBackground();
      }

      bool positive = customStyle.TryGetValue(s_IndicatorColorPositive, out _colorIndicatorPositive);
      bool negative = customStyle.TryGetValue(s_IndicatorColorNegative, out _colorIndicatorNegative);

      if (positive || negative)
      {
        SetIndicatorStyle(positive);
      }

      if (customStyle.TryGetValue(s_OverlayColor, out _colorOverlay))
      {
        SetColorOverlay();
      }

      bool colorChanged = false;
      for (int i = 0; i < s_ProgressColor.Length; i++)
      {
        if (_progress == null || _progress.Length <= i || _progress[i] == null)
        {
          break;
        }

        ProgressBarNode node = _progress[i];

        if (customStyle.TryGetValue(s_ProgressColor[i], out Color tempColor))
        {
          node.Color = tempColor;
          colorChanged = true;
        }
      }

      if (colorChanged)
      {
        RebuildProgressSegments();
        InstantSegments();
      }
    }

    public void PlayProgress()
    {
      if (_progressElements == null)
      {
        return;
      }

      float totalCurrent = 0;
      float totalTarget = 0;
      for (int i = 0; i < _progressElements.Length; i++)
      {
        ProgressBarNode node = _progressElements[i];
        totalCurrent += node.CurrentValue;
        totalTarget += node.TargetValue;
      }

      bool positive = totalTarget > totalCurrent;
      bool instant = positive ? InstantPositive : InstantNegative;
      bool first = !positive;
      float before = 0;

      for (int i = 0; i < _progressElements.Length; i++)
      {
        ProgressBarNode node = _progressElements[i];

        node.VisualElement.style.backgroundColor = _progress[i].Color;

        if (instant)
        {
          node.CurrentValue = node.TargetValue;

          InstantSegments();
        }
        else
        {
          if (positive)
          {
            if (i > 0)
            {
              node.VisualElement.style.left = Length.Percent(before);
            }
            before += node.TargetValue * 100;

            AnimateProgress(first, i, false);
          }
          else
          {
            AnimateProgress(first, i, true);
          }
        }
      }
    }
    private void AnimateProgress(bool first, int i, bool updatePos)
    {
      if (_progressElements == null)
      {
        return;
      }

      long delay = first ? FirstDelay : SecondDelay;

      ProgressBarNode node = _progressElements[i];
      node.ScheduledItem?.Pause();

      if (UpdateFrequency > 0.0f)
      {
        float step = node.CurrentValue > node.TargetValue ? -Step : Step;
        float clampMin = step > 0 ? 0 : node.TargetValue;
        float clampMax = step > 0 ? node.TargetValue : 1;

        if (!Mathf.Approximately(node.CurrentValue, node.TargetValue))
        {
          node.ScheduledItem = _progressElements[i].VisualElement.schedule
              .Execute(() =>
              {
                float distance = Mathf.Abs(node.CurrentValue - node.TargetValue);
                float dynamicStep = Mathf.Lerp(step, step * 8, distance);
                node.CurrentValue += dynamicStep;
                node.CurrentValue = Mathf.Clamp(node.CurrentValue, clampMin, clampMax);  // Clamp between 0 and 1 as percentages
                _progressElements[i].VisualElement.style.width = Length.Percent(node.CurrentValue * 100);

                if (updatePos && i > 0)
                {
                  UpdateStartPos(i);
                }
              })
              .Every(UpdateFrequency)
              .Until(() =>
                {
                  // Check if the condition to stop the task is met
                  bool isCompleted = step > 0 ? node.CurrentValue >= node.TargetValue : node.CurrentValue <= node.TargetValue;

                  if (updatePos && isCompleted && i > 0)
                  {
                    UpdateStartPos(i);
                  }

                  return isCompleted;
                })
              .StartingIn(delay);
        }
        else
        {
          _progressElements[i].VisualElement.style.width = Length.Percent(node.TargetValue * 100);
          if (updatePos && i > 0)
          {
            UpdateStartPos(i);
          }
        }
      }
      else
      {
        _progressElements[i].VisualElement.style.width = Length.Percent(node.TargetValue * 100);
        if (updatePos && i > 0)
        {
          UpdateStartPos(i);
        }
      }
    }

    private void UpdateStartPos(int i)
    {
      ProgressBarNode node = _progressElements[i];
      float before = 0;
      for (int y = 0; y < i; y++)
      {
        ProgressBarNode beforeNode = _progressElements[y];
        before += beforeNode.TargetValue * 100;
      }

      node.VisualElement.style.left = Length.Percent(before);
    }

    private void AnimateIndicator(bool first)
    {
      if (_indicator.VisualElement == null)
      {
        return;
      }

      if (_indicator.TargetValue > _indicator.CurrentValue)
      {
        SetIndicatorStyle(true);
      }
      else
      {
        SetIndicatorStyle(false);
      }

      long delay = first ? FirstDelay : SecondDelay;

      if (UpdateFrequency > 0.0f)
      {
        _indicator.ScheduledItem?.Pause();

        float step = _indicator.CurrentValue > _indicator.TargetValue ? -Step : Step;
        float clampMin = step > 0 ? 0 : _indicator.TargetValue;
        float clampMax = step > 0 ? _indicator.TargetValue : 1;

        if (!Mathf.Approximately(_indicator.CurrentValue, _indicator.TargetValue))
        {
          _indicator.ScheduledItem = _indicator.VisualElement.schedule
              .Execute(() =>
              {
                float distance = Mathf.Abs(_indicator.CurrentValue - _indicator.TargetValue);
                float dynamicStep = Mathf.Lerp(step, step * 8, distance);
                _indicator.CurrentValue += dynamicStep;
                _indicator.CurrentValue = Mathf.Clamp(_indicator.CurrentValue, clampMin, clampMax);  // Clamp between 0 and 1 as percentages
                _indicator.VisualElement.style.width = Length.Percent(_indicator.CurrentValue * 100);
              })
              .Every(UpdateFrequency)
              .Until(() => step > 0 ? _indicator.CurrentValue >= _indicator.TargetValue : _indicator.CurrentValue <= _indicator.TargetValue)
              .StartingIn(delay);
        }
        else
        {
          _indicator.VisualElement.style.width = Length.Percent(_indicator.TargetValue * 100);
        }
      }
      else
      {
        _indicator.VisualElement.style.width = Length.Percent(_indicator.TargetValue * 100);
      }
    }

    public void SetIndicatorStyle(bool positive)
    {
      if (_indicator.VisualElement == null)
      {
        return;
      }

      _indicator.VisualElement.ClearClassList();
      _indicator.VisualElement.AddToClassList(_ussChangeIndicator);

      if (positive)
      {
        _indicator.VisualElement.style.backgroundColor = _colorIndicatorPositive;
      }
      else
      {
        _indicator.VisualElement.style.backgroundColor = _colorIndicatorNegative;
      }
    }

    public void SetFilling(float current, float add, float max)
    {
      float next = current + add;
      if (next > max)
      {
        next = max;
      }
      string sign = "";
      if (add >= 0)
      {
        sign = "+";
        _progressElements[0].TargetValue = current / max;
        Indicator.TargetValue = next / max;
        SetIndicatorStyle(true);
      }
      else
      {
        Indicator.TargetValue = current / max;
        _progressElements[0].TargetValue = next / max;
        SetIndicatorStyle(false);
      }

      Title = next + " (" + sign + add + ")";
    }

    public void UpdateProgressSegments()
    {
      if (_progressElements == null)
      {
        return;
      }

      for (int i = 0; i < _progress.Length; i++)
      {
        _progressElements[i].TargetValue = _progress[i].TargetValue;
        _progressElements[i].Color = _progress[i].Color;
      }
    }
    private void InstantSegments()
    {
      if (_progressElements == null)
      {
        return;
      }

      float start = 0;
      for (int i = 0; i < _progressElements.Length; i++)
      {
        ProgressBarNode node = _progressElements[i];

        node.CurrentValue = node.TargetValue;
        float percent = node.CurrentValue * 100;

        node.VisualElement.style.width = Length.Percent(percent);
        node.VisualElement.style.left = Length.Percent(start);

        start += percent;
      }
    }
    public void RebuildProgressSegments()
    {
      int required = _progress.Length;

      // Remove excess elements if _progressElements is not null.
      if (_progressElements != null)
      {
        for (int i = required; i < _progressElements.Length; i++)
          _progressElements[i].VisualElement.RemoveFromHierarchy();
      }

      // Create a new array for the required number of nodes.
      ProgressBarNode[] newElements = new ProgressBarNode[required];

      for (int i = 0; i < required; i++)
      {
        // Reuse existing node if available.
        if (_progressElements != null && i < _progressElements.Length)
          newElements[i] = _progressElements[i];

        // If no existing node, create a new one.
        if (newElements[i] == null)
        {
          newElements[i] = new ProgressBarNode();
          newElements[i].CreateElement();
          _progressElementsContainer.Add(newElements[i].VisualElement);
        }

        // Setup the node.
        newElements[i].VisualElement.ClearClassList();
        newElements[i].VisualElement.AddToClassList(_ussProgressElement);
        newElements[i].Color = _progress[i].Color;
        newElements[i].CurrentValue = _progress[i].CurrentValue;
        newElements[i].TargetValue = _progress[i].TargetValue;
        newElements[i].VisualElement.style.backgroundColor = _progress[i].Color;
      }

      _progressElements = newElements;
      SetVerticalLine();
    }
    public void SetOverlayRepeatAmount(int repeatAmount)
    {
      float width = _progressContainer.contentRect.width;
      if (float.IsNaN(width) || width <= 0)
      {
        return;
      }

      int cellWidth = (int)(width / repeatAmount + 0.5f);

      OverlayImageWidth = cellWidth;
    }
  }
}
