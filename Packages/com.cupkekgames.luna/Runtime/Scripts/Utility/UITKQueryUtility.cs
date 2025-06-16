using UnityEngine.UIElements;

namespace CupkekGames.Luna
{
    public static class UITKQueryUtility
    {
        /// <summary>
        /// Finds the closest parent element with the specified class name.
        /// </summary>
        /// <param name="element">The element to start searching from.</param>
        /// <returns>The parent element with the specified class name, or null if not found.</returns>
        public static T FindParent<T>(VisualElement element) where T : VisualElement
        {
            var current = element.parent;
            while (current != null)
            {
                if (current is T typedElement)
                {
                    return typedElement;
                }
                current = current.parent;
            }
            return null;
        }

        public static T FindParentWithClass<T>(VisualElement element, string className) where T : VisualElement
        {
            var current = element.parent;
            while (current != null)
            {
                if (current.ClassListContains(className) && current is T typedElement)
                {
                    return typedElement;
                }
                current = current.parent;
            }
            return null;
        }
        public static VisualElement FindParentWithClass(VisualElement element, string className)
        {
            var current = element.parent;
            while (current != null)
            {
                if (current.ClassListContains(className))
                {
                    return current;
                }
                current = current.parent;
            }
            return null;
        }
        public static VisualElement FindParentWithName(VisualElement element, string name)
        {
            var current = element.parent;
            while (current != null)
            {
                if (current.name == name)
                {
                    return current;
                }
                current = current.parent;
            }
            return null;
        }
    }
} 