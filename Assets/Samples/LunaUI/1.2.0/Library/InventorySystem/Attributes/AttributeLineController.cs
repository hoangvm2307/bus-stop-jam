using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using CupkekGames.Luna;

namespace CupkekGames.InventorySystem
{
  public class AttributeLineController
  {
    // UI Elements
    private VisualElement _container;
    private Label _name;
    private VisualElement _icon;
    private Label _oldValue;
    private VisualElement _arrow;
    private Label _newValue;
    private TooltipManipulator _tooltipManipulator;
    public AttributeLineController(VisualElement parent, bool withName, bool withIcon)
    {
      if (parent == null)
      {
        throw new Exception("parent cannot be null");
      }

      _container = new VisualElement()
      {
        name = "AttributeLine",
        pickingMode = PickingMode.Ignore
      };
      _container.AddToClassList("flex-row");
      _container.AddToClassList("items-center");

      if (withIcon)
      {
        _icon = new VisualElement()
        {
          name = "AttributeIcon",
          pickingMode = PickingMode.Ignore
        };
        _icon.AddToClassList("size-36");
        _icon.AddToClassList("size-min-36");
        _icon.AddToClassList("mr-8");
        _container.Add(_icon);
      }

      if (withName)
      {
        _name = new Label
        {
          name = "AttributeName",
          pickingMode = PickingMode.Ignore
        };
        _container.Add(_name);
      }

      _oldValue = new Label
      {
        name = "AttributeOld",
        pickingMode = PickingMode.Ignore
      };

      _arrow = new Label
      {
        name = "AttributeArrow",
        pickingMode = PickingMode.Ignore
      };

      _arrow.AddToClassList("icon-arrow-right");
      _arrow.AddToClassList("size-32");
      _arrow.AddToClassList("size-min-32");

      _newValue = new Label
      {
        name = "AttributeNew",
        pickingMode = PickingMode.Ignore
      };

      _arrow.style.display = DisplayStyle.None;
      _newValue.style.display = DisplayStyle.None;

      _container.Add(_oldValue);
      _container.Add(_arrow);
      _container.Add(_newValue);

      parent.Add(_container);
    }

    public void SetName(string name)
    {
      if (_name == null)
      {
        return;
      }

      _name.text = name;
    }
    public void SetIcon(Sprite sprite)
    {
      if (_icon == null)
      {
        return;
      }
      _icon.style.backgroundImage = new StyleBackground(sprite);
    }
    public void ShowIcon()
    {
      if (_icon == null)
      {
        return;
      }
      _icon.style.display = DisplayStyle.Flex;
    }
    public void HideIcon()
    {
      if (_icon == null)
      {
        return;
      }
      _icon.style.display = DisplayStyle.None;
    }
    public void SetOldValue(string value)
    {
      _oldValue.text = value;
    }

    public void SetNewValue(string value, AttributeChangeType changeType, bool hideOldValue)
    {
      if (changeType == AttributeChangeType.INCREASE)
      {
        value = RichTextColor.Colorize(value, RichTextColor.LIME);
      }
      else if (changeType == AttributeChangeType.DECREASE)
      {
        value = RichTextColor.Colorize(value, RichTextColor.RED);
      }

      _newValue.text = value;

      if (hideOldValue)
      {
        _oldValue.style.display = DisplayStyle.None;
        _arrow.style.display = DisplayStyle.None;
      }
      else
      {
        _oldValue.style.display = DisplayStyle.Flex;
        _arrow.style.display = DisplayStyle.Flex;
      }

      _newValue.style.display = DisplayStyle.Flex;
    }
    public void HideNewValue()
    {
      _arrow.style.display = DisplayStyle.None;
      _newValue.style.display = DisplayStyle.None;
    }

    public void SetData(AttributeLine data, GameObject parent = null, TooltipController tooltipController = null, TooltipPosition tooltipPosition = TooltipPosition.Right)
    {
      SetName(data.Name);

      if (_icon != null)
      {
        if (data.Icon != null)
        {
          SetIcon(data.Icon);
          ShowIcon();
        }
        else
        {
          HideIcon();
        }
      }

      SetOldValue(data.OldValue);
      if (!string.IsNullOrEmpty(data.NewValue))
      {
        SetNewValue(data.NewValue, data.ChangeType, data.HideOldValue);
      }

      if (parent != null && tooltipController != null)
      {
        AddTooltip(data, parent, tooltipController, tooltipPosition);
      }
      else
      {
        RemoveTooltip();
      }
    }

    private void AddTooltip(AttributeLine data, GameObject parent, TooltipController tooltipController, TooltipPosition tooltipPosition = TooltipPosition.Right)
    {
      RemoveTooltip();

      List<TooltipContainerSetup> setups = new List<TooltipContainerSetup> {
        new TooltipContainerSetup(null, new Label(data.Name), null, null)
      };

      _tooltipManipulator = new TooltipManipulator(parent, tooltipController, setups);

      _container.AddManipulator(_tooltipManipulator);
      _container.AddToClassList(tooltipPosition.GetUssClass());
      _container.pickingMode = PickingMode.Position;
    }
    private void RemoveTooltip()
    {
      if (_tooltipManipulator != null)
      {
        _container.RemoveManipulator(_tooltipManipulator);
      }

      _container.pickingMode = PickingMode.Ignore;
    }
  }
}