using Assets.Scripts.Model.InventoryItems;
using System;
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
        public Player Player { get; private set; }
        [SerializeField]
        private string _serializedEnemy;
        public Player Enemy { get; private set; }
        public GameData(GameState state, int roundNumber, int roundsCount, Player player, Player enemy, Weapon weaponTemplate = null, Armor armorTemplate = null, Inventory playerInventory = null, Inventory enemyInventory = null)
        {
            _state = state;
            RoundNumber = roundNumber;
            RoundsCount = roundsCount;
            Player = player;
            Enemy = enemy;
            if (weaponTemplate != null)
            {
                Player.WeaponTemplate = weaponTemplate;
                Enemy.WeaponTemplate = weaponTemplate;
            }
            if (armorTemplate != null)
            {
                Player.ArmorTemplate = armorTemplate;
                Enemy.ArmorTemplate = armorTemplate;
            }
            if (playerInventory != null)
            {
                Player.InventoryConfig = playerInventory;
            }
            if (enemyInventory != null)
            {
                Enemy.InventoryConfig = enemyInventory;
            }
        }

        public void OnAfterDeserialize()
        {
            if (Player != null && !string.IsNullOrEmpty(_serializedPlayer))
            {
                JsonUtility.FromJsonOverwrite(_serializedPlayer, Player);
            }
            if (Enemy != null && !string.IsNullOrEmpty(_serializedEnemy))
            {
                JsonUtility.FromJsonOverwrite(_serializedEnemy, Enemy);
            }
        }
        public void OnBeforeSerialize()
        {
            _serializedPlayer = _serializedEnemy = string.Empty;
            if (Player != null)
            {
                _serializedPlayer = JsonUtility.ToJson(Player, true);
            }
            if (Enemy != null)
            {
                _serializedEnemy = JsonUtility.ToJson(Enemy, true);
            }
        }
    }
}
