using System;
using UnityEngine;

namespace Assets.Scripts.Model.InventoryItems
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(Ammo), menuName = nameof(Inventory) + "/" + nameof(Ammo))]
    public class Ammo : InventoryItem
    {
        [field: Header(nameof(Ammo))]
        [field: SerializeField]
        public string WeaponName { get; private set; }

        [field: SerializeField, Min(0)]
        public float Damage { get; private set; }
    }

}
