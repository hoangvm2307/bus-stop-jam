using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CupkekGames.Luna;

namespace CupkekGames.InventorySystem
{
    [Serializable]
    public abstract class InventoryBase
    {
        public abstract List<InventoryItem> Items { get; }

        public abstract InventoryItem GetItem(Guid id);
        public abstract InventoryItem GetItemAt(int index);

        public abstract void AddItem(InventoryItem add);
        public abstract void SetItem(int index, InventoryItem item);

        /// <summary>
        /// Removes a specified amount of an item from the inventory.
        /// </summary>
        /// <param name="id">The unique identifier of the item to remove.</param>
        /// <param name="amount">The amount to remove.</param>
        /// <returns>True if the item was completely removed, false if some amount remains.</returns>
        public abstract bool RemoveItem(Guid id, int amount);

        public abstract bool RemoveItem(InventoryItem item);

        public abstract int GetItemSlot(Guid id);

        public abstract void SwapSlots(int a, int b);

        public bool RemoveItem(Guid id)
        {
            var item = GetItem(id);

            return RemoveItem(item);
        }

        public void MoveItem(InventoryItemDatabase manager, Guid id, InventoryItemSlotController to,
            int index, ItemDragAndDrop dragAndDrop, Func<InventoryItem, InventoryItemDefinition, AttributeData> getComparison,
            Func<List<TooltipContainerSetup>> getSetups)
        {
            InventoryItem item = GetItem(id);

            MoveItem(item, manager.GetItemDefinition(item), to, index, dragAndDrop, getComparison, getSetups);
        }

        public virtual void MoveItem(InventoryItem item, InventoryItemDefinition itemDefinition, InventoryItemSlotController to,
            int index, ItemDragAndDrop dragAndDrop, Func<InventoryItem, InventoryItemDefinition, AttributeData> getComparison,
            Func<List<TooltipContainerSetup>> getSetups)
        {
            bool toSelected = to.Selected;
            if (!to.IsEmpty)
            {
                InventoryItem toItem = to.Item;
                if (item == null)
                {
                    // To has item, current is empty
                    AddItem(toItem);
                    to.UnbindItem();
                }
                else
                {
                    // Both has item
                    if (toItem.CanStackWith(item))
                    {
                        int left = toItem.AddAmount(item.Amount);
                        item.SetAmount(left);
                        if (item.Amount == 0)
                        {
                            RemoveItem(item);
                        }
                        to.UpdateItemDisplay();
                    }
                    else
                    {
                        // Cant stack
                        int itemSlot = GetItemSlot(item.ID);
                        RemoveItem(item);

                        if (itemSlot != -1)
                        {
                            SetItem(itemSlot, toItem);
                        }
                        else
                        {
                            AddItem(toItem);
                        }

                        to.UnbindItem();
                        to.BindItem(item, itemDefinition, index, toSelected, dragAndDrop, getComparison, getSetups);
                    }
                }
            }
            else
            {
                if (item != null)
                {
                    // To is empty, current has item
                    RemoveItem(item);
                    to.BindItem(item, itemDefinition, index, toSelected, dragAndDrop, getComparison, getSetups);
                }
                // else both empty
            }
        }
    }
}