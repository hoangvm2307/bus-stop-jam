using System;
using CupkekGames.InventorySystem;
using CupkekGames.Luna.Library;
using CupkekGames.Systems;
using UnityEngine.UIElements;

namespace CupkekGames.Luna.Mobile
{
    public class InventoryItemPopupController : ChoicePopupController
    {
        // Dependencies
        private InventoryItemDatabase _itemDatabase;

        // UI Elements
        private VisualElement _modal;
        private MobileInventoryItemSlotController _itemSlotController;
        private AttributeLineController _attributeLineController;
        protected Label _itemTitle;
        protected Label _itemStat;

        // State
        private EquipmentSlotController _equipmentSlot;
        public EquipmentSlotController EquipmentSlot => _equipmentSlot;
        private InventoryItemSlotController _invetorySlot;
        public InventoryItemSlotController InventorySlot => _invetorySlot;
        private bool _isEquiping;
        public bool IsEquiping => _isEquiping;

        protected override void Awake()
        {
            base.Awake();

            _modal = ParentElement.Q<VisualElement>("Modal");

            _itemDatabase = ServiceLocator.Get<InventoryItemDatabase>();

            _itemSlotController = new (_itemDatabase, gameObject, ParentElement, null, TooltipPosition.Right);

            _attributeLineController = new AttributeLineController(ParentElement.Q<VisualElement>("AttributeLine"), false, false);

            _itemTitle = ParentElement.Q<Label>("ItemTitle");
            _itemStat = ParentElement.Q<Label>("ItemStat");
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Fade.OnFadeOutStart += OnFadeOutStart;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            Fade.OnFadeOutStart -= OnFadeOutStart;
        }

        protected override void OnFadeInStart()
        {
            base.OnFadeInStart();

            _modal.AddToClassList("visible");
        }

        protected virtual void OnFadeOutStart()
        {
            base.OnFadeOut();

            _modal.RemoveFromClassList("visible");
        }

        public void SetEquiping(AttributeData playerAttributeData, EquipmentSlotController equipmentSlot, InventoryItemSlotController invetorySlot)
        {
            _isEquiping = true;

            _equipmentSlot = equipmentSlot;
            _invetorySlot = invetorySlot;

            InventoryItem oldItem = _equipmentSlot.Item;
            InventoryItem newItem = _invetorySlot.Item;

            InventoryItemDefinition newItemDefinition = _itemDatabase.GetItemDefinition(newItem);
            string header = newItemDefinition.Name;

            _itemSlotController.BindItem(newItem, newItemDefinition, -1, false, null, null, null);

            float currentValue = playerAttributeData.GetValue(0);
            _attributeLineController.SetOldValue(currentValue.ToString());

            float oldItemValue = 0f;
            if (oldItem is Equipment oldEquipment)
            {
                AttributeData oldEquipmentAttributeData = oldEquipment.AttributeData(_itemDatabase);
                oldItemValue = oldEquipmentAttributeData.GetValue(0);
            }

            float newItemValue = 0f;
            if (newItem is Equipment newEquipment)
            {
                AttributeData newEquipmentAttributeData = newEquipment.AttributeData(_itemDatabase);
                newItemValue = newEquipmentAttributeData.GetValue(0);

                header += $" (Lv.{newEquipment.Level})";
                _itemStat.text = newItemValue.ToString();
            }

            float newValue = currentValue - oldItemValue + newItemValue;
            var changeType = AttributeLine.GetAttributeChangeType(newValue, currentValue);
            _attributeLineController.SetNewValue(newValue.ToString(), changeType, false);

            _itemTitle.text = header;
            TextBody = newItemDefinition.Description;
            TextHeader = "Equip Item?";
        }

        public void SetUnequiping(AttributeData playerAttributeData, EquipmentSlotController equipmentSlot)
        {
            _isEquiping = false;

            _equipmentSlot = equipmentSlot;
            _invetorySlot = null;

            InventoryItem oldItem = _equipmentSlot.Item;
            if (oldItem == null)
            {
                // If nothing is equipped, clear UI
                _itemSlotController.BindItem(null, null, -1, false, null, null, null);
                _itemTitle.text = "None Equipped";
                _itemStat.text = "-";
                _attributeLineController.SetOldValue(playerAttributeData.GetValue(0).ToString());
                _attributeLineController.SetNewValue(playerAttributeData.GetValue(0).ToString(), AttributeChangeType.NEUTRAL, false);
                TextBody = string.Empty;
                return;
            }

            InventoryItemDefinition oldItemDefinition = _itemDatabase.GetItemDefinition(oldItem);
            string header = oldItemDefinition.Name;

            _itemSlotController.BindItem(oldItem, oldItemDefinition, -1, false, null, null, null);

            float currentValue = playerAttributeData.GetValue(0);
            float oldItemValue = 0f;
            if (oldItem is Equipment oldEquipment)
            {
                AttributeData oldEquipmentAttributeData = oldEquipment.AttributeData(_itemDatabase);
                oldItemValue = oldEquipmentAttributeData.GetValue(0);
                header += $" (Lv.{oldEquipment.Level})";
                _itemStat.text = oldItemValue.ToString();
            }

            float newValue = currentValue - oldItemValue;
            var changeType = AttributeLine.GetAttributeChangeType(newValue, currentValue);
            _attributeLineController.SetOldValue(currentValue.ToString());
            _attributeLineController.SetNewValue(newValue.ToString(), changeType, false);

            _itemTitle.text = header;
            TextBody = oldItemDefinition.Description;
            TextHeader = "Unequip Item?";
        }
    }
}