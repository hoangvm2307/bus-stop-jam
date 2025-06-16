using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using CupkekGames.InventorySystem;
using CupkekGames.Systems;
using CupkekGames.Core;

namespace CupkekGames.Luna.Library
{
    public class EquipmentViewPagination : UIViewComponent
    {
        [SerializeField] private VisualTreeAsset _itemSlotTemplate;
        [SerializeField] private List<InventoryItemReference> _itemReferences;
        [SerializeField] private List<Sprite> _equipmentBackgrounds;
        [SerializeField] private int _lineCount = 3;
        [MultiLineHeader("Button Count Per Line")]
        [SerializeField] private int _buttonCountPerLine = 3;
        [MultiLineHeader("Item Width For Auto Calculating Count Per Line\n-1 to disable, positive value to enable")]
        [SerializeField] private int _itemWidth = -1;
        [SerializeField] private bool _hideEmpty = false;
        [SerializeField] private UIColorName _paginationNormal = UIColorName.PRIMARY;
        [SerializeField] private UIColorName _paginationActive = UIColorName.SECONDARY;
        [SerializeField] private int _paginationMaxButtons = 3;
        private ICollection<string> _attributes;
        [SerializeField] private InventoryItemDatabase _itemDatabase;
        [SerializeField] private TooltipController _tooltipController;
        private Inventory _inventory;
        private InventoryViewPagination _inventoryView;
        private Button _returnButton;
        private List<EquipmentSlotController> _equipmenSlots = new();
        private AttributeData _attributeData;
        private AttributeDataController _attributeDataController;

