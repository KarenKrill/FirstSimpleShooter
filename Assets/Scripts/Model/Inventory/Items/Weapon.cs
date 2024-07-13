using System;
using UnityEngine;

namespace Assets.Scripts.Model.InventoryItems
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(Weapon), menuName = nameof(Inventory) + "/" + nameof(Weapon))]
    public class Weapon : InventoryItem
    {
        [field: Header("Weapon")]
        [field: SerializeField, Min(0)]
        public float Damage { get; private set; }

        [SerializeField, Min(0)]
        private int _ammoCount = 0;
        public int AmmoCount
        {
            get => _ammoCount;
            set
            {
                if (_ammoCount != value)
                {
                    _ammoCount = value;
                    AmmoCountChanged?.Invoke(this);
                }
            }
        }
        public Action<Weapon> AmmoCountChanged;

        [field: SerializeField, Min(0)]
        public int MaxAmmoCount { get; private set; }

        [field: SerializeField, Min(0)]
        public int AmmoCountPerShoot { get; private set; }

        protected void OnValidate()
        {
            if (AmmoCount > MaxAmmoCount)
            {
                AmmoCount = MaxAmmoCount;
            }
            if (AmmoCountPerShoot > MaxAmmoCount)
            {
                AmmoCountPerShoot = MaxAmmoCount;
            }
        }
    }
}
