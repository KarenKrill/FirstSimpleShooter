using System;
using UnityEngine;

namespace Assets.Scripts.Model.InventoryItems
{
    [Serializable]
    public enum ArmorType
    {
        Headgear,
        Cuirass
    }
    [Serializable]
    [CreateAssetMenu(fileName = nameof(Armor), menuName = nameof(Inventory) + "/" + nameof(Armor))]
    public class Armor : InventoryItem
    {
        [field: Header(nameof(Armor))]
        [field: SerializeField]
        public ArmorType Type { get; private set; }

        [field: SerializeField, Min(0)]
        public float Defence { get; private set; }
    }
}
