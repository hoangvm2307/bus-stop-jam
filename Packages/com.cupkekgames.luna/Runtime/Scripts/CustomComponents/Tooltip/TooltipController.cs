using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  [RequireComponent(typeof(UIDocument))]
  public class TooltipController : MonoBehaviour
  {
    protected UIDocument _uiDocument;
    protected Tooltip _tooltip;
    public Tooltip Tooltip => _tooltip;

    protected virtual void Awake()
    {
      _uiDocument = GetComponent<UIDocument>();

      _tooltip = _uiDocument.rootVisualElement.Q<Tooltip>();
    }

    protected virtual void OnEnable()
    {
      _tooltip.OnEnable(this);
    }

    protected virtual void OnDisable()
    {
      _tooltip.OnDisable();
    }
  }
}
