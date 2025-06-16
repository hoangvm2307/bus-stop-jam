using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
  [UxmlElement]
  public partial class CooldownWipe : VisualElement
  {
    private float _progress = 0f;
    [UxmlAttribute]
    public float Progress
    {
      get => _progress;
      set { _progress = Mathf.Clamp01(value); MarkDirtyRepaint(); }
    }

    private Color _fillColor = new Color(0, 0, 0, 1f);
    [UxmlAttribute]
    public Color FillColor
    {
      get => _fillColor;
      set { _fillColor = value; MarkDirtyRepaint(); }
    }
    private int _segmentAmount = 20;
    [UxmlAttribute]
    public int SegmentAmount
    {
      get => _segmentAmount;
      set { _segmentAmount = value; MarkDirtyRepaint(); }
    }

    private bool _reverseWinding = false;
    [UxmlAttribute]
    public bool ReverseWinding
    {
      get => _reverseWinding;
      set { _reverseWinding = value; MarkDirtyRepaint(); }
    }
    private float _lineWidth = 0f;
    [UxmlAttribute]
    public float LineWidth
    {
      get => _lineWidth;
      set { _lineWidth = value; MarkDirtyRepaint(); }
    }

    private Color _lineColor = new Color(0, 0, 0, 1f);
    [UxmlAttribute]
    public Color LineColor
    {
      get => _lineColor;
      set { _lineColor = value; MarkDirtyRepaint(); }
    }
    public CooldownWipe()
    {
      generateVisualContent += DrawCanvas;
    }

    void DrawCanvas(MeshGenerationContext ctx)
    {
      Painter2D painter = ctx.painter2D;
      painter.fillColor = _fillColor;

      // Adjusting the rectangle position based on contentRect
      Vector2 rectPos = contentRect.position;
      float rectWidth = contentRect.width;
      float rectHeight = contentRect.height;

      // Draw full square (outer rectangle) in clockwise order
      painter.BeginPath();
      painter.MoveTo(new Vector2(rectPos.x, rectPos.y));
      painter.LineTo(new Vector2(rectPos.x + rectWidth, rectPos.y));
      painter.LineTo(new Vector2(rectPos.x + rectWidth, rectPos.y + rectHeight));
      painter.LineTo(new Vector2(rectPos.x, rectPos.y + rectHeight));
      painter.ClosePath();

      if (_progress >= 1)
      {
        painter.Fill();
        return;
      }
      if (_progress <= 0)
        return;

      Vector2 center = contentRect.center;
      float startAngle = -90f;
      float sweepAngle = _reverseWinding ? _progress * -360f : _progress * 360f;
      float endAngle = startAngle + sweepAngle;

      int segments = Mathf.CeilToInt(_segmentAmount * _progress);
      float radius = Mathf.Max(contentRect.width, contentRect.height);

      painter.BeginPath();
      painter.MoveTo(center);

      for (int i = 0; i <= segments; i++)
      {
        float angle = Mathf.Lerp(startAngle, endAngle, (float)i / segments) * Mathf.Deg2Rad;
        Vector2 point = new Vector2(
          center.x + radius * Mathf.Cos(angle),
          center.y + radius * Mathf.Sin(angle)
        );
        painter.LineTo(point);
      }

      painter.ClosePath();
      painter.Fill();
      if (_lineWidth > 0)
      {
        painter.lineWidth = _lineWidth;
        painter.strokeColor = _lineColor;
        painter.Stroke();
      }
    }
  }
}
