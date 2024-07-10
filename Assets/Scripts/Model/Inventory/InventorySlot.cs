using System;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class InventorySlot
    {
        [SerializeField]
        public int ItemId;

        [SerializeField]
        public InventoryItems.InventoryItem Item;
        
        [SerializeField, Min(1)]
        [Obsolete("Not trigger " + nameof(StackCountChanged) + " event, use " + nameof(AddCount) + "/" + nameof(RemoveCount) + " instead")]
        public int _stackCount = 1;

        public int StackCount
        {
#pragma warning disable 0618
            get => _stackCount;
#pragma warning restore 0618
            set
            {
                if (value < 0 || Item == null)
                {
                    value = 0;
                }
                else if (value > Item.MaxStackCount)
                {
                    value = Item.MaxStackCount;
                }
#pragma warning disable 0618
                if (_stackCount != value)
                {
                    _stackCount = value;
#pragma warning restore 0618
                    StackCountChanged?.Invoke(this);
                }
            }
        }

        public event Action<InventorySlot> StackCountChanged;
        
        public InventorySlot(int itemId, InventoryItems.InventoryItem item, int stackCount)
        {
            ItemId = itemId;
            Item = item;
#pragma warning disable 0618
            _stackCount = stackCount;
#pragma warning restore 0618
        }

        public void AddCount(int count)
        {
            if(count > 0)
            {
                StackCount += count;
            }
        }
        public void RemoveCount(int count)
        {
            if (count > 0)
            {
                StackCount -= count;
            }
        }
        public void Clear()
        {
            StackCount = 0;
            ItemId = 0;
            Item = null;
        }
    }
}
