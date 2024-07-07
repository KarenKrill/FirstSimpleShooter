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
        public InventoryDatabase ItemsDatabase { get; private set; }

        [field: SerializeField]
        public List<InventorySlot> ItemsSlots { get; private set; }

        public void AddItem(InventoryItems.InventoryItem item, int count)
        {
            if (count >= 0)
            {
                for (int i = 0; i < ItemsSlots.Count; i++)
                {
                    if (ItemsSlots[i].Item == item)
                    {
                        ItemsSlots[i].AddCount(count);
                        return;
                    }
                }
                if (ItemsDatabase.TryGetItemId(item, out int id))
                {
                    ItemsSlots.Add(new(id, item, count));
                }
            }
        }

        public string Serialize(bool prettyPrint = false) => JsonUtility.ToJson(this, prettyPrint);
        public void Deserialize(string serializedStr) => JsonUtility.FromJsonOverwrite(serializedStr, this);

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ItemsSlots == null)
            {
                return;
            }
            if (ItemsSlots.Count > 0 && (ItemsDatabase == null || ItemsDatabase.Items == null || ItemsDatabase.Items.Count <= 0))
            {
                //ItemsSlots.Clear();
                Debug.LogWarning($"\"{this.name}\" {nameof(Inventory)} refers to an empty {nameof(Inventory)}.{nameof(Inventory.ItemsDatabase)}");
            }
            else
            {
                for (int i = 0; i < ItemsSlots.Count; i++)
                {
                    var itemSlot = ItemsSlots[i];
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
                                Debug.LogWarning($"{nameof(Inventory)} \"{name}\": {nameof(InventoryItem)} \"{itemSlot.Item.name}\" (id:{itemSlot.ItemId}) doesn't exists in {nameof(InventoryDatabase)} \"{ItemsDatabase.name}\"");
                            }
                            //else
                            //{
                            //    Debug.LogWarning($"{nameof(Inventory)} \"{name}\": {nameof(InventoryItem)} {i} (id:{itemSlot.ItemId}) doesn't exists in {nameof(InventoryDatabase)} \"{ItemsDatabase.name}\"");
                            //}
                            itemSlot.Item = null;
                            itemSlot.ItemId = 0;
                            itemSlot.StackCount = 0;
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
                        else
                        {
                            itemSlot.ItemId = 0;
                            itemSlot.StackCount = 0;
                        }
                    }
                }
            }
        }
#endif
    }
}
