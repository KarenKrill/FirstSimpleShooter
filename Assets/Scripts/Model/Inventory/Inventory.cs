using Assets.Scripts.Model.InventoryItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(Inventory), menuName = nameof(Inventory) + "/" + nameof(Inventory))]
    public class Inventory : ScriptableObject, ISerializationCallbackReceiver
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

        #region ISerializationCallbackReceiver
        public void OnAfterDeserialize()
        {
            if (ItemsSlots != null)
            {
                foreach (var itemSlot in ItemsSlots)
                {
                    if (itemSlot != null)
                    {
                        if (itemSlot.Item != null && ItemsDatabase.TryGetItemId(itemSlot.Item, out var id))
                        {
                            itemSlot.ItemId = id;
                        }
                        else if (itemSlot.ItemId > 0 && ItemsDatabase.TryGetItem(itemSlot.ItemId, out var item))
                        {
                            itemSlot.Item = item;
                        }
                        else
                        {
                            //itemSlot.Item = null;
                            //itemSlot.ItemId = 0;
                            //itemSlot.StackCount = 0;
                            Debug.LogWarning($"{nameof(Inventory)}.{nameof(Inventory.ItemsDatabase)} doesn't contains item with id {itemSlot.ItemId}");
                        }
                    }
                }
                //JsonUtility.FromJsonOverwrite(_serializedItemsSlots, ItemsSlots[0]);
            }
        }
        public void OnBeforeSerialize() { }
        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (ItemsSlots.Count > 0 && ItemsDatabase == null)
            {
                ItemsSlots.Clear();
                Debug.LogWarning($"{nameof(Inventory)}.{nameof(Inventory.ItemsSlots)} can't be populate while {nameof(Inventory)}.{nameof(Inventory.ItemsDatabase)} is null");
            }
            else
            {
                for (int i = 0; i < ItemsSlots.Count; i++)
                {
                    var itemSlot = ItemsSlots[i];
                    if (itemSlot != null)
                    {
                        if (!ItemsDatabase.Contains(itemSlot.ItemId))
                        {
                            ItemsSlots[i].Item = null;
                            Debug.LogWarning($"{nameof(Inventory)}.{nameof(Inventory.ItemsSlots)} can't contains item which doesn't exists in {nameof(InventoryDatabase)}.{nameof(InventoryDatabase.Items)}");
                        }
                        else if (itemSlot.Item != null)
                        {
                            if (itemSlot.StackCount > itemSlot.Item.MaxStackCount)
                            {
                                itemSlot.StackCount = itemSlot.Item.MaxStackCount;
                            }
                        }
                        else if(itemSlot.ItemId != 0)
                        {
                            itemSlot.ItemId = 0;
                        }
                    }
                }
            }
        }
#endif
    }
}
