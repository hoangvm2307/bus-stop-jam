using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    [System.Serializable]
    public class ProgressBarNode
    {
        [SerializeField] private float _currentValue;
        public float CurrentValue
        {
            get => _currentValue;
            set
            {
                _currentValue = value;
            }
        }
        [SerializeField] private float _targetValue;
        public float TargetValue
        {
            get => _targetValue;
            set
            {
                _targetValue = value;
                if (float.IsNaN(_targetValue))
                {
                    _targetValue = 0;
                }
                else
                {
                    _targetValue = Mathf.Clamp(_targetValue, 0, 1);
                }
            }
        }
        [SerializeField] private Color _color = new Color(1, 1, 1, 1);
        public Color Color
        {
            get => _color;
            set
            {
                _color = value;
            }
        }
        [NonSerialized] public VisualElement VisualElement;
        [NonSerialized] public IVisualElementScheduledItem ScheduledItem;

        public void CreateElement()
        {
            VisualElement = new VisualElement { pickingMode = PickingMode.Ignore, usageHints = UsageHints.DynamicTransform };
        }
    }
}