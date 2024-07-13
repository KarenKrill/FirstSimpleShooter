using System;
using System.IO;
using UnityEngine;
using Assets.Scripts.Model;

namespace Assets.Scripts
{
    public class GameDataManager
    {
        private static readonly string AppDirectory = Application.dataPath; // Application.persistentDataPath
        private static readonly string SavedGameDataFileName = Path.Combine(AppDirectory, "GameData.json");
        private GameData _defaultGameData, _gameData;
        private Inventory _inventory;
        private GameDataManager() { }
        private static Lazy<GameDataManager> _instance = new(() => new());
        public static GameDataManager Instance => _instance.Value;
        public GameData GameData => _gameData;
        public void Init(GameData defaultGameData)
        {
            _defaultGameData = defaultGameData;
            ResetToDefaults();
        }
        public bool IsSavedDataExists => File.Exists(SavedGameDataFileName);
        public void Save()
        {
            var serializedGameData = JsonUtility.ToJson(_gameData, true);
            File.WriteAllText(SavedGameDataFileName, serializedGameData);
        }
        public void Load()
        {
            if (IsSavedDataExists)
            {
                var serializedGameData = File.ReadAllText(SavedGameDataFileName);
                JsonUtility.FromJsonOverwrite(serializedGameData, _gameData);
            }
        }
        public void ResetToDefaults()
        {
            Player playerClone = _defaultGameData.Player.Clone();
            Player enemyClone = _defaultGameData.Enemy.Clone();
            if (_gameData != null)
            {
                if (_gameData.Player != null)
                {
                    if (_gameData.Player.InventoryConfig != null)
                    {
                        foreach (var slot in _gameData.Player.InventoryConfig.ItemsSlots)
                        {
                            if (slot != null && slot.Item != null)
                            {
                                UnityEngine.Object.Destroy(slot.Item);
                                slot.Clear();
                            }
                        }
                        UnityEngine.Object.Destroy(_gameData.Player.InventoryConfig);
                    }
                    UnityEngine.Object.Destroy(_gameData.Player);
                }
                if (_gameData.Enemy != null)
                {
                    if (_gameData.Enemy.InventoryConfig != null)
                    {
                        foreach (var slot in _gameData.Enemy.InventoryConfig.ItemsSlots)
                        {
                            if (slot != null && slot.Item != null)
                            {
                                UnityEngine.Object.Destroy(slot.Item);
                                slot.Clear();
                            }
                        }
                        UnityEngine.Object.Destroy(_gameData.Enemy.InventoryConfig);
                    }
                    UnityEngine.Object.Destroy(_gameData.Enemy);
                }
            }
            _gameData = new(_defaultGameData.State, _defaultGameData.RoundNumber, _defaultGameData.RoundsCount, playerClone, enemyClone);
        }
    }
}
