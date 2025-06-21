using System;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CupkekGames.Luna;

namespace CupkekGames.InventorySystem
{
  public class AttributeDataController
  {
    private VisualElement _container;
    public VisualElement Container => _container;
    private Dictionary<int, AttributeLineController> _controllers = new();
    // Settings
    private ICollection<string> _attributes;
    private List<Sprite> _icons;
    private List<Func<float, string>> _beautify;
    private GameObject _owner;
    private TooltipController _tooltipController;
    private TooltipPosition _tooltipPosition;

    public AttributeDataController(ICollection<string> attributes, VisualElement parent,
      AttributeData attributeData, AttributeData comparison, bool hideOldValue, int itemPerLine,
      bool withName, bool withIcon, List<Sprite> icons = null, GameObject owner = null,
      TooltipController tooltipController = null, TooltipPosition tooltipPosition = TooltipPosition.Right,
      List<Func<float, string>> beautify = null)
    {
      if (parent == null)
      {
        throw new Exception("parent cannot be null");
      }

      _attributes = attributes;
      _icons = icons;
      _beautify = beautify;
      _owner = owner;
      _tooltipController = tooltipController;
      _tooltipPosition = tooltipPosition;

      _container = new VisualElement();
      _container.AddToClassList("flex-row");

      int i;
      for (i = 0; i < itemPerLine; i++)
      {
        VisualElement col = new()
        {
          name = "AttributeCollumn"
        };
        col.AddToClassList("flex-col");
        _container.Add(col);
      }

      List<VisualElement> columns = _container.Children().ToList();

      i = 0;
      foreach (string name in attributes)
      {
        int colIndex = i % itemPerLine;

        VisualElement col = columns[colIndex];

        AttributeLine attributeLine;
        float value = attributeData.GetValue(i);
        Sprite icon = null;
        if (icons != null && i < icons.Count)
        {
          icon = icons[i];
        }
        if (comparison != null)
        {
          float comparisonValue = comparison.GetValue(i);
          if (Mathf.Approximately(value, 0) && Mathf.Approximately(comparisonValue, 0))
          {
            i++;
            continue;
          }

          string valueStr = null;
          AttributeChangeType changeType = AttributeLine.GetAttributeChangeType(value, comparisonValue);
          if (changeType != AttributeChangeType.NEUTRAL)
          {
            valueStr = ValueToString(i, value);
          }

          attributeLine = new AttributeLine(i, name, icon, ValueToString(i, comparisonValue), valueStr, changeType, hideOldValue);
        }
        else
        {
          if (Mathf.Approximately(value, 0))
          {
            i++;
            continue;
          }
          attributeLine = new AttributeLine(i, name, icon, ValueToString(i, value));
        }

        AttributeLineController controller = new AttributeLineController(col, withName, withIcon);
        controller.SetData(attributeLine, owner, tooltipController, tooltipPosition);

        _controllers.Add(i, controller);
        i++;
      }

      parent.Add(_container);
    }

    public AttributeLineController GetAttributeLineController(int type)
    {
      return _controllers[type];
    }
    public void Update(AttributeData attributeData)
    {
      for (int i = 0; i < attributeData.Count; i++)
      {
        float value = attributeData.GetValue(i);
        _controllers[i].SetOldValue(ValueToString(i, value));
      }
    }

    public void Hide()
    {
      _container.style.display = DisplayStyle.None;
    }
    public void Show()
    {
      _container.style.display = DisplayStyle.Flex;
    }

    private string ValueToString(int index, float value)
    {
      if (_beautify != null)
      {
        if (index < _beautify.Count)
        {
          return _beautify[index](value);
        }
      }

      return value.ToString();
    }

    public void SetComparison(AttributeData attributeData, AttributeData comparison)
    {
      int i = 0;
      foreach (string name in _attributes)
      {
        AttributeLineController controller = _controllers[i];

        float value = attributeData.GetValue(i);
        float comparisonValue = comparison.GetValue(i);
        if (Mathf.Approximately(value, 0) && Mathf.Approximately(comparisonValue, 0))
        {
          i++;
          continue;
        }

        string valueStr = null;
        AttributeChangeType changeType = AttributeLine.GetAttributeChangeType(value, comparisonValue);
        if (changeType != AttributeChangeType.NEUTRAL)
        {
          valueStr = ValueToString(i, value);
        }

        AttributeLine attributeLine = new AttributeLine(i, name, _icons[i], ValueToString(i, comparisonValue), valueStr, changeType, false);

        controller.SetData(attributeLine, _owner, _tooltipController, _tooltipPosition);

        i++;
      }
    }

    public void HideNewValue()
    {
      foreach (AttributeLineController controller in _controllers.Values)
      {
        controller.HideNewValue();
      }
    }
  }
}