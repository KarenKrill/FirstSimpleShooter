using Assets.Scripts.Model.InventoryItems;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.EventSystems.EventTrigger;

namespace Assets.Scripts.Model
{
    [Serializable]
    [CreateAssetMenu(fileName = nameof(Player), menuName = nameof(Player))]
    public class Player : ScriptableObject
    {
        [field: SerializeField]
        public string Name { get; set; }

        [SerializeField, Min(0.001f)]
        private float _health = 100;
        public float Health
        {
            get => _health;
            set
            {
                if (value > MaxHealth)
                {
                    value = MaxHealth;
                }
                else if (value < 0)
                {
                    value = 0;
                }
                if (_health != value)
                {
                    _health = value;
                    HealthChanged?.Invoke(this);
                }
            }
        }
        public Action<Player> HealthChanged;
        [SerializeField, Min(0.001f)]
        private float _maxHealth = 100;
        public float MaxHealth
        {
            get => _maxHealth;
            set
            {
                if (value < 0)
                {
                    value = 0.001f;
                }
                if (_maxHealth != value)
                {
                    _maxHealth = value;
                    if (_health > _maxHealth)
                    {
                        _health = _maxHealth;
                    }
                    MaxHealthChanged?.Invoke(this);
                }
            }
        }
        public Action<Player> MaxHealthChanged;
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
        [field: SerializeField]
        public Inventory InventoryConfig { get; set; }
        [field: SerializeField]
        private Weapon _equippedWeapon;
        public Weapon EquippedWeapon
        {
            get => _equippedWeapon;
            set
            {
                if (_equippedWeapon != value)
                {
                    if (_equippedWeapon != null)
                    {
                        _equippedWeapon.IsEquipped = false;
                    }
                    if (value != null)
                    {
                        value.IsEquipped = true;
                    }
                    _equippedWeapon = value;
                }
            }
        }
        [field: SerializeField]
        public Dictionary<ArmorType, Armor> EquippedArmors = new();

        private void OnValidate()
        {
            if (Health > MaxHealth)
            {
                Health = MaxHealth;
            }
            if (InventoryConfig != null)
            {
                if (EquippedWeapon != null)
                {
                    if (!InventoryConfig.ItemsSlots.Select(slot => slot.Item).Where(item => item != null && item is Weapon).Contains(EquippedWeapon))
                    {
                        EquippedWeapon.IsEquipped = false;
                        EquippedWeapon = null;
                    }
                    else EquippedWeapon.IsEquipped = true;
                }
                var armors = InventoryConfig.ItemsSlots.Select(slot => slot.Item).Where(item => item != null && item is Armor).ToList();
                Dictionary<ArmorType, Armor> equippedArmors = new();
                foreach (var armor in EquippedArmors.Values)
                {
                    if (armors.Contains(armor))
                    {
                        equippedArmors[armor.Type] = armor;
                        armor.IsEquipped = true;
                    }
                    else armor.IsEquipped = false;
                }
                EquippedArmors = equippedArmors;
            }
        }

        public Player Clone()
        {
            var clone = Instantiate(this);
            if (InventoryConfig != null)
            {
                clone.InventoryConfig = Instantiate(InventoryConfig);
                if(InventoryConfig.ItemsDatabase != null)
                {
                    clone.InventoryConfig.ItemsDatabase = Instantiate(InventoryConfig.ItemsDatabase);
                    Dictionary<int, InventoryItem> cloneItemsMap = new();
                    for (int i = 0; i < InventoryConfig.ItemsSlots.Count; i++)
                    {
                        var originalSlot = InventoryConfig.ItemsSlots[i];
                        if (originalSlot != null)
                        {
                            var cloneSlot = clone.InventoryConfig.ItemsSlots[i];
                            if (originalSlot.Item != null)
                            {
                                if (cloneItemsMap.TryGetValue(cloneSlot.ItemId, out var alreadyClonedItem))
                                {
                                    // Populate constant values ​​for all items of this type:
                                    cloneSlot.Item.Icon = alreadyClonedItem.Icon;
                                }
                                else
                                {
                                    cloneSlot.Item = Instantiate(originalSlot.Item);
                                    if (!clone.InventoryConfig.ItemsDatabase.TryReplaceItem(originalSlot.Item, cloneSlot.Item))
                                    {
                                        Debug.LogWarning($"{nameof(Player)}.{nameof(Clone)}: Can't replace {originalSlot.Item.name} on {cloneSlot.Item.name}");
                                    }
                                    else cloneItemsMap[cloneSlot.ItemId] = cloneSlot.Item;
                                }
                                if (originalSlot.Item is Weapon weapon && weapon == EquippedWeapon)
                                {
                                    clone.EquippedWeapon = (Weapon)cloneSlot.Item;
                                }
                                else if (originalSlot.Item is Armor originalArmor && EquippedArmors.TryGetValue(originalArmor.Type, out var equippedArmor) && originalArmor == equippedArmor)
                                {
                                    clone.EquippedArmors[originalArmor.Type] = (Armor)cloneSlot.Item;
                                }
                            }
                        }
                    }
                }
            }
            return clone;
        }
    }
}
