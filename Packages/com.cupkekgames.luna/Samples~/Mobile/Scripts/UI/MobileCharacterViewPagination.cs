using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.InventorySystem;
using CupkekGames.Systems;
using CupkekGames.Core;
using CupkekGames.Luna.Library;

namespace CupkekGames.Luna.Mobile
{
    public class MobileCharacterViewPagination : UIViewComponent
    {
        [SerializeField] private VisualTreeAsset _itemSlotTemplate;
        [SerializeField] private List<Sprite> _equipmentBackgrounds;
        [SerializeField] private int _lineCount = 5;
        [SerializeField] private int _buttonCountPerLine = 3;
        [MultiLineHeader("Item Width For Auto Calculating Count Per Line\n-1 to disable, positive value to enable")]
        [SerializeField] private int _itemWidth = -1;
        [SerializeField] private bool _hideEmpty = false;
        [SerializeField] private int _itemAmount = 10;
        private ICollection<string> _attributes;
        [SerializeField] private InventoryItemDatabase _itemDatabase;
        [SerializeField] private InventoryItemPopupController _itemPopupController;
        private TooltipController _tooltipController;
        private InventoryWithSlots _inventory;
        private MobileInventoryViewPagination _inventoryView;
        private List<EquipmentSlotController> _equipmenSlots = new();
        private AttributeData _attributeData;
        private AttributeLineController _attributeDataController;

        protected override void Awake()
        {
            base.Awake();

            if (_itemDatabase == null)
            {
                _itemDatabase = ServiceLocator.Get<InventoryItemDatabase>();
            }

            _attributes = new List<string>()
                {
                    "CombatProficiency",
                };

            _attributeData = new AttributeData();
            _attributeData.SetValue(0, 0);

            VisualElement combatProficiency = ParentElement.Q<VisualElement>("CombatProficiency");
            _attributeDataController = new AttributeLineController(combatProficiency, false, false);
            _attributeDataController.SetOldValue(_attributeData.GetValue(0).ToString());

            List<VisualElement> _equipmentVE = new()
            {
                ParentElement.Q<VisualElement>("EquipmentHand").Q<VisualElement>("InventoryItem"),
                ParentElement.Q<VisualElement>("EquipmentShield").Q<VisualElement>("InventoryItem"),
                ParentElement.Q<VisualElement>("EquipmentAmulet").Q<VisualElement>("InventoryItem"),
                ParentElement.Q<VisualElement>("EquipmentArmor").Q<VisualElement>("InventoryItem"),
                ParentElement.Q<VisualElement>("EquipmentPants").Q<VisualElement>("InventoryItem"),
                ParentElement.Q<VisualElement>("EquipmentBoots").Q<VisualElement>("InventoryItem")
            };

            // Create Equipment Slots
            CreateEquipmentSlot(_equipmentVE[0], 0, "Hand");
            CreateEquipmentSlot(_equipmentVE[1], 1, "Shield");
            CreateEquipmentSlot(_equipmentVE[2], 2, "Amulet");
            CreateEquipmentSlot(_equipmentVE[3], 3, "Armor");
            CreateEquipmentSlot(_equipmentVE[4], 4, "Pants");
            CreateEquipmentSlot(_equipmentVE[5], 5, "Boots");

            // Dictionary<int, Func<List<TooltipContainerSetup>>> _equipmentTypeToTooltipSetup = new()
            // {
            //     { 0, () => GetEquipmentTooltipSetup(0) },
            //     { 1, () => GetEquipmentTooltipSetup(1) },
            //     { 2, () => GetEquipmentTooltipSetup(2) },
            //     { 3, () => GetEquipmentTooltipSetup(3) }
            // };

            VisualElement inventoryElement = UIDocument.rootVisualElement.Q<VisualElement>("Inventory");
            VisualElement container = inventoryElement.Q<VisualElement>("InventorySlotContainer");

            List<List<Button>> filterButtons = new()
            {
                new List<Button>() {
                    inventoryElement.Q<Button>("InventoryFilterHand"),
                    inventoryElement.Q<Button>("InventoryFilterShield"),
                    inventoryElement.Q<Button>("InventoryFilterAmulet"),
                    inventoryElement.Q<Button>("InventoryFilterArmor"),
                    inventoryElement.Q<Button>("InventoryFilterPants"),
                    inventoryElement.Q<Button>("InventoryFilterBoots")
                }
            };

            List<List<Button>> filterClearButtons = new()
            {
                new List<Button>() {
                    inventoryElement.Q<Button>("InventoryFilterAll"),
                }
            };

            var filters = new List<Func<List<InventoryItem>, int, List<InventoryItem>>>
            {
                FilterItems
            };

            // Create random items
            EquipmentDatabase equipments = ((InventoryItemDatabaseExample)_itemDatabase).Equipments;
            Dictionary<string, EquipmentDefinitionSO>.KeyCollection keys = equipments.Keys;

            Dictionary<int, InventoryItem> items = new();

            for (int i = 0; i < _itemAmount; i++)
            {
                items.Add(i, new Equipment(
                    0,
                    keys.ElementAt(UnityEngine.Random.Range(0, keys.Count)),
                    UnityEngine.Random.Range(0, 4)
                ));
            }

            _inventory = new InventoryWithSlots(items);

            _inventoryView = new(
                _itemDatabase,
                _inventory,
                _itemSlotTemplate,
                gameObject,
                inventoryElement,
                container,
                _tooltipController
            );
            _inventoryView.SetPagination(_lineCount, _buttonCountPerLine);
            if (_itemWidth > 0)
            {
                _inventoryView.SetDynamicItemPerLine(_lineCount, _itemWidth);
            }
            _inventoryView.SetFilters(filters);
            _inventoryView.SetFilterButtons(filterButtons);
            _inventoryView.SetFilterClearButtons(filterClearButtons);
            _inventoryView.UpdateFilterButtons(0);
            _inventoryView.SetHideEmpty(_hideEmpty);
            _inventoryView.SetAttributes(_attributes);
            _inventoryView.SetTooltipPosition(TooltipPosition.Left);
            _inventoryView.SetDragAndDrop(null, GetEquipmentSlotFromItem, OnItemDrop, UIDocument.rootVisualElement, true);
            _inventoryView.SetGetTooltipSetups(GetTooltipSetups);
            _inventoryView.SetGetAttributeComparison(GetAttributeData);

            List<VisualElement> except = new List<VisualElement>()
            {
                ParentElement,
                inventoryElement
            };
        }

