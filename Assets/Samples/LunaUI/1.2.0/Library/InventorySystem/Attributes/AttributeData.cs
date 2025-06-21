

using System;
using System.Collections.Generic;
using UnityEngine;

namespace CupkekGames.InventorySystem
{
    [Serializable]
    public class AttributeData
    {
        [SerializeField] protected List<float> _values = new();
        public int Count => _values.Count;
        public void Copy(AttributeData other)
        {
            _values = new List<float>(other._values);
        }
        public void Add(AttributeData other)
        {
            for (int i = 0; i < other.Count; i++)
            {
                if (i >= _values.Count)
                {
                    _values.Add(other._values[i]);
                }
                else
                {
                    _values[i] += other._values[i];
                }
            }
        }
        public void Remove(AttributeData other)
        {
            for (int i = 0; i < _values.Count; i++)
            {
                if (i >= other._values.Count)
                {
                    break;
                }

                _values[i] -= other._values[i];
            }
        }

        public float GetValue(int i)
        {
            if (i >= _values.Count)
            {
                return 0;
            }

            return _values[i];
        }
        public void SetValue(int i, float value)
        {
            while (i >= _values.Count)
            {
                _values.Add(0);
            }

            _values[i] = value;
        }
        public void AddValue(float value)
        {
            _values.Add(value);
        }
        public void Clear()
        {
            _values.Clear();
        }

        public void MultiplyAll(float value)
        {
            for (int i = 0; i < _values.Count; i++)
            {
                _values[i] *= value;
            }
        }
    }
}