using Assets.Scripts.Model.InventoryItems;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Model
{
    [Serializable]
    public class GameData : ISerializationCallbackReceiver
    {
        [SerializeField]
        private GameState _state;
        public GameState State
        {
            get => _state;
            set
            {
                if (_state != value)
                {
                    _state = value;
                    GameStateChanged?.Invoke(_state);
                }
            }
        }
        public event Action<GameState> GameStateChanged;
        [SerializeField]
        public int RoundNumber;
        [SerializeField]
        public int RoundsCount;
        [SerializeField]
        private string _serializedPlayer;
        [SerializeField]
        private string _serializedPlayerInventory;
        [SerializeField]
        private string[] _serializedPlayerInventoryItems;
        public Player Player { get; private set; }
        [SerializeField]
        private string _serializedEnemy;
        [SerializeField]
        private string _serializedEnemyInventory;
        [SerializeField]
        private string[] _serializedEnemyInventoryItems;
        public Player Enemy { get; private set; }
        public GameData(GameState state, int roundNumber, int roundsCount, Player player, Player enemy)
        {
            _state = state;
            RoundNumber = roundNumber;
            RoundsCount = roundsCount;
            Player = player;
            Enemy = enemy;
        }

        public void OnAfterDeserialize()
        {
            static void DeserializePlayerDynamicObjects(Player player, string serializedPlayer, string serializedInventory, string[] serializedInventoryItems)
            {
                if (player != null && !string.IsNullOrEmpty(serializedPlayer))
                {
                    var inventoryBackup = player.InventoryConfig;
                    JsonUtility.FromJsonOverwrite(serializedPlayer, player); // will reset the InventoryConfig
                    if (inventoryBackup != null && !string.IsNullOrEmpty(serializedInventory))//serializedInventoryItems != null && serializedInventoryItems.Length > 0)
                    {
                        player.InventoryConfig = inventoryBackup;
                        var itemsDatabaseBackup = player.InventoryConfig.ItemsDatabase;
                        JsonUtility.FromJsonOverwrite(serializedInventory, player.InventoryConfig); // will reset items and database
                        player.InventoryConfig.ItemsDatabase = itemsDatabaseBackup;
                        int itemIndex = 0;
                        foreach (var slot in player.InventoryConfig.ItemsSlots)
                        {
                            if (itemIndex >= serializedInventoryItems.Length)
                            {
                                break;
                            }
                            var serializedPlayerInventoryItem = serializedInventoryItems[itemIndex++];
                            if (slot.Item == null && player.InventoryConfig.ItemsDatabase != null)
                            {
                                if (!player.InventoryConfig.ItemsDatabase.TryGetItem(slot.ItemId, out slot.Item))
                                {
                                    Debug.LogWarning($"{nameof(GameData)}.{nameof(OnAfterDeserialize)}: can't find {nameof(InventoryItem)} {slot.ItemId} in {nameof(Player)} \"{player.Name}\" {nameof(player.InventoryConfig.ItemsDatabase)}");
                                }
                            }
                            if (slot.Item != null && !string.IsNullOrEmpty(serializedPlayerInventoryItem))
                            {
                                var iconBackup = slot.Item.Icon;
                                JsonUtility.FromJsonOverwrite(serializedPlayerInventoryItem, slot.Item);
                                slot.Item.Icon = iconBackup;
                                if (slot.Item.IsEquipped)
                                {
                                    if (slot.Item is Weapon weapon)
                                    {
                                        player.EquippedWeapon = weapon;
                                    }
                                    else if (slot.Item is Armor armor)
                                    {
                                        player.EquippedArmors[armor.Type] = armor;
                                        armor.IsEquipped = true;
                                    }
                                 }
                            }
                        }
                    }
                }
            }
            DeserializePlayerDynamicObjects(Player, _serializedPlayer, _serializedPlayerInventory, _serializedPlayerInventoryItems);
            DeserializePlayerDynamicObjects(Enemy, _serializedEnemy, _serializedEnemyInventory, _serializedEnemyInventoryItems);
        }
        public void OnBeforeSerialize()
        {
            _serializedPlayer = _serializedEnemy = string.Empty;
            _serializedPlayerInventory = _serializedEnemyInventory = string.Empty;
            _serializedPlayerInventoryItems = _serializedEnemyInventoryItems = Array.Empty<string>();
            static void SerializePlayerDynamicObjects(Player player, out string serializedPlayer, out string serializedInventory, out string[] serializedInventoryItems)
            {
                serializedPlayer = JsonUtility.ToJson(player, true);
                List<int> equippedArmors = new();
                if (player.InventoryConfig != null)
                {
                    serializedInventory = JsonUtility.ToJson(player.InventoryConfig, true);
                    serializedInventoryItems = new string[player.InventoryConfig.ItemsSlots.Count];
                    int itemIndex = 0;
                    foreach (var slot in player.InventoryConfig.ItemsSlots)
                    {
                        serializedInventoryItems[itemIndex++] = (slot != null && slot.Item != null) ? JsonUtility.ToJson(slot.Item, true) : string.Empty;
                    }
                }
                else
                {
                    serializedInventory = string.Empty;
                    serializedInventoryItems = Array.Empty<string>();
                }
            }
            if (Player != null)
            {
                SerializePlayerDynamicObjects(Player, out _serializedPlayer, out _serializedPlayerInventory, out _serializedPlayerInventoryItems);
            }
            if (Enemy != null)
            {
                SerializePlayerDynamicObjects(Enemy, out _serializedEnemy, out _serializedEnemyInventory, out _serializedEnemyInventoryItems);
            }
        }
    }
}
