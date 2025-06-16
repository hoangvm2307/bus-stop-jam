#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace CupkekGames.Core.Editor
{
    [CustomPropertyDrawer(typeof(MultiLineHeaderAttribute))]
    public class MultiLineHeaderDrawer : DecoratorDrawer
    {
        private Color backgroundColor = new Color(0f, 0f, 0f, 0.2f);
        private Color borderColor = new Color(0f, 0f, 0f, 0.4f);
        private float margin = 5f;
        private float cachedHeight;

        public override void OnGUI(Rect position)
        {
            MultiLineHeaderAttribute header = (MultiLineHeaderAttribute)attribute;

            GUIStyle style = new GUIStyle(EditorStyles.label)
            {
                wordWrap = true,
                padding = new RectOffset(10, 10, 5, 5)
            };

            // Adjust position to include margin
            Rect marginRect = new Rect(
                position.x + margin,
                position.y + margin,
                position.width - 2 * margin,
                position.height - 2 * margin
            );

            DrawBackgroundWithBorderRadius(marginRect);
            EditorGUI.LabelField(marginRect, header.headerText, style);

            // Cache the height calculation here, so GetHeight can use it
            cachedHeight = style.CalcHeight(new GUIContent(header.headerText), position.width - 2 * margin)
                           + style.padding.top + style.padding.bottom + 2 * margin;
        }

        public override float GetHeight()
        {
            return cachedHeight;
        }

        private void DrawBackgroundWithBorderRadius(Rect position)
        {
            Rect adjustedRect = new Rect(position.x, position.y, position.width, position.height);

            EditorGUI.DrawRect(adjustedRect, backgroundColor);
            Handles.color = borderColor;
            Handles.DrawSolidRectangleWithOutline(adjustedRect, Color.clear, borderColor);
        }
    }
}
#endif
