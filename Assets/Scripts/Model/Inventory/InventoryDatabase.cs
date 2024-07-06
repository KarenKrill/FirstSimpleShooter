using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(InventoryDatabase), menuName = nameof(Inventory) + "/" + nameof(InventoryDatabase))]
    public class InventoryDatabase : ScriptableObject, ISerializationCallbackReceiver
    {
        private Dictionary<int, InventoryItems.InventoryItem> _itemsById = new();
        private Dictionary<InventoryItems.InventoryItem, int> _idsByItem = new();
        [field: SerializeField]
        public List<InventoryItems.InventoryItem> Items { get; private set; }
        public int GetItemId(InventoryItems.InventoryItem item) => _idsByItem[item];
        public bool TryGetItemId(InventoryItems.InventoryItem item, out int id) => _idsByItem.TryGetValue(item, out id);
        public InventoryItems.InventoryItem GetItem(int id) => _itemsById[id];
        public bool TryGetItem(int id, out InventoryItems.InventoryItem item) => _itemsById.TryGetValue(id, out item);
        public bool Contains(int id) => _itemsById.ContainsKey(id);
        #region ISerializationCallbackReceiver
        public void OnAfterDeserialize()
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
                        if(_idsByItem.ContainsKey(item))
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
        public void OnBeforeSerialize() { }
        #endregion
    }
}
