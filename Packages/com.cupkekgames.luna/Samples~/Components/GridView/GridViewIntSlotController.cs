using System;
using System.Collections.Generic;
using CupkekGames.Luna;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Systems;

namespace CupkekGames.Luna.Demo.Components
{
  public class GridViewIntSlotController
  {
    private VisualElement _parent;
    private Label _label;
    
    public GridViewIntSlotController(VisualElement parent)
    {
      _parent = parent;
      _label = _parent.Q<Label>();
    }

    public void BindItem(int item, int index)
    {
      _label.text = $"i: {index}, v: {item}";
    }

    public void UnbindItem()
    {
      _label.text = "";
    }
  }
}