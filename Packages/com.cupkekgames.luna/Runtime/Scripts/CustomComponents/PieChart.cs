using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    [UxmlElement]
    public partial class PieChart : VisualElement
    {
        private List<float> _values = new List<float>();
        private List<Color> _colors = new List<Color>();

        [UxmlAttribute]
        public List<float> Values
        {
            get { return _values; }
            set { _values = value; MarkDirtyRepaint(); }
        }

        [UxmlAttribute]
        public List<Color> Colors
        {
            get { return _colors; }
            set { _colors = value; MarkDirtyRepaint(); }
        }

        public PieChart()
        {
            generateVisualContent += DrawCanvas;
        }
        [UxmlAttribute]
        public Color BackgroundColor { get; set; } = Color.white;
        [UxmlAttribute]
        public Color OutlineColor { get; set; } = Color.white;
        [UxmlAttribute]
        public float OutlineWidth { get; set; } = 10.0f;
        [UxmlAttribute]
        public float GapWidth { get; set; } = 0.1f;

        void DrawCanvas(MeshGenerationContext ctx)
        {
            Painter2D painter = ctx.painter2D;

            float radius = contentRect.width / 2f;

            painter.fillColor = BackgroundColor;
            painter.BeginPath();
            painter.MoveTo(new Vector2(radius, radius));
            painter.Arc(new Vector2(radius, radius), radius, 0, 360);
            painter.Fill();

            DrawPie(painter, radius);

            painter.strokeColor = OutlineColor;
            painter.lineWidth = OutlineWidth;
            painter.BeginPath();
            painter.Arc(new Vector2(radius, radius), radius, 0.0f, 360.0f);
            painter.Stroke();
        }

        private void DrawPie(Painter2D painter, float radius)
        {
            Vector2 center = new Vector2(radius, radius);

            float total = 0f;
            foreach (var val in _values)
            {
                total += val;
            }

            float angle = 0.0f;
            for (int i = 0; i < _values.Count; i++)
            {
                float pct = _values[i] / total;
                float anglePct = 360.0f * pct;

                float endAngle = angle + anglePct;

                if (i >= _colors.Count)
                {
                    painter.fillColor = Color.black;
                }
                else
                {
                    painter.fillColor = _colors[i];
                }

                painter.BeginPath();
                painter.MoveTo(center);
                painter.Arc(center, radius, angle, endAngle);
                painter.Fill();

                angle += anglePct;
            }

            if (GapWidth > 0)
            {
                angle = 0.0f;
                for (int i = 0; i < _values.Count; i++)
                {
                    float pct = _values[i] / total;
                    float anglePct = 360.0f * pct;

                    float endAngle = angle + anglePct;

                    Vector2 gapEnd = new Vector2(radius + Mathf.Cos(Mathf.Deg2Rad * endAngle) * radius,
                                            radius + Mathf.Sin(Mathf.Deg2Rad * endAngle) * radius);

                    painter.BeginPath();
                    painter.lineWidth = GapWidth;
                    painter.strokeColor = BackgroundColor;
                    painter.MoveTo(center); // Start at the center
                    painter.LineTo(gapEnd);
                    painter.Stroke(); // Stroke the gap line

                    angle += anglePct;
                }
            }
        }
    }
}