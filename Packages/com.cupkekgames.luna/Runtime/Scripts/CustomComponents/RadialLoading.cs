using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{

    // An element that displays progress inside a partially filled circle
    [UxmlElement]
    public partial class RadialLoading : VisualElement
    {
        private float _arcSize = 0.2f;
        [UxmlAttribute]
        public float ArcSize
        {
            get { return _arcSize; }
            set
            {
                _taskArcSize?.Pause();

                _taskArcSize = schedule
                .Execute(() =>
                {
                    float distance = Mathf.Abs(_arcSize - value);
                    _arcSize += Mathf.Lerp(0.01f, 0.1f, distance);
                    MarkDirtyRepaint();
                })
                .Every(UpdateFrequency)
                .Until(() =>
                {
                    // Check if the condition to stop the task is met
                    bool isCompleted = _arcSize >= value;

                    if (isCompleted)
                    {
                        _arcSize = value;
                    }

                    return isCompleted;
                });
            }
        }
        private IVisualElementScheduledItem _taskArcSize;
        [UxmlAttribute] public float LineWidth { get; private set; } = 10f;
        [UxmlAttribute] public float Step { get; private set; } = 0.05f;
        private long _updateFrequency = 10;
        [UxmlAttribute]
        public long UpdateFrequency
        {
            get
            {
                return _updateFrequency;
            }
            private set
            {
                _updateFrequency = value;

                StartAnimation();
            }
        }

        // These are USS class names for the control overall and the label.
        public static readonly string ussClassName = "radial-loading";

        // These objects allow C# code to access custom USS properties.
        static CustomStyleProperty<Color> s_TrackColor = new CustomStyleProperty<Color>("--radial-loading-track-color");
        static CustomStyleProperty<Color> s_ProgressColor = new CustomStyleProperty<Color>("--radial-loading-line-color");

        private Color _trackColor = Color.gray;
        private Color _progressColor = Color.red;
        private Color? _trackColorOverride = null;
        public Color? TrackColorOverride
        {
            get { return _trackColorOverride; }
            set
            {
                _trackColorOverride = value;
                if (value.HasValue)
                {
                    _trackColor = value.Value;
                }
                MarkDirtyRepaint();
            }
        }
        private Color? _progressColorOverride = null;
        public Color? ProgressColorOverride
        {
            get { return _progressColorOverride; }
            set
            {
                _progressColorOverride = value;
                if (value.HasValue)
                {
                    _progressColor = value.Value;
                }
                MarkDirtyRepaint();
            }
        }

        // This is the number that the Label displays as a percentage.
        private float _offset;
        private IVisualElementScheduledItem _task;

        // This default constructor is RadialProgress's only constructor.
        public RadialLoading()
        {
            // Add the USS class name for the overall control.
            AddToClassList(ussClassName);

            // Register a callback after custom style resolution.
            RegisterCallback<CustomStyleResolvedEvent>(evt => CustomStylesResolved(evt));

            // Register a callback to generate the visual content of the control.
            generateVisualContent += GenerateVisualContent;

            _offset = 0.0f;

            StartAnimation();
        }

        static void CustomStylesResolved(CustomStyleResolvedEvent evt)
        {
            RadialLoading element = (RadialLoading)evt.currentTarget;
            element.UpdateCustomStyles();
        }

        // After the custom colors are resolved, this method uses them to color the meshes and (if necessary) repaint
        // the control.
        void UpdateCustomStyles()
        {
            bool repaint = false;

            if (ProgressColorOverride.HasValue)
            {
                _progressColor = ProgressColorOverride.Value;
            }
            else if (customStyle.TryGetValue(s_ProgressColor, out _progressColor))
            {
                repaint = true;
            }

            if (TrackColorOverride.HasValue)
            {
                _trackColor = TrackColorOverride.Value;
            }
            else if (customStyle.TryGetValue(s_TrackColor, out _trackColor))
            {
                repaint = true;
            }

            if (repaint)
            {
                MarkDirtyRepaint();
            }
        }

        void GenerateVisualContent(MeshGenerationContext context)
        {
            Vector2 center = new Vector2(contentRect.width * 0.5f, contentRect.height * 0.5f);
            float width = contentRect.width - LineWidth;
            float height = contentRect.height - LineWidth;

            var painter = context.painter2D;
            painter.lineWidth = LineWidth;
            painter.lineCap = LineCap.Round;

            // Draw the track (full circle)
            painter.strokeColor = _trackColor;
            painter.BeginPath();
            painter.Arc(center, width * 0.5f, 0.0f, 360.0f);
            painter.Stroke();

            // Convert ArcSize from a percentage (0.1 = 10%) to degrees (0.1 * 360 = 36 degrees)
            float sizeDegrees;
            if (_arcSize >= 0.99f)
            {
                sizeDegrees = 359.9f; // Slightly less than 360 to avoid endpoint overlap
            }
            else
            {
                sizeDegrees = _arcSize * 360.0f;
            }
            float offsetDegrees;
            if (_offset >= 0.99f)
            {
                offsetDegrees = 359.9f; // Slightly less than 360 to avoid endpoint overlap
            }
            else
            {
                offsetDegrees = _offset * 360.0f;
            }

            painter.lineWidth = LineWidth + 1.0f;

            // Draw the fixed-size arc with offset
            painter.strokeColor = _progressColor;
            painter.BeginPath();
            painter.Arc(center, width * 0.5f, offsetDegrees, sizeDegrees + offsetDegrees);
            painter.Stroke();
        }

        public void StartAnimation()
        {
            _task?.Pause();

            _task = schedule
              .Execute(() =>
              {
                  _offset += Step;

                  if (_offset > 1f)
                  {
                      _offset = 0;
                  }

                  MarkDirtyRepaint();
              })
              .Every(UpdateFrequency);
        }

        public void StopAnimation()
        {
            _task?.Pause();
            MarkDirtyRepaint();
        }
    }
}