using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    [Serializable]
    public class UIColor
    {
        [SerializeField] public UIColorName Name;
        [SerializeField] public UIColorValue Value;

        public UIColor()
        {
            Name = UIColorName.SLATE;
            Value = UIColorValue.V_50;
        }

        public UIColor(UIColorName name, UIColorValue value)
        {
            Name = name;
            Value = value;
        }

        public string GetUssClassBG()
        {
            if (Name == UIColorName.TRANSPARENT)
            {
                return "bg-" + Name.ToString().ToLower();
            }

            return "bg-" + Name.ToString().ToLower() + "-" + Value.GetUSSClass();
        }

        public string GetUssClassBorder()
        {
            if (Name == UIColorName.TRANSPARENT)
            {
                return "border-" + Name.ToString().ToLower();
            }

            return "border-" + Name.ToString().ToLower() + "-" + Value.GetUSSClass();
        }

        public string GetUssClassText()
        {
            if (Name == UIColorName.TRANSPARENT)
            {
                return "text-" + Name.ToString().ToLower();
            }

            return "text-" + Name.ToString().ToLower() + "-" + Value.GetUSSClass();
        }

        public string GetUssClassImageTint()
        {
            if (Name == UIColorName.TRANSPARENT)
            {
                return "tint-" + Name.ToString().ToLower();
            }

            return "tint-" + Name.ToString().ToLower() + "-" + Value.GetUSSClass();
        }

        public Color GetColor(VisualElement element)
        {
            if (Name == UIColorName.TRANSPARENT)
            {
                return new Color();
            }

            var colorProperty = new CustomStyleProperty<Color>("--color-" + Name.ToString().ToLower() + "-" + Value.GetUSSClass());

            // Get the custom color
            if (element.customStyle.TryGetValue(colorProperty, out var value))
            {
                return value;
            }
            else
            {
                return new Color();
            }
        }
    }
}