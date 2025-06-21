using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.Luna;

namespace CupkekGames.InventorySystem
{
    public class InventoryItemSlotController
    {
        // References
        protected InventoryItemDatabase _itemDatabase;
        // Fields
        protected int _index = -1;
        protected bool _selected;
        protected InventoryItem _item;
        protected InventoryItemDefinition _itemDefinition;
        protected GameObject _owner;
        protected TooltipController _tooltipController;
        protected TooltipManipulator _tooltipManipulator;
        protected ItemDragAndDrop _dragAndDrop;
        protected ICollection<string> _attributeNames;
        protected Sprite _emptyIcon;
        protected TooltipManipulator _emptyTooltipManipulator;

        // Properties
        public int Index => _index;
        public bool Selected => _selected;
        public InventoryItem Item => _item;
        public InventoryItemDefinition ItemDefinition => _itemDefinition;
        public bool IsEmpty => _item == null;

        // UI Elements
        public VisualElement Parent;
        public Button Background;
        public Label TopLeft;
        public Label TopRight;
        public Label BottomLeft;
        public Label BottomRight;
        protected TooltipPosition _tooltipPosition;

        // Events
        public event Action<InventoryItemSlotController> OnClick;

        // Constructor
        public InventoryItemSlotController(InventoryItemDatabase itemDatabase, GameObject owner, VisualElement parent, TooltipController tooltipController,
            TooltipPosition tooltipPosition)
        {
            _itemDatabase = itemDatabase;
            _owner = owner;
            _tooltipPosition = tooltipPosition;

            ResetVisualElements(parent);

            _tooltipController = tooltipController;
        }

        public virtual void ResetVisualElements(VisualElement parent)
        {
            UnbindItem();

            if (Background != null)
            {
                Background.clicked -= OnButtonClick;
            }

            Parent = parent;

            if (Parent == null)
            {
                return;
            }

            Background = Parent.Q<Button>("Background");
            Background.AddToClassList(_tooltipPosition.GetUssClass());

            // Add event listener
            Background.clicked += OnButtonClick;

            TopLeft = Parent.Q<Label>("TopLeft");
            TopRight = Parent.Q<Label>("TopRight");
            BottomLeft = Parent.Q<Label>("BottomLeft");
            BottomRight = Parent.Q<Label>("BottomRight");

            ClearItemDisplay();
        }

        public void SetAttributeNames(ICollection<string> attributeNames)
        {
            _attributeNames = attributeNames;
        }
        public void SetEmptyIcon(Sprite icon)
        {
            _emptyIcon = icon;

            if (IsEmpty)
            {
                Background.style.backgroundImage = icon != null ? new StyleBackground(icon) : null;
            }
        }
        public void SetEmptyTooltip(TooltipManipulator tooltipManipulator)
        {
            _emptyTooltipManipulator = tooltipManipulator;

            if (_emptyTooltipManipulator != null && IsEmpty)
            {
                Background.AddManipulator(_emptyTooltipManipulator);
            }
        }
        // Binds an item to the slot
        public virtual bool BindItem(InventoryItem item, InventoryItemDefinition itemDefinition, int index, bool selected,
            ItemDragAndDrop dragAndDrop, Func<InventoryItem, InventoryItemDefinition, AttributeData> getComparison,
            Func<List<TooltipContainerSetup>> getSetups)
        {
            _item = item;
            _selected = selected;
            _itemDefinition = itemDefinition;
            _index = index;
            _dragAndDrop = dragAndDrop;

            // Update item display
            UpdateItemDisplay();

            // Update tooltip
            UpdateTooltip(getComparison, getSetups);

            if (_dragAndDrop != null)
            {
                Background.AddManipulator(_dragAndDrop);
            }

            return true;
        }

        // Unbinds the current item from the slot
        public virtual void UnbindItem()
        {
            _item = null;
            _itemDefinition = null;
            _index = -1;
            _selected = false;

            // Remove tooltip and drag handlers
            RemoveManipulators();
            if (_emptyTooltipManipulator != null && Background != null)
            {
                Background.AddManipulator(_emptyTooltipManipulator);
            }

            // Clear UI display
            ClearItemDisplay();
        }

