using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  [UxmlElement]
  public partial class AspectRatioElement : VisualElement
  {
    private int _x = 1;
    [UxmlAttribute]
    public int X
    {
      get => _x;
      set { _x = value; UpdateSize(); }
    }
    private int _y = 1;
    [UxmlAttribute]
    public int Y
    {
      get => _y;
      set { _y = value; UpdateSize(); }
    }
    
    public AspectRatioElement()
    {
      RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
      RegisterCallback<DetachFromPanelEvent>(OnDetachFromPanel);
    }

    private void OnAttachToPanel(AttachToPanelEvent evt)
    {
      RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
      // Initial size update
      UpdateSize();
    }

    private void OnDetachFromPanel(DetachFromPanelEvent evt)
    {
      UnregisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
      UpdateSize();
    }
    
    private void UpdateSize()
    {
      if (parent == null)
        return;

      float parentWidth = parent.contentRect.width;
      float parentHeight = parent.contentRect.height;

      float aspectRatio = (float)X / (float)Y;

      if (aspectRatio > 1)
      {
        style.width = parentWidth;
        style.height = parentWidth / aspectRatio;
      }
      else
      {
        style.width = parentHeight * aspectRatio;
        style.height = parentHeight;
      }
    }
  }
}