        private void CreateEquipmentSlot(VisualElement equipmentVE, int equipmentType, string name)
        {
            EquipmentSlotController equipment = new MobileEquipmentSlotController(
                _itemDatabase,
                gameObject,
                equipmentVE,
                _tooltipController,
                TooltipPosition.Right,
                (item, itemDefition) => EquipCondition(0, equipmentType, item, itemDefition)
            );
            equipment.SetAttributeNames(_attributes);
            equipment.SetEmptyIcon(_equipmentBackgrounds[equipmentType]);
            if (_tooltipController != null)
            {
                equipment.SetEmptyTooltip(new TooltipManipulator(gameObject, _tooltipController, new() { GetEmptyEquipmentTooltipSetup(name, $"Empty {name} Slot") }));
            }
            _equipmenSlots.Add(equipment);
        }

        private void Start()
        {
            if (_itemWidth > 0)
            {
                _inventoryView.RegisterDynamicItemPerLineUpdate();
            }
            else
            {
                _inventoryView.CreateSlots();
            }
        }

        private void OnEnable()
        {
            _inventoryView.OnItemSlotClick += OnInventorySlotClicked;

            foreach (EquipmentSlotController controller in _equipmenSlots)
            {
                controller.OnClick += OnEquipmentSlotClicked;
            }

            _itemPopupController.OnButtonClick += OnItemPopupButtonClick;
        }

        private void OnDisable()
        {
            _inventoryView.OnItemSlotClick -= OnInventorySlotClicked;

            foreach (EquipmentSlotController controller in _equipmenSlots)
            {
                controller.OnClick -= OnEquipmentSlotClicked;
            }

            _itemPopupController.OnButtonClick -= OnItemPopupButtonClick;
        }

        private void OnItemDrop(int slotIndex, int dropIndex, VisualElement dropSlot, InventoryItemSlotController inventorySlot)
        {
            int equipmentOffset = 1;
            if (dropIndex < equipmentOffset)
            {
                // Drop inventory item on equipment slot
                SwapInventoryItemWithEquipment(inventorySlot);
            }
            else
            {
                int target = dropIndex - equipmentOffset;
                if (slotIndex != target)
                {
                    // Drop inventory item on inventory another inventory slot
                    // Debug.Log($"Drop inventory item on inventory slot {dropIndex - equipmentOffset} from {slotIndex}");

                    // Swap inventory slot with another inventory slot
                    SwapItems(slotIndex, target);
                }
            }
        }
        private void SwapItems(int a, int b)
        {
            _inventory.SwapSlots(a, b);
            _inventoryView.Refresh();
        }
        private List<VisualElement> GetEquipmentSlotFromItem(InventoryItem item)
        {
            List<VisualElement> result = new List<VisualElement>();

            if (item == null)
            {
                return result;
            }

            InventoryItemDefinition itemDefinition = _itemDatabase.GetItemDefinition(item);

            int equipmentSlotIndex = GetEquipmentSlotIndex(item, itemDefinition);

            EquipmentSlotController equipmentSlot = _equipmenSlots[equipmentSlotIndex];

            result.Add(equipmentSlot.Parent);

            return result;
        }

