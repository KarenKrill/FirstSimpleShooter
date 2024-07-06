using System;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(Player), menuName = nameof(Player))]
    public class Player : ScriptableObject
    {
        [field: SerializeField]
        public string Name { get; private set; }

        [field: SerializeField, Min(0.001f)]
        public float Health { get; private set; } = 100;
        
        [field: SerializeField, Min(0.001f)]
        public float MaxHealth { get; private set; } = 100;

        [field: SerializeField, Min(0)]
        public float HeadDefence { get; private set; }
        
        [field: SerializeField, Min(0)]
        public float BodyDefence { get; private set; }

        [field: SerializeField, Min(1)]
        public float HeadDamageMultiplier { get; private set; } = 1;

        [field: SerializeField, Min(1)]
        public float BodyDamageMultiplier { get; private set; } = 1;

        [field: SerializeField]
        public Inventory InventoryConfig { get; private set; }

        [field: SerializeField]
        public Weapon EquippedWeapon { get; set; }

        //[field: SerializeField]
        //public List<Armor> EquippedArmors { get; set; }

        private void OnValidate()
        {
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
        }
    }
}
