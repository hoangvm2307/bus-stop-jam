using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Demo.Components
{
    public class DragAndDropDemo : UIViewComponent
    {
        // Images to use as data of this demo
        [SerializeField] Sprite[] _images;

        protected override void Awake()
        {
            base.Awake();

            // Get drag starter elements
            List<VisualElement> dragStarters = ParentElement.Query<VisualElement>("DragStarter").ToList();
            // Get drop slot elements
            List<VisualElement> dropSlots = ParentElement.Query<VisualElement>("DropSlot").ToList();

            // Setup drag starters
            for (int i = 0; i < dragStarters.Count; i++)
            {
                VisualElement dragStarter = dragStarters[i];

                Sprite image = _images[i];
                dragStarter.style.backgroundImage = new StyleBackground(image);

                // Create manipulator
                DragAndDropManipulatorDemo manipulator = new DragAndDropManipulatorDemo(
                    null, // UIElementManager can be null. Refer to the documentation for its usage in DragAndDropManipulator.
                    ParentElement, // The drag area.
                    i, // Key identifier for the manipulator.
                    dropSlots, // Predefined drop slots.
                    null, // Optional dynamic drop slots, if needed.
                    image // Custom data specific to the demo.
                );

                // Register to event
                manipulator.ExampleOnDropEvent += OnDrop;

                // Create manipulator to dragStarter
                dragStarter.AddManipulator(manipulator);
            }
        }

        private void OnDrop(VisualElement dropSlot, Sprite _background)
        {
            // Change the dropSlot's background image
            dropSlot.style.backgroundImage = new StyleBackground(_background);
        }
    }
}