        protected override void Awake()
        {
            base.Awake();

            if (_itemDatabase == null)
            {
                _itemDatabase = ServiceLocator.Get<InventoryItemDatabase>();
            }

            _attributes = new List<string>()
                {
                    "Attack",
                    "Health",
                    "Defense"
                };

            _attributeData = new AttributeData();
            for (int i = 0; i < 3; i++)
            {
                _attributeData.SetValue(i, 10);
            }

            // Binding
            VisualElement statsContainer = ParentElement.Q<VisualElement>("StatsContainer");
            _attributeDataController = new AttributeDataController(_attributes, statsContainer, _attributeData, null, false, 3, true, false);

            List<VisualElement> _equipmentVE = new()
            {
                ParentElement.Q<VisualElement>("SlotHand"),
                ParentElement.Q<VisualElement>("SlotShield"),
                ParentElement.Q<VisualElement>("SlotAmulet"),
                ParentElement.Q<VisualElement>("SlotChest"),
                ParentElement.Q<VisualElement>("SlotLeggings"),
                ParentElement.Q<VisualElement>("SlotBoots"),
                ParentElement.Q<VisualElement>("SlotPotion")
            };

            if (_tooltipController == null)
            {
                _tooltipController = TooltipDatabaseExample.Instance.TooltipController;
            }

            // Create Equipment Slots
            CreateEquipmentSlot(_equipmentVE, (int)InventoryItemType.Equipment, (int)EquipmentType.Hand);
            CreateEquipmentSlot(_equipmentVE, (int)InventoryItemType.Equipment, (int)EquipmentType.Shield);
            CreateEquipmentSlot(_equipmentVE, (int)InventoryItemType.Equipment, (int)EquipmentType.Amulet);
            CreateEquipmentSlot(_equipmentVE, (int)InventoryItemType.Equipment, (int)EquipmentType.Chest);
            CreateEquipmentSlot(_equipmentVE, (int)InventoryItemType.Equipment, (int)EquipmentType.Pants);
            CreateEquipmentSlot(_equipmentVE, (int)InventoryItemType.Equipment, (int)EquipmentType.Boots);
            CreateEquipmentSlot(_equipmentVE, (int)InventoryItemType.Potion, (int)EquipmentType.Boots);

            Dictionary<int, Func<List<TooltipContainerSetup>>> _equipmentTypeToTooltipSetup = new()
            {
                { 0, () => GetEquipmentTooltipSetup(0) },
                { 1, () => GetEquipmentTooltipSetup(1) },
                { 2, () => GetEquipmentTooltipSetup(2) },
                { 3, () => GetEquipmentTooltipSetup(3) },
                { 4, () => GetEquipmentTooltipSetup(4) },
                { 5, () => GetEquipmentTooltipSetup(5) },
                { 6, () => GetEquipmentTooltipSetup(6) }
            };

            VisualElement inventoryElement = UIDocument.rootVisualElement.Q<VisualElement>("Inventory");
            VisualElement container = inventoryElement.Q<VisualElement>("InventorySlotContainer");
            InputPrompt filterItemPrevious = inventoryElement.Q<InputPrompt>("FilterItemPrevious");
            InputPrompt filterItemNext = inventoryElement.Q<InputPrompt>("FilterItemNext");
            VisualElement paginationElement = inventoryElement.Q<VisualElement>("Pagination");

            List<List<Button>> filterButtons = new()
            {
                inventoryElement.Query<Button>("FilterItemType").ToList()
            };

            var filters = new List<Func<List<InventoryItem>, int, List<InventoryItem>>>
            {
                FilterItems
            };

            List<InventoryItem> items = new();
            foreach (InventoryItemReference reference in _itemReferences)
            {
                items.Add(_itemDatabase.CreateItem(reference));
            }

            _inventory = new Inventory(items);

            _inventoryView = new InventoryViewPagination(
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
            _inventoryView.SetPaginationUI(paginationElement, _paginationNormal, _paginationActive, _paginationMaxButtons);
            _inventoryView.SetInputPrompt(filterItemPrevious, filterItemNext);
            _inventoryView.SetFilters(filters);
            _inventoryView.SetFilterButtons(filterButtons);
            _inventoryView.SetHideEmpty(_hideEmpty);
            _inventoryView.SetAttributes(_attributes);
            _inventoryView.SetTooltipPosition(TooltipPosition.Left);
            _inventoryView.SetDragAndDrop(null, GetEquipmentSlotFromItem, OnItemDropOnEquipment, UIDocument.rootVisualElement, false);
            _inventoryView.SetGetTooltipSetups(GetTooltipSetups);
            _inventoryView.SetGetAttributeComparison(GetAttributeData);

            _returnButton = UIDocument.rootVisualElement.Q<Button>("ReturnButton");

            List<VisualElement> except = new List<VisualElement>()
            {
                ParentElement,
                inventoryElement
            };
        }

        private void CreateEquipmentSlot(List<VisualElement> equipmentVE, int itemType, int equipmentType)
        {
            int index = itemType + equipmentType;
            EquipmentSlotController equipment = new EquipmentSlotController(
                _itemDatabase,
                gameObject,
                equipmentVE[index],
                _tooltipController,
                TooltipPosition.Right,
                (item, itemDefition) => EquipCondition(itemType, equipmentType, item, itemDefition)
                );
            equipment.SetAttributeNames(_attributes);
            equipment.SetEmptyIcon(_equipmentBackgrounds[index]);
            string name;
            if (itemType == 0)
            {
                // Equipment
                name = Enum.GetName(typeof(EquipmentType), equipmentType);
            }
            else
            {
                name = Enum.GetName(typeof(InventoryItemType), equipmentType);
            }
            equipment.SetEmptyTooltip(new TooltipManipulator(gameObject, _tooltipController, new() { GetEmptyEquipmentTooltipSetup(name, "Empty " + name + " Slot") }));
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
            _returnButton.clicked += FadeOutThenDestroy;

            _inventoryView.OnItemSlotClick += OnInventorySlotClicked;

            foreach (EquipmentSlotController controller in _equipmenSlots)
            {
                controller.OnClick += OnEquipmentSlotClicked;
            }
        }

        private void OnDisable()
        {
            _returnButton.clicked -= FadeOutThenDestroy;

            _inventoryView.OnItemSlotClick -= OnInventorySlotClicked;

            foreach (EquipmentSlotController controller in _equipmenSlots)
            {
                controller.OnClick -= OnEquipmentSlotClicked;
            }
        }

        private void OnItemDropOnEquipment(int slotIndex, int equipmentIndex, VisualElement dropSlot, InventoryItemSlotController inventorySlot)
        {
            SwapInventoryItemWithEquipment(inventorySlot);
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
                _inventory.AddItem(equipment);
                RemoveAttribute(equipment);

                equipmentSlot.UnbindItem();
            }

            _inventoryView.Refresh();
            _attributeDataController.Update(_attributeData);
        }

        private bool Equip(EquipmentSlotController equipmentSlot, int equipmentSlotIndex, InventoryItem item, InventoryItemDefinition itemDefinition)
        {
            if (!equipmentSlot.Condition(item, itemDefinition))
            {
                return false;
            }

            InventoryItemSlotController dragSlotController = new InventoryItemSlotController(_itemDatabase, gameObject, null, null, TooltipPosition.Right);

            ItemDragAndDrop itemDragAndDrop = new ItemDragAndDrop(LunaUIManager, UIDocument.rootVisualElement, equipmentSlotIndex,
                null, GetInventorySlots, _itemSlotTemplate, equipmentSlot, dragSlotController);
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

        private void OnInventorySlotClicked(InventoryItemSlotController controller)
        {
            if (controller.IsEmpty)
            {
                return;
            }

            SwapInventoryItemWithEquipment(controller);
        }

        private void OnEquipmentSlotClicked(InventoryItemSlotController controller)
        {
            if (controller.IsEmpty)
            {
                return;
            }

            Unequip((EquipmentSlotController)controller);
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
                _attributeDataController.Update(_attributeData);
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
            _attributeDataController.Update(_attributeData);
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
                return items.OrderBy(item => item.ItemType).ToList();
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