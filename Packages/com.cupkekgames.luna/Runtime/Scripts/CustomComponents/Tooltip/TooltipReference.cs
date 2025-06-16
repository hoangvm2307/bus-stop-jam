using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  public class TooltipReference : MonoBehaviour
  {
    private HashSet<Tooltip> _tooltips = new();

    private void OnDisable()
    {
      CloseAll();
    }

    public void Add(Tooltip tooltip)
    {
      _tooltips.Add(tooltip);
    }
    public void CloseAll()
    {
      foreach (Tooltip tooltip in _tooltips)
      {
        if (tooltip == null)
        {
          continue;
        }

        tooltip.CloseAll();
      }
    }
  }
}
