using System;
using UnityEngine;

namespace Assets.Scripts.Model.InventoryItems
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(Potion), menuName = nameof(Inventory) + "/" + nameof(Potion))]
    public class Potion : InventoryItem
    {
        [field: Header(nameof(Potion))]
        [field: SerializeField, Min(0)]
        public float RestoreValue { get; private set; }
    }

}