        private void OnEquipmentDropOnItem(int equipmentIndex, int dropIndex, VisualElement dropSlot, InventoryItemSlotController sourceSlot)
        {
            EquipmentSlotController equipmentSlot = (EquipmentSlotController)sourceSlot;
            InventoryItem equipment = equipmentSlot.Item;
            InventoryItemDefinition equipmentDefinition = equipmentSlot.ItemDefinition;
            InventoryItemSlotController inventorySlot = (InventoryItemSlotController)dropSlot.userData;
            InventoryItem inventoryItem = inventorySlot.Item;
            InventoryItemDefinition inventoryItemDefinition = inventorySlot.ItemDefinition;

            if (inventoryItem != null && inventoryItem.GetType() == equipment.GetType())
            {
                // Replace items
                if (Equip(equipmentSlot, equipmentIndex, inventoryItem, inventoryItemDefinition))
                {
                    RemoveAttribute(equipment);
                    AddAttribute(inventoryItem);
                }
            }
            else
            {
                _inventory.SetItem(dropIndex, equipment);
                RemoveAttribute(equipment);

                equipmentSlot.UnbindItem();
            }

            _inventoryView.Refresh();
            _attributeDataController.SetOldValue(_attributeData.GetValue(0).ToString());
        }

        private bool Equip(EquipmentSlotController equipmentSlot, int equipmentSlotIndex, InventoryItem item, InventoryItemDefinition itemDefinition)
        {
            if (!equipmentSlot.Condition(item, itemDefinition))
            {
                return false;
            }

            MobileInventoryItemSlotController dragSlotController = new MobileInventoryItemSlotController(_itemDatabase, gameObject, null, null, TooltipPosition.Right);

            ItemDragAndDrop itemDragAndDrop = new ItemDragAndDrop(LunaUIManager, UIDocument.rootVisualElement, equipmentSlotIndex,
                null, GetInventorySlots, _itemSlotTemplate, equipmentSlot, dragSlotController);
            itemDragAndDrop.SetDropSlotClasses(new List<string> { "highlight" });
            itemDragAndDrop.OnItemDrop += OnEquipmentDropOnItem;

            _inventory.MoveItem(item, itemDefinition, equipmentSlot, equipmentSlotIndex, itemDragAndDrop, null, null);

            return true;
        }

        private List<VisualElement> GetInventorySlots()
        {
            return _inventoryView.ItemSlots;
        }

        private List<TooltipContainerSetup> GetEquipmentTooltipSetup(int equipmentSlotIndex)
        {
            List<TooltipContainerSetup> result = new();

            if (equipmentSlotIndex >= _equipmenSlots.Count)
            {
                return result;
            }

            EquipmentSlotController equipmentSlot = _equipmenSlots[equipmentSlotIndex];

            if (!equipmentSlot.IsEmpty)
            {
                TooltipContainerSetup setup = equipmentSlot.GetTooltipSetup(null);
                setup.Bottom.Add(new Label(RichTextColor.Colorize("(Equiped)", RichTextColor.YELLOW)));
                result.Add(setup);
            }

            return result;
        }

        private void OnInventorySlotClicked(InventoryItemSlotController inventorySlot)
        {
            if (inventorySlot.IsEmpty)
            {
                return;
            }


            int equipmentSlotIndex = GetEquipmentSlotIndex(inventorySlot.Item, inventorySlot.ItemDefinition);
            EquipmentSlotController equipmentSlot = _equipmenSlots[equipmentSlotIndex];

            _itemPopupController.SetEquiping(_attributeData, equipmentSlot, inventorySlot);
            _itemPopupController.Fade.FadeIn();
        }

        private void OnItemPopupButtonClick(int buttonIndex)
        {
            // 0 - Accept, 1 - Decline
            if (buttonIndex == 0)
            {
                if (_itemPopupController.IsEquiping)
                {
                    SwapInventoryItemWithEquipment(_itemPopupController.InventorySlot);
                }
                else
                {
                    Unequip(_itemPopupController.EquipmentSlot);
                }
            }
        }

        private void OnEquipmentSlotClicked(InventoryItemSlotController controller)
        {
            if (controller.IsEmpty)
            {
                return;
            }

            _itemPopupController.SetUnequiping(_attributeData, (EquipmentSlotController)controller);
            _itemPopupController.Fade.FadeIn();
        }

