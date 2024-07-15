using Assets.Scripts.Model.InventoryItems;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(InventoryDatabase), menuName = nameof(Inventory) + "/" + nameof(InventoryDatabase))]
    public class InventoryDatabase : ScriptableObject, ISerializationCallbackReceiver
    {
        private Dictionary<int, InventoryItem> _itemsById = new();
        private Dictionary<InventoryItem, int> _idsByItem = new();
        /// <summary>Call <see cref="ItemsChanged"/> after any changing</summary>
        [field: SerializeField]
        public List<InventoryItem> Items { get; private set; } = new(); // mb replace on ObservableCollection?
        /// <summary>Call this function after any <see cref="Items"/> changing</summary>
        public void ItemsChanged()
        {
            if (Items != null)
            {
                _itemsById.Clear();
                _idsByItem.Clear();
                for (int i = 0; i < Items.Count; i++)
                {
                    var item = Items[i];
                    if (item != null)
                    {
                        if (_idsByItem.ContainsKey(item))
                        {
                            Items[i] = null;
                            Debug.LogWarning($"{nameof(InventoryDatabase)}.{nameof(Items)} already contains item \"{item.Name}\" [Id: {item.GetInstanceID()}]");
                        }
                        else
                        {
                            var itemId = i + 1;
                            _itemsById[itemId] = item;
                            _idsByItem[item] = itemId;
                        }
                    }
                }
            }
        }
        public int GetItemId(InventoryItem item) => _idsByItem[item];
        public bool TryGetItemId(InventoryItem item, out int id) => _idsByItem.TryGetValue(item, out id);
        public InventoryItem GetItem(int id) => _itemsById[id];
        public bool TryGetItem(int id, out InventoryItem item) => _itemsById.TryGetValue(id, out item);
        public int GetItemIndex(int itemId)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] != null && _idsByItem[Items[i]] == itemId)
                {
                    return i;
                }
            }
            return -1;
        }
        public bool TryReplaceItem(InventoryItem oldItem, InventoryItem newItem)
        {
            if (_idsByItem.TryGetValue(oldItem, out var id))
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] == oldItem)
                    {
                        Items[i] = newItem;
                        _idsByItem.Remove(oldItem);
                        _idsByItem[newItem] = id;
                        _itemsById[id] = newItem;
                        return true;
                    }
                }
            }
            return false;
        }
        public bool Contains(int id) => _itemsById.ContainsKey(id);
        #region ISerializationCallbackReceiver
        public void OnAfterDeserialize() => ItemsChanged();
        public void OnBeforeSerialize() { }
        #endregion
    }
}
