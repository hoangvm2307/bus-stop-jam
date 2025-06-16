using System;
using System.Globalization;
using UnityEditor.UIElements;
using UnityEngine;

namespace CupkekGames.Luna.Editor
{
    public class ProgressBarNodeConverter : UxmlAttributeConverter<ProgressBarNode>
    {
        public override ProgressBarNode FromString(string value)
        {
            // Split using a | so that comma (,) can be used by the list.
            var split = value.Split('|');

            if (!ColorUtility.TryParseHtmlString("#" + split[2], out Color parsedColor))
            {
                parsedColor = Color.white;
            }

            return new ProgressBarNode
            {
                CurrentValue = float.Parse(split[0], CultureInfo.InvariantCulture),
                TargetValue = float.Parse(split[1], CultureInfo.InvariantCulture),
                Color = parsedColor
            };
        }

        public override string ToString(ProgressBarNode value)
        {
            return FormattableString.Invariant($"{value.CurrentValue}|{value.TargetValue}|{ColorUtility.ToHtmlStringRGBA(value.Color)}|");
        }
    }
}