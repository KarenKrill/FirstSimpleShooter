using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public enum GameState
{
    None,
    Menu,
    PlayerTurn,
    EnemyTurn,
    Win,
    Defeat
}
[Serializable]
public enum PlayerChangeType
{
    Inventory,
    Health,
    Defence,
    Weapon
}
public class GameManager : MonoBehaviour
{
    private GameState _state;
    private bool _lastGameIsNotOver = false;
    public Player Player, Enemy;
    public GameItem EnemyWeapon;
    private uint _roundCount = 0;
    public uint RoundsCount = 3;
    public List<GameItem> RewardItems;
    public GameObject PlayerLifeSliderParent;
    public GameObject EnemyLifeSliderParent;
    public GameState State
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;
                GameStateChanged?.Invoke(value);
                if (_state == GameState.Defeat || _state == GameState.Win)
                {
                    TryRemoveSavedGameData();
                }
            }
        }
    }
    public event Action<GameState> GameStateChanged;
    public void SetState(GameState state) => State = state;
    public static GameManager Instance;
    public void StartNewGame()
    {
        _roundCount = 0;
        Player.Health = Player.MaxHealth;
        Enemy.Health = Enemy.MaxHealth;
        UpdateHealSlider(Player, PlayerLifeSliderParent);
        UpdateHealSlider(Enemy, EnemyLifeSliderParent);
        State = GameState.PlayerTurn;
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.ClearItems();
            InventoryManager.Instance.EquippedArmor?.Clear();
            InventoryManager.Instance.EquippedWeapon = null;
            foreach (var item in Player.Inventory.Items)
            {
                if (item != null)
                {
                    var cloneItem = Instantiate(item);
                    InventoryManager.Instance.AddItem(cloneItem);
                }
            }
        }
    }
    private string _savedGameDataFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GameData.bin");
    private void LoadGameData()
    {
        if (System.IO.File.Exists(_savedGameDataFileName))
        {
            // load _roundCount
            // load Player and his inventory
            // load Enemy
            // load GameState
            /*JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.General);
            options.Converters.Add(new JsonStringEnumConverter());
            var jsonStr = System.IO.File.ReadAllText(_savedGameDataFileName);
            var serializedFields = System.Text.Json.JsonSerializer.Deserialize<List<(string, object)>>(jsonStr, options);*/
            InventoryManager.Instance?.ClearItems();
            foreach (var item in Player.Inventory.Items)
            {
                if (item != null)
                {
                    var cloneItem = Instantiate(item);
                    InventoryManager.Instance?.AddItem(cloneItem);
                }
            }
        }
    }
    /*[Serializable]
    public class SavedGameItem
    {
        public string GameItemType;
        public string GameItemJsonStr;
        public SavedGameItem(string gameItemType, string gameItemJsonStr)
        {
            GameItemType = gameItemType;
            GameItemJsonStr = gameItemJsonStr;
        }
    }
    [Serializable]
    public class SaveGameDataStruct
    {
        [SerializeField]
        public GameState GameState;
        [SerializeField]
        public string PlayerJsonStr, EnemyJsonStr;
        [SerializeField]
        public uint RoundCount;
        public List<SavedGameItem> PlayerItemsJsonStrs;
        public SaveGameDataStruct(GameState gameState, string playerJsonStr, string enemyJsonStr, uint roundCount, List<SavedGameItem> playerItemsJsonStrs)
        {
            GameState = gameState;
            PlayerJsonStr = playerJsonStr;
            EnemyJsonStr = enemyJsonStr;
            RoundCount = roundCount;
            PlayerItemsJsonStrs = playerItemsJsonStrs;
        }
    }*/
    private void SaveGameData()
    {
        // save _roundCount
        // update Player inventory with InventoryManager
        // save Player and his inventory
        // save Enemy
        // save GameState
        //Dictionary<string, object> serializedFields = new() { { "RoundCount", _roundCount }, { "Player", Player }, { "Enemy", Enemy }, { "GameState", State } };
        //List<(string, object)> serializedFields = new() { ("RoundCount", _roundCount), ("Player", Player), ("Enemy", Enemy), ("GameState", State) };
        /*var playerJsonStr = JsonUtility.ToJson(Player, true);
        var enemyJsonStr = JsonUtility.ToJson(Player, true);
        List<SavedGameItem> itemsJsonStrs = new();
        foreach(var item in Player.Inventory.Items)
        {
            switch(item)
            {
                case Ammo ammo:
                    itemsJsonStrs.Add(new SavedGameItem(nameof(Ammo), JsonUtility.ToJson(ammo)));
                    break;
                case Armor armor:
                    itemsJsonStrs.Add(new SavedGameItem(nameof(Armor), JsonUtility.ToJson(armor)));
                    break;
                case Potion potion:
                    itemsJsonStrs.Add(new SavedGameItem(nameof(Potion), JsonUtility.ToJson(potion)));
                    break;
                case Weapon weapon:
                    itemsJsonStrs.Add(new SavedGameItem(nameof(Weapon), JsonUtility.ToJson(weapon)));
                    break;
                default:
                    break;
            }
        }
        SaveGameDataStruct serializedFields = new(State, playerJsonStr, enemyJsonStr, _roundCount, itemsJsonStrs);
        //var jsonStr = JsonUtility.ToJson(Player);
        var jsonStr = JsonUtility.ToJson(serializedFields, true);
        System.IO.File.WriteAllText(_savedGameDataFileName, jsonStr);*/
    }
    private void TryRemoveSavedGameData()
    {
        if (System.IO.File.Exists(_savedGameDataFileName))
        {
            System.IO.File.Delete(_savedGameDataFileName);
        }
    }
    public void ContinueGame()
    {
        LoadGameData();
    }
    public void ExitToMenu()
    {
        if (State == GameState.PlayerTurn || State == GameState.EnemyTurn)
        {
            SaveGameData();
        }
        State = GameState.Menu;
    }
    public void Exit()
    {
        if (State == GameState.PlayerTurn || State == GameState.EnemyTurn)
        {
            SaveGameData();
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    private static void UpdateHealSlider(Player player, GameObject sliderParent)
    {
        var playerLifeSlider = sliderParent?.GetComponent<Slider>();
        if (playerLifeSlider != null)
        {
            if (player.IsAlive)
            {
                var health = player.Health / player.MaxHealth * playerLifeSlider.maxValue; // minvalue ignore (must be 0)
                playerLifeSlider.value = health;
            }
            else playerLifeSlider.value = 0;
        }
    }
    public ArmorType _playerLastAim, _enemyLastAim;
    private System.Random _random = new();
    public void Attack()
    {
        if (InventoryManager.Instance?.EquippedWeapon != null)
        {
            var weapon = InventoryManager.Instance.EquippedWeapon;
            if (_state == GameState.PlayerTurn)
            {
                var ammoCountPerShoot = weapon.AmmoCountPerShoot < weapon.AmmoCount ? weapon.AmmoCountPerShoot : weapon.AmmoCount;
                float damage = weapon.Damage * ammoCountPerShoot;// calc damage
                _playerLastAim = _playerLastAim == ArmorType.Headgear ? ArmorType.Cuirass : ArmorType.Headgear;
                _playerLastAim = _random.Next(0, 3) == 0 ? ArmorType.Headgear : ArmorType.Cuirass;
                damage *= _playerLastAim == ArmorType.Headgear ? Enemy.HeadDamageMultiplier : Enemy.BodyDamageMultiplier;
                damage *= ((100f - 5 * (_playerLastAim == ArmorType.Headgear ? Enemy.HeadDefence : Enemy.BodyDefence)) / 100f); // armor attack reduction
                weapon.AmmoCount -= ammoCountPerShoot;
                Enemy.Damage(damage);
                UpdateHealSlider(Enemy, EnemyLifeSliderParent);
                if (Enemy.IsAlive)
                {
                    State = GameState.EnemyTurn;
                    EnemyAttack();
                }
                else
                {
                    if (RewardItems != null && RewardItems.Count > 0)
                    {
                        System.Random random = new();
                        int rewardItemIndex = RewardItems.Count == 1 ? 0 : random.Next(0, RewardItems.Count - 1);
                        var rewardItem = Instantiate(RewardItems[rewardItemIndex]);
                        InventoryManager.Instance.AddItem(rewardItem);
                    }
                    if (++_roundCount >= RoundsCount)
                    {
                        State = GameState.Win;
                    }
                    else
                    {
                        Enemy.Health = Enemy.MaxHealth;
                        Debug.Log($"Round {_roundCount+1}/{RoundsCount+1}!");
                    }

                    UpdateHealSlider(Enemy, EnemyLifeSliderParent);
                }
            }
        }
        else Debug.Log("Weapon isn't equipped!");
    }
    public void EnemyAttack()
    {
        if (InventoryManager.Instance.EquippedArmor != null)
        {
            if (InventoryManager.Instance.EquippedArmor.TryGetValue(ArmorType.Headgear, out var headgear))
            {
                Player.HeadDefence = headgear.Defence;
            }
            if (InventoryManager.Instance.EquippedArmor.TryGetValue(ArmorType.Cuirass, out var cuirass))
            {
                Player.BodyDefence = cuirass.Defence;
            }
        }
        var weapon = (Weapon)EnemyWeapon;
        float damage = weapon.Damage;
        //_enemyLastAim = _enemyLastAim == ArmorType.Headgear ? ArmorType.Cuirass : ArmorType.Headgear;
        _enemyLastAim = _random.Next(0, 3) == 0 ? ArmorType.Headgear : ArmorType.Cuirass;
        damage *= _enemyLastAim == ArmorType.Headgear ? Player.HeadDamageMultiplier : Player.BodyDamageMultiplier;
        damage *= ((100f - 5 * (_enemyLastAim == ArmorType.Headgear ? Player.HeadDefence : Player.BodyDefence)) / 100f); // armor attack reduction
        Player.Damage(damage);
        UpdateHealSlider(Player, PlayerLifeSliderParent);
        if (Player.IsDead)
        {
            State = GameState.Defeat;
        }
        else State = GameState.PlayerTurn;
    }
    public void Heal(float hp)
    {
        Player.Heal(hp);
        UpdateHealSlider(Player, PlayerLifeSliderParent);
    }
    void Awake()
    {
        Instance = this;
        _lastGameIsNotOver = System.IO.File.Exists(_savedGameDataFileName);
        if (_lastGameIsNotOver)
        {
            LoadGameData();
        }
        else
        {
            Player = Instantiate(Player);
            Enemy = Instantiate(Enemy);
        }
    }
    void Start()
    {
        State = GameState.Menu;
    }
}
