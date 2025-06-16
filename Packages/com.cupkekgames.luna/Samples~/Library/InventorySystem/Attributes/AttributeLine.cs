using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Core;

namespace CupkekGames.InventorySystem
{
  public struct AttributeLine
  {
    public int Key;
    public string Name;
    public Sprite Icon;
    public string OldValue;
    public string NewValue;
    public AttributeChangeType ChangeType;
    public bool HideOldValue;
    public AttributeLine(int key, string name, Sprite icon, string oldValue, string newValue, AttributeChangeType changeType, bool hideOldValue)
    {
      Key = key;
      Name = name;
      Icon = icon;
      OldValue = oldValue;
      NewValue = newValue;
      ChangeType = changeType;
      HideOldValue = hideOldValue;
    }
    public AttributeLine(int key, string name, Sprite icon, string oldValue)
    {
      Key = key;
      Name = name;
      Icon = icon;
      OldValue = oldValue;
      NewValue = null;
      ChangeType = AttributeChangeType.NEUTRAL;
      HideOldValue = false;
    }

    public static AttributeChangeType GetAttributeChangeType(float value, float comparisonValue)
    {
      AttributeChangeType result = AttributeChangeType.DECREASE;

      if (Mathf.Approximately(value, comparisonValue))
      {
        result = AttributeChangeType.NEUTRAL;
      }
      else if (value > comparisonValue)
      {
        result = AttributeChangeType.INCREASE;
      }

      return result;
    }
  }
}