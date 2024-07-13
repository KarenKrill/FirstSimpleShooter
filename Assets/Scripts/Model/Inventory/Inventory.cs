using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(Inventory), menuName = nameof(Inventory) + "/" + nameof(Inventory))]
    public class Inventory : ScriptableObject
    {
        [field: SerializeField]
        public InventoryDatabase ItemsDatabase;

        [SerializeField]
        private List<InventorySlot> _itemsSlots = new();

        public IReadOnlyList<InventorySlot> ItemsSlots => _itemsSlots;

        public event Action<InventoryItems.InventoryItem> ItemAdded;
        public event Action<InventoryItems.InventoryItem> ItemRemoved;
        public event Action SlotsCleared;
        public event Action ItemsCleared;
        public event Action<InventoryItems.InventoryItem> ItemCountChanged;

        private void OnSlotStackCountChanged(InventorySlot slot)
        {
            ItemCountChanged?.Invoke(slot.Item);
        }

        public void ReserveSlots(int n)
        {
            _itemsSlots.Capacity = n;
        }
        public void SetItem(int slotIndex, InventoryItems.InventoryItem item, int count)
        {
            if (slotIndex >= 0 && slotIndex < _itemsSlots.Count)
            {
                if (_itemsSlots[slotIndex].Item != null)
                {
                    RemoveItemAt(slotIndex);
                }
                if (ItemsDatabase.TryGetItemId(item, out int id))
                {
                    var itemSlot = new InventorySlot(id, item, count);
                    itemSlot.StackCountChanged += OnSlotStackCountChanged;
                    _itemsSlots[slotIndex] = itemSlot;
                    ItemAdded?.Invoke(item);
                }
            }
        }
        public void AddItem(InventoryItems.InventoryItem item, int count)
        {
            if (item != null && count >= 0)
            {
                for (int i = 0; i < _itemsSlots.Count; i++)
                {
                    if (_itemsSlots[i].Item == item)
                    {
                        _itemsSlots[i].AddCount(count);
                        return;
                    }
                }
                if (ItemsDatabase.TryGetItemId(item, out int id))
                {
                    var itemSlot = new InventorySlot(id, item, count);
                    itemSlot.StackCountChanged += OnSlotStackCountChanged;
                    _itemsSlots.Add(itemSlot);
                    ItemAdded?.Invoke(item);
                }
            }
        }
        public void RemoveItem(InventoryItems.InventoryItem item)
        {
            if (item != null)
            {
                for (int i = 0; i < _itemsSlots.Count; i++)
                {
                    var slot = _itemsSlots[i];
                    if (slot.Item == item)
                    {
                        slot.Clear();
                        slot.StackCountChanged -= OnSlotStackCountChanged;
                        ItemRemoved?.Invoke(item);
                        return;
                    }
                }
            }
        }
        public void RemoveItem(InventoryItems.InventoryItem item, int count)
        {
            if (item != null && count >= 0)
            {
                for (int i = 0; i < _itemsSlots.Count; i++)
                {
                    var slot = _itemsSlots[i];
                    if (slot.Item == item)
                    {
                        slot.RemoveCount(count);
                        if (slot.StackCount == 0)
                        {
                            slot.StackCountChanged -= OnSlotStackCountChanged;
                            slot.Clear();
                            ItemRemoved?.Invoke(item);
                        }
                        return;
                    }
                }
            }
        }
        public void RemoveItemAt(int slotIndex, int count)
        {
            if (slotIndex > 0 && slotIndex < _itemsSlots.Count && count >= 0)
            {
                var slot = _itemsSlots[slotIndex];
                slot.RemoveCount(count);
                if (slot.StackCount == 0)
                {
                    var removingItem = slot.Item;
                    slot.Clear();
                    slot.StackCountChanged -= OnSlotStackCountChanged;
                    ItemRemoved?.Invoke(removingItem);
                }
            }
        }
        public void RemoveItemAt(int slotIndex)
        {
            if (slotIndex > 0 && slotIndex < _itemsSlots.Count)
            {
                var slot = _itemsSlots[slotIndex];
                var removingItem = slot.Item;
                slot.Clear();
                slot.StackCountChanged -= OnSlotStackCountChanged;
                ItemRemoved?.Invoke(removingItem);
            }
        }
        public void ClearSlots()
        {
            if (_itemsSlots.Count > 0)
            {
                _itemsSlots.Clear();
                SlotsCleared?.Invoke();
            }
        }
        public void ClearItems()
        {
            if (_itemsSlots.Count > 0)
            {
                bool dirtyFlag = false;
                foreach (var slot in _itemsSlots)
                {
                    if (slot.Item != null)
                    {
                        slot.StackCountChanged -= OnSlotStackCountChanged;
                        slot.Clear();
                        dirtyFlag = true;
                    }
                }
                if (dirtyFlag)
                {
                    ItemsCleared?.Invoke();
                }
            }
        }

        public string Serialize(bool prettyPrint = false) => JsonUtility.ToJson(this, prettyPrint);
        public void Deserialize(string serializedStr) => JsonUtility.FromJsonOverwrite(serializedStr, this);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_itemsSlots == null)
            {
                return;
            }
            if (_itemsSlots.Count > 0 && (ItemsDatabase == null || ItemsDatabase.Items == null || ItemsDatabase.Items.Count <= 0))
            {
                //_itemsSlots.Clear();
                Debug.LogWarning($"\"{this.name}\" {nameof(Inventory)} refers to an empty {nameof(Inventory)}.{nameof(Inventory.ItemsDatabase)}");
            }
            else
            {
                for (int i = 0; i < _itemsSlots.Count; i++)
                {
                    var itemSlot = _itemsSlots[i];
                    if (itemSlot != null)
                    {
                        if (itemSlot.Item != null && ItemsDatabase.TryGetItemId(itemSlot.Item, out var id))
                        {
                            itemSlot.ItemId = id;
                        }
                        //else if (itemSlot.ItemId > 0 && ItemsDatabase.TryGetItem(itemSlot.ItemId, out var item))
                        //{
                        //    itemSlot.Item = item;
                        //}
                        else
                        {
                            if (itemSlot.Item != null)
                            {
                                Debug.LogWarning($"{nameof(Inventory)} \"{name}\": {nameof(InventoryItemComponent)} \"{itemSlot.Item.name}\" (id:{itemSlot.ItemId}) doesn't exists in {nameof(InventoryDatabase)} \"{ItemsDatabase.name}\"");
                            }
                            //else
                            //{
                            //    Debug.LogWarning($"{nameof(Inventory)} \"{name}\": {nameof(InventoryItem)} {i} (id:{itemSlot.ItemId}) doesn't exists in {nameof(InventoryDatabase)} \"{ItemsDatabase.name}\"");
                            //}
                            itemSlot.Clear();
                            continue;
                        }
                        if (itemSlot.Item != null)
                        {
                            if (itemSlot.StackCount > itemSlot.Item.MaxStackCount)
                            {
                                itemSlot.StackCount = itemSlot.Item.MaxStackCount;
                            }
                            else if (itemSlot.StackCount <= 0)
                            {
                                itemSlot.StackCount = 1;
                            }
                        }
                        else itemSlot.Clear();
                    }
                }
            }
        }
#endif
    }
}