        // Event handler for button click
        protected void OnButtonClick()
        {
            if (_tooltipManipulator != null)
            {
                _tooltipManipulator.Close();
            }

            OnClick?.Invoke(this);

            if (_tooltipManipulator != null && !IsEmpty)
            {
                _tooltipManipulator.Show();
            }
        }

        // Configures the tooltip setup
        public TooltipContainerSetup GetTooltipSetup(Func<InventoryItem, InventoryItemDefinition, AttributeData> getComparison)
        {
            AttributeData comparison = getComparison?.Invoke(_item, _itemDefinition);

            Label tooltipName = new Label(_item.DisplayName(_itemDefinition));
            Label tooltipDescription = new Label(_itemDefinition.Description);
            VisualElement tooltipIcon = new() { style = { backgroundImage = Background.style.backgroundImage } };
            VisualElement bottom = _item.TooltipBottom(_itemDatabase, _attributeNames, getComparison);

            return new TooltipContainerSetup(tooltipIcon, tooltipName, tooltipDescription, bottom);
        }

        // Updates the UI labels and background for the item
        public virtual void UpdateItemDisplay()
        {
            Background.style.backgroundImage = _itemDefinition.Icon != null ? new StyleBackground(_itemDefinition.Icon) : null;
            if (BottomRight != null)
            {
                BottomRight.text = _item.Amount > 1 ? "x" + _item.Amount : "";
            }

            Background.RemoveFromClassList("selected");
            if (_selected)
            {
                Background.AddToClassList("selected");
            }
        }

        public virtual void ClearItemDisplay()
        {
            if (Background != null)
            {
                Background.style.backgroundImage = _emptyIcon != null ? new StyleBackground(_emptyIcon) : null;
            }
            ClearLabels();
        }

        // Configures tooltip with comparison and setup data
        protected void UpdateTooltip(Func<InventoryItem, InventoryItemDefinition, AttributeData> getComparison,
            Func<List<TooltipContainerSetup>> getSetups)
        {
            RemoveTooltipManipulator();

            if (_tooltipController != null)
            {
                _tooltipManipulator = new TooltipManipulator(_owner, _tooltipController);
                _tooltipManipulator.SetSetup(GetTooltipSetup(getComparison));
                _tooltipManipulator.SetSetupProviders(getSetups);

                Background.AddManipulator(_tooltipManipulator);
            }
        }

        // Removes existing manipulators
        protected void RemoveManipulators()
        {
            RemoveTooltipManipulator();
            RemoveDragManipulator();
        }

        // Removes tooltip manipulator
        protected void RemoveTooltipManipulator()
        {
            if (_tooltipManipulator != null)
            {
                _tooltipManipulator.Close();
                Background.RemoveManipulator(_tooltipManipulator);
            }
            if (_emptyTooltipManipulator != null)
            {
                _emptyTooltipManipulator.Close();
                Background.RemoveManipulator(_emptyTooltipManipulator);
            }
        }

        // Removes drag manipulator
        protected void RemoveDragManipulator()
        {
            if (_dragAndDrop != null)
            {
                Background.RemoveManipulator(_dragAndDrop);
            }
        }

        // Clears text from all labels
        protected void ClearLabels()
        {
            if (TopLeft != null)
            {
                TopLeft.text = "";
            }
            if (TopRight != null)
            {
                TopRight.text = "";
            }
            if (BottomLeft != null)
            {
                BottomLeft.text = "";
            }
            if (BottomRight != null)
            {
                BottomRight.text = "";
            }
        }

        public void ReopenTooltip()
        {
            if (_tooltipManipulator != null)
            {
                _tooltipManipulator.Close();

                if (!IsEmpty)
                {
                    _tooltipManipulator.Show();
                }
            }
        }

        public void SetTooltipPosition(TooltipPosition tooltipPosition)
        {
            Background.RemoveFromClassList(_tooltipPosition.GetUssClass());
            Background.AddToClassList(tooltipPosition.GetUssClass());
            _tooltipPosition = tooltipPosition;
        }
    }
}
