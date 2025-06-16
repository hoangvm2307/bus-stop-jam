using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Core;
using System.Collections;

namespace CupkekGames.Luna.Demo.Components
{
  public class ListViewDemo : ListViewController
  {
    [SerializeField] private List<string> _source;

    public override IList GetSourceList()
    {
      return _source;
    }

    public override void BindItem(VisualElement item, int index)
    {
      item.AddToClassList("line_item_container");
      item.Q<Label>().text = _source[index];
      item.Q<Label>().AddToClassList("line_item");
    }

    public override void UnbindItem(VisualElement item, int index)
    {

    }

    public override void OnSelectionChanged(IEnumerable<object> selectedItems)
    {

    }
  }
}
