using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  [UxmlElement]
  public partial class CircleHoleElement : VisualElement
  {
    private float _radius = 10f;

    [UxmlAttribute]
    public float Radius
    {
      get { return _radius; }
      set { _radius = value; MarkDirtyRepaint(); }
    }
    private int _circleSegments = 10;

    [UxmlAttribute]
    public int CircleSegments
    {
      get { return _circleSegments; }
      set { _circleSegments = value; MarkDirtyRepaint(); }
    }
    private Color _fillColor = new Color(0, 0, 0, 1f);
    [UxmlAttribute]
    public Color FillColor
    {
      get { return _fillColor; }
      set { _fillColor = value; MarkDirtyRepaint(); }
    }

    public CircleHoleElement()
    {
      generateVisualContent += DrawCanvas;
    }

    void DrawCanvas(MeshGenerationContext ctx)
    {
      Painter2D painter = ctx.painter2D;
      painter.fillColor = _fillColor; // Semi-transparent overlay

      painter.BeginPath();

      // Outer rectangle (drawn in clockwise order)
      painter.MoveTo(new Vector2(0, 0));
      painter.LineTo(new Vector2(contentRect.width, 0));
      painter.LineTo(new Vector2(contentRect.width, contentRect.height));
      painter.LineTo(new Vector2(0, contentRect.height));
      painter.ClosePath();

      if (_radius <= 0)
      {
        painter.Fill();
        return;
      }

      // Inner circle (drawn with reversed winding order)
      Vector2 center = contentRect.center;
      float angleStep = 360f / _circleSegments;
      // Start at the rightmost point of the circle
      painter.MoveTo(new Vector2(center.x + _radius, center.y));
      // Use negative angles to build the circle in reverse order
      for (int i = 1; i <= _circleSegments; i++)
      {
        float angle = -i * angleStep;
        float rad = angle * Mathf.Deg2Rad;
        Vector2 point = center + new Vector2(Mathf.Cos(rad) * _radius,
                                              Mathf.Sin(rad) * _radius);
        painter.LineTo(point);
      }
      painter.ClosePath();

      painter.Fill();
    }
  }
}