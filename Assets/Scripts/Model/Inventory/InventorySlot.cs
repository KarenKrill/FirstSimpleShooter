using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class InventorySlot
    {
        [field: SerializeField]
        public int ItemId { get; set; }

        [field: SerializeField]
        public InventoryItems.InventoryItem Item { get; set; }
        
        [field: SerializeField, Min(1)]
        public int StackCount { get; set; } = 1;
        
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
