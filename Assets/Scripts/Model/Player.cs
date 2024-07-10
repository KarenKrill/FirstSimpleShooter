using Assets.Scripts.Model.InventoryItems;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Assets.Scripts.Model
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(Player), menuName = nameof(Player))]
    public class Player : ScriptableObject, ISerializationCallbackReceiver
    {
        [field: SerializeField]
        public string Name { get; set; }

        [field: SerializeField, Min(0.001f)]
        public float Health { get; set; } = 100;
        
        [field: SerializeField, Min(0.001f)]
        public float MaxHealth { get; set; } = 100;
        public void Damage(float healthPoints) => Health -= healthPoints;
        public void Heal(float healthPoints) => Health += healthPoints;
        public bool IsAlive => Health > 0;
        public bool IsDead => Health <= 0;

        [field: SerializeField, Min(0)]
        public float HeadDefence { get; set; }
        
        [field: SerializeField, Min(0)]
        public float BodyDefence { get; set; }

        [field: SerializeField, Min(1)]
        public float HeadDamageMultiplier { get; set; } = 1;

        [field: SerializeField, Min(1)]
        public float BodyDamageMultiplier { get; set; } = 1;

        [SerializeField]
        private string _serializedInventory;
        public Inventory InventoryConfig { get; set; }

        [SerializeField]
        private string _serializedEquippedWeapon;
        public Weapon EquippedWeapon { get; set; }
        public Weapon WeaponTemplate { get; set; }
        public Armor ArmorTemplate { get; set; }

        [SerializeField]
        private string _serializedEquippedArmors;
        public Dictionary<ArmorType, Armor> EquippedArmors = new();

        public void OnAfterDeserialize()
        {
            if (InventoryConfig != null && !string.IsNullOrEmpty(_serializedInventory))
            {
                JsonUtility.FromJsonOverwrite(_serializedInventory, InventoryConfig);
            }
            if (EquippedWeapon == null)
            {
                EquippedWeapon = Instantiate(WeaponTemplate);
            }
            if (EquippedWeapon != null && !string.IsNullOrEmpty(_serializedEquippedWeapon))
            {
                JsonUtility.FromJsonOverwrite(_serializedEquippedWeapon, EquippedWeapon);
            }
            if (EquippedArmors != null && !string.IsNullOrEmpty(_serializedEquippedArmors))
            {
                List<Armor> equippedArmorsList = new();
                JsonUtility.FromJsonOverwrite(_serializedEquippedArmors, equippedArmorsList);
                EquippedArmors.Clear();
                foreach (var armor in equippedArmorsList)
                {
                    EquippedArmors[armor.Type] = armor;
                }
            }
        }
        public void OnBeforeSerialize()
        {
            _serializedInventory = _serializedEquippedWeapon = _serializedEquippedArmors = string.Empty;
            if (InventoryConfig != null)
            {
                _serializedInventory = JsonUtility.ToJson(InventoryConfig, true);
            }
            if (EquippedWeapon != null)
            {
                _serializedEquippedWeapon = JsonUtility.ToJson(EquippedWeapon, true);
            }
            if (EquippedArmors != null)
            {
                var equippedArmorsList = EquippedArmors.Values.ToList();
                _serializedEquippedArmors = JsonUtility.ToJson(equippedArmorsList, true);
            }
        }

        private void OnValidate()
        {
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
            if (InventoryConfig != null)
            {
                if (!InventoryConfig.ItemsSlots.Select(slot => slot.Item).Where(item => item != null && item is InventoryItems.Weapon).Contains(EquippedWeapon))
                {
                    EquippedWeapon = null;
                }
                var armors = InventoryConfig.ItemsSlots.Select(slot => slot.Item).Where(item => item != null && item is InventoryItems.Armor).ToList();
                Dictionary<ArmorType, Armor> equippedArmors = new();
                foreach (var armor in EquippedArmors.Values)
                {
                    if (armors.Contains(armor))
                    {
                        equippedArmors[armor.Type] = armor;
                    }
                }
                EquippedArmors = equippedArmors;
            }
        }
    }
}
