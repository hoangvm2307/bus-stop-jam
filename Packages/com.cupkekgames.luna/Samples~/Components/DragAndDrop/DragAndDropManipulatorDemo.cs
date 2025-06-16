using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
    public class DragAndDropManipulatorDemo : DragAndDropManipulator
    {
        // Custom Data
        private Sprite _background;

        // Custom OnDrop event
        // Demonstrates the use of DisposeDragElement
        // Parameters: drop slot and _background sprite
        public event Action<VisualElement, Sprite> ExampleOnDropEvent;

        public DragAndDropManipulatorDemo(
            LunaUIManager uiElementManager,
            VisualElement dragArea, int key,
            List<VisualElement> dropSlots,
            Func<List<VisualElement>> getDropSlots,
            Sprite background)
         : base(uiElementManager, dragArea, key, dropSlots, getDropSlots)
        {
            _background = background;
        }

        // Called when dragging starts
        public override VisualElement CreateDragElement()
        {
            // Create and style a new VisualElement for the drag operation
            VisualElement dragElement = new VisualElement();
            dragElement.AddToClassList("size-128");
            dragElement.AddToClassList("rounded-lg");
            dragElement.AddToClassList("bg-base-600");
            dragElement.AddToClassList("border-4");
            dragElement.AddToClassList("border-base-50");

            // Set background image
            dragElement.style.backgroundImage = new StyleBackground(_background);

            // Register custom event with the OnDrop event
            OnDrop += OnItemDropInner;

            return dragElement;
        }

        // Called when dragging ends
        public override void DisposeDragElement()
        {
            // Unregister custom event from the OnDrop event
            OnDrop -= OnItemDropInner;
        }

        private void OnItemDropInner(int key, int dropIndex, VisualElement dropSlot)
        {
            // Invoke the custom event, passing the drop slot and background sprite
            ExampleOnDropEvent?.Invoke(dropSlot, _background);
        }
    }
}