using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    [UxmlElement]
    public partial class RadialProgressBar : VisualElement
    {
        [UxmlAttribute] public bool Instant { get; set; } = true;
        [UxmlAttribute] public long UpdateFrequency { get; private set; } = 10;
        [UxmlAttribute] public float Step { get; private set; } = 0.01f;
        [UxmlAttribute] public long FirstDelay { get; private set; } = 0;
        public long _secondDelay = 500;
        [UxmlAttribute]
        public long SecondDelay
        {

            get
            {
                return _secondDelay;
            }
            set
            {
                _secondDelay = value;
            }
        }
        public float _thickness = 16f;
        [UxmlAttribute]
        public float Thickness
        {

            get
            {
                return _thickness;
            }
            set
            {
                _thickness = value;
                MarkDirtyRepaint();
            }
        }
        public Color _backgroundColor = Color.gray;
        [UxmlAttribute]
        public Color BackgroundColor
        {

            get
            {
                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;
                MarkDirtyRepaint();
            }
        }
        public Color _indicatorPositiveColor = Color.green;
        [UxmlAttribute]
        public Color IndicatorPositiveColor
        {
            get
            {
                return _indicatorPositiveColor;
            }
            set
            {
                _indicatorPositiveColor = value;
                MarkDirtyRepaint();
            }
        }
        public Color _indicatorNegativeColor = Color.red;
        [UxmlAttribute]
        public Color IndicatorNegativeColor
        {
            get
            {
                return _indicatorNegativeColor;
            }
            set
            {
                _indicatorNegativeColor = value;
                MarkDirtyRepaint();
            }
        }
        public Color _progressColor = Color.black;
        [UxmlAttribute]
        public Color ProgressColor
        {
            get
            {
                return _progressColor;
            }
            set
            {
                _progressColor = value;
                MarkDirtyRepaint();
            }
        }

        private float _currentValue = 0.5f;
        public float CurrentValue => _currentValue;
        private float _targetValue;
        [UxmlAttribute]
        public float TargetValue
        {
            get
            {
                return _targetValue;
            }
            set
            {
                if (float.IsNaN(value))
                {
                    _targetValue = 0;
                }
                else
                {
                    _targetValue = Mathf.Clamp(value, 0, 1);
                }

                if (Instant)
                {
                    _currentValue = _targetValue;
                    MarkDirtyRepaint();
                }
                else
                {
                    // if new value is less than current value, progress goes first
                    bool first = value < _currentValue;
                    AnimateProgress(first);
                }
            }
        }
        private Color _indicatorColor;
        private float _currentIndicator = -1;
        private float _targetIndicator; // Target indicator
        [UxmlAttribute]
        public float TargetIndicator
        {
            get
            {
                return _targetIndicator;
            }
            set
            {
                if (float.IsNaN(value))
                {
                    _targetIndicator = 0;
                }
                else
                {
                    _targetIndicator = Mathf.Clamp(value, 0, 1);
                }

                if (Instant)
                {
                    _currentIndicator = _targetIndicator;
                    MarkDirtyRepaint();
                }
                else
                {
                    // if new value is greater than current value, progress goes first
                    bool first = value > _currentIndicator;
                    AnimateIndicator(first);
                }
            }
        }
        private const string _ussClassName = "radial-progress";
        private CustomStyleProperty<Color> s_BgColor = new CustomStyleProperty<Color>("--radial-progress-bg-color");
        private CustomStyleProperty<Color> s_ProgressColor = new CustomStyleProperty<Color>("--radial-progress-progress-color");
        private CustomStyleProperty<Color> s_IndicatorColorPositive = new CustomStyleProperty<Color>("--radial-progress-indicator-positive-color");
        private CustomStyleProperty<Color> s_IndicatorColorNegative = new CustomStyleProperty<Color>("--radial-progress-indicator-negative-color");
        private IVisualElementScheduledItem _taskValue;
        private IVisualElementScheduledItem _taskIndicator;
        // Events
        public event Action OnValueReachTarget;

        // ------------------------------------------------------------------------------------------------------------

        public RadialProgressBar()
        {
            AddToClassList(_ussClassName);
            pickingMode = PickingMode.Ignore;

            SetIndicatorStyle(true);

            // These values update a visual element on set, so trigger them here
            _currentIndicator = _targetIndicator;
            _currentValue = _targetValue;

            TargetIndicator = _targetIndicator;
            TargetValue = _targetValue;

            generateVisualContent += GenerateVisualContent;

            RegisterCallback<CustomStyleResolvedEvent>(CustomStylesResolved);
        }

        private static void CustomStylesResolved(CustomStyleResolvedEvent evt)
        {
            RadialProgressBar element = (RadialProgressBar)evt.currentTarget;
            element.UpdateCustomStyles();
        }
        void UpdateCustomStyles()
        {
            bool repaint = false;
            if (customStyle.TryGetValue(s_BgColor, out _backgroundColor))
                repaint = true;

            if (customStyle.TryGetValue(s_ProgressColor, out _progressColor))
                repaint = true;

            if (customStyle.TryGetValue(s_IndicatorColorPositive, out _indicatorPositiveColor))
                repaint = true;

            if (customStyle.TryGetValue(s_IndicatorColorNegative, out _indicatorNegativeColor))
                repaint = true;

            if (repaint)
                MarkDirtyRepaint();
        }
        private void AnimateProgress(bool first)
        {
            long delay = first ? FirstDelay : SecondDelay;

            if (UpdateFrequency > 0.0f)
            {
                _taskValue?.Pause();

                float step = _currentValue > _targetValue ? -Step : Step;
                float clampMin = step > 0 ? 0 : _targetValue;
                float clampMax = step > 0 ? _targetValue : 1;

                if (!Mathf.Approximately(_currentValue, _targetValue))
                {
                    _taskValue = this.schedule
                        .Execute(() =>
                        {
                            float distance = Mathf.Abs(_currentValue - _targetValue);
                            float dynamicStep = Mathf.Lerp(step, step * 8, distance);
                            _currentValue += dynamicStep;
                            _currentValue = Mathf.Clamp(_currentValue, clampMin, clampMax);  // Clamp between 0 and 1 as percentages
                            MarkDirtyRepaint();
                        })
                        .Every(UpdateFrequency)
                        .Until(() =>
                          {
                              // Check if the condition to stop the task is met
                              bool isCompleted = step > 0 ? _currentValue >= _targetValue : _currentValue <= _targetValue;

                              if (isCompleted)
                              {
                                  // Fire the completion event if set
                                  OnValueReachTarget?.Invoke();
                              }

                              return isCompleted;
                          })
                        .StartingIn(delay);
                }
                else
                {
                    MarkDirtyRepaint();
                }
            }
            else
            {
                MarkDirtyRepaint();
            }
        }

        private void AnimateIndicator(bool first)
        {
            if (_targetIndicator > _currentIndicator)
            {
                SetIndicatorStyle(true);
            }
            else
            {
                SetIndicatorStyle(false);
            }

            long delay = first ? FirstDelay : SecondDelay;

            if (UpdateFrequency > 0.0f)
            {
                _taskIndicator?.Pause();

                float step = _currentIndicator > _targetIndicator ? -Step : Step;
                float clampMin = step > 0 ? 0 : _targetIndicator;
                float clampMax = step > 0 ? _targetIndicator : 1;

                if (!Mathf.Approximately(_currentIndicator, _targetIndicator))
                {
                    _taskIndicator = this.schedule
                        .Execute(() =>
                        {
                            float distance = Mathf.Abs(_currentIndicator - _targetIndicator);
                            float dynamicStep = Mathf.Lerp(step, step * 8, distance);
                            _currentIndicator += dynamicStep;
                            _currentIndicator = Mathf.Clamp(_currentIndicator, clampMin, clampMax);  // Clamp between 0 and 1 as percentages
                            MarkDirtyRepaint();
                        })
                        .Every(UpdateFrequency)
                        .Until(() => step > 0 ? _currentIndicator >= _targetIndicator : _currentIndicator <= _targetIndicator)
                        .StartingIn(delay);
                }
                else
                {
                    MarkDirtyRepaint();
                }
            }
            else
            {
                MarkDirtyRepaint();
            }
        }

        public void SetIndicatorStyle(bool positive)
        {
            if (positive)
            {
                _indicatorColor = IndicatorPositiveColor;
            }
            else
            {
                _indicatorColor = IndicatorNegativeColor;
            }
        }

        public void SetFilling(float current, float add, float max)
        {
            float next = current + add;
            if (next > max)
            {
                next = max;
            }
            // string sign = "";
            if (add >= 0)
            {
                // sign = "+";
                TargetValue = current / max;
                TargetIndicator = next / max;
                SetIndicatorStyle(true);
            }
            else
            {
                TargetIndicator = current / max;
                TargetValue = next / max;
                SetIndicatorStyle(false);
            }

            // Title = next + " (" + sign + add + ")";
        }

        private void GenerateVisualContent(MeshGenerationContext context)
        {
            float width = contentRect.width;
            float height = contentRect.height;

            var painter = context.painter2D;
            painter.lineWidth = Thickness;
            painter.lineCap = LineCap.Round;

            // Draw the track
            painter.strokeColor = BackgroundColor;
            painter.BeginPath();
            painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.5f, 0.0f, 360.0f);
            painter.Stroke();

            painter.lineWidth = Thickness + 1.0f;

            // Draw the indicator
            painter.strokeColor = _indicatorColor;
            painter.BeginPath();
            painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.5f, 0, 360.0f * _currentIndicator);
            painter.Stroke();

            painter.lineWidth = Thickness + 2.0f;

            // Draw the progress
            painter.strokeColor = ProgressColor;
            painter.BeginPath();
            painter.Arc(new Vector2(width * 0.5f, height * 0.5f), width * 0.5f, 0, 360.0f * _currentValue);
            painter.Stroke();
        }
    }
}
