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
        public int StackCount = 1;
        
        public InventorySlot(int itemId, InventoryItems.InventoryItem item, int stackCount)
        {
            ItemId = itemId;
            Item = item;
            StackCount = stackCount;
        }
        
        public void AddCount(int count)
        {
            if(count > 0)
            {
                int stackCount = StackCount + count;
                if (stackCount > 0 && stackCount < Item.MaxStackCount)
                {
                    StackCount = stackCount;
                }
                else StackCount = Item.MaxStackCount;
            }
        }
    }
}