        private void SwapInventoryItemWithEquipment(InventoryItemSlotController inventorySlot)
        {
            int equipmentSlotIndex = GetEquipmentSlotIndex(inventorySlot.Item, inventorySlot.ItemDefinition);
            EquipmentSlotController equipmentSlot = _equipmenSlots[equipmentSlotIndex];
            InventoryItem equipment = equipmentSlot.Item;
            InventoryItemDefinition equipmentDefinition = equipmentSlot.ItemDefinition;
            InventoryItem inventoryItem = inventorySlot.Item;
            InventoryItemDefinition inventoryItemDefinition = inventorySlot.ItemDefinition;

            if (Equip(equipmentSlot, equipmentSlotIndex, inventoryItem, inventoryItemDefinition))
            {
                if (equipment != null)
                {
                    RemoveAttribute(equipment);
                }

                AddAttribute(inventoryItem);

                _inventoryView.Refresh();
                _attributeDataController.SetOldValue(_attributeData.GetValue(0).ToString());
            }
        }
        private void Unequip(EquipmentSlotController equipmentSlot)
        {
            InventoryItem equipment = equipmentSlot.Item;
            InventoryItemDefinition equipmentDefinition = equipmentSlot.ItemDefinition;

            _inventory.AddItem(equipment);
            RemoveAttribute(equipment);

            equipmentSlot.UnbindItem();

            _inventoryView.Refresh();
            _attributeDataController.SetOldValue(_attributeData.GetValue(0).ToString());
        }

        public AttributeData GetAttributeData(InventoryItem item, InventoryItemDefinition itemDefinition)
        {
            if (itemDefinition is EquipmentDefinition equipmentDefinition)
            {
                int equipmentType = (int)equipmentDefinition.EquipmentType;

                if (equipmentType >= _equipmenSlots.Count)
                {
                    return null;
                }

                EquipmentSlotController slot = _equipmenSlots[equipmentType];

                if (slot.IsEmpty)
                {
                    return null;
                }

                if (slot.Item is Equipment equipment)
                {
                    return equipment.AttributeData(_itemDatabase);
                }
            }

            return null;
        }

        private bool EquipCondition(int itemType, int equipmentType, InventoryItem item, InventoryItemDefinition itemDefinition)
        {
            if (itemType == 0)
            {
                if (itemDefinition is EquipmentDefinition equipmentDefinition)
                {
                    return (int)equipmentDefinition.EquipmentType == equipmentType;
                }
            }

            return itemType == item.ItemType;
        }

        private bool AddAttribute(InventoryItem item)
        {
            if (item is Equipment equipment)
            {
                _attributeData.Add(equipment.AttributeData(_itemDatabase));

                return true;
            }

            return false;
        }
        private bool RemoveAttribute(InventoryItem item)
        {
            if (item is Equipment equipment)
            {
                _attributeData.Remove(equipment.AttributeData(_itemDatabase));

                return true;
            }

            return false;
        }
        private Func<List<TooltipContainerSetup>> GetTooltipSetups(InventoryItem item, InventoryItemDefinition itemDefinition)
        {
            return () => GetEquipmentTooltipSetup(GetEquipmentSlotIndex(item, itemDefinition));
        }

        private int GetEquipmentSlotIndex(InventoryItem item, InventoryItemDefinition itemDefinition)
        {
            if (itemDefinition is EquipmentDefinition equipmentDefinition)
            {
                return (int)equipmentDefinition.EquipmentType;
            }

            // Equipment type offset
            int equipmentOffset = Enum.GetNames(typeof(EquipmentType)).Length - 1;

            return item.ItemType + equipmentOffset;
        }
        private TooltipContainerSetup GetEmptyEquipmentTooltipSetup(string name, string description)
        {
            Label tooltipName = new Label(name);
            Label tooltipDescription = new Label(description);

            return new TooltipContainerSetup(null, tooltipName, tooltipDescription, null);
        }

        public List<InventoryItem> FilterItems(List<InventoryItem> items, int filter)
        {
            if (filter < 0)
            {
                return items;
            }

            int itemType = 0;
            int equipmentType = 0;

            int equipmentOffset = Enum.GetNames(typeof(EquipmentType)).Length - 1;

            if (filter <= equipmentOffset)
            {
                // equipments
                itemType = 0;
                equipmentType = filter;
            }
            else
            {
                // potions
                itemType = 1;
                equipmentType = 0;
            }

            return items.Where(item =>
            {
                if (item == null)
                {
                    return false;
                }

                if (item.ItemType != itemType)
                {
                    return false;
                }

                if (itemType == 0)
                {
                    // Check equipment type
                    InventoryItemDefinition itemDefinition = _itemDatabase.GetItemDefinition(item);

                    if (itemDefinition is EquipmentDefinition equipmentDefinition)
                    {
                        return (int)equipmentDefinition.EquipmentType == equipmentType;
                    }
                }

                return true;
            }).ToList();
        }
    }
}