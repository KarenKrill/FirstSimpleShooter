using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditorInternal.VersionControl;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.Progress;

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
    private bool _LastGameIsNotOver
    {
        get => _lastGameIsNotOver;
        set
        {
            if (ContinueMenuButton != null)
            {
                ContinueMenuButton.interactable = value;
            }
            _lastGameIsNotOver = value;
        }
    }
    public Player Player, Enemy;
    public GameItem EnemyWeapon;
    private uint _roundCount = 0;
    public uint RoundsCount = 3;
    public List<GameItem> RewardItems;
    public GameObject PlayerLifeSliderParent;
    public GameObject EnemyLifeSliderParent;
    public Button ContinueMenuButton;
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
                    _LastGameIsNotOver = false;
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
        Player.Stats.Health = Player.Stats.MaxHealth;
        Enemy.Stats.Health = Enemy.Stats.MaxHealth;
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
#if UNITY_EDITOR
    private static readonly string AppDirectory = Application.dataPath;
#else
    private static readonly string AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
#endif
    private string _savedGameDataFileName = System.IO.Path.Combine(AppDirectory, "GameData.json");
    [Serializable]
    public class SavedGameItemStats
    {
        public GameItemStats GameItemStats;
        public string GameItemType;
        public string GameItemJsonSpecificStatsStr;
        public SavedGameItemStats(string gameItemType, GameItemStats gameItemStats, string gameItemJsonSpecificStatsStr)
        {
            GameItemType = gameItemType;
            GameItemStats = gameItemStats;
            GameItemJsonSpecificStatsStr = gameItemJsonSpecificStatsStr;
        }
    }
    [Serializable]
    public class EquippedArmor
    {
        public string Name;
        public ArmorType ArmorType;
        public EquippedArmor(string name, ArmorType armorType)
        {
            Name = name;
            ArmorType = armorType;
        }
    }
    [Serializable]
    public class SaveGameDataStruct
    {
        [SerializeField]
        public GameState GameState;
        [SerializeField]
        public string PlayerStatsJsonStr, EnemyStatsJsonStr;
        [SerializeField]
        public uint RoundCount;
        public List<SavedGameItemStats> PlayerItemsStatsJsonStrs;
        public string EquippedWeaponName;
        public List<EquippedArmor> EquippedArmors;
        public SaveGameDataStruct(GameState gameState, string playerJsonStr, string enemyJsonStr, uint roundCount, List<SavedGameItemStats> playerItemsJsonStrs, string equippedWeapon, List<EquippedArmor> equippedArmors)
        {
            GameState = gameState;
            PlayerStatsJsonStr = playerJsonStr;
            EnemyStatsJsonStr = enemyJsonStr;
            RoundCount = roundCount;
            PlayerItemsStatsJsonStrs = playerItemsJsonStrs;
            EquippedWeaponName = equippedWeapon;
            EquippedArmors = equippedArmors;
        }
    }
    private void SaveGameData()
    {
        var playerJsonStr = JsonUtility.ToJson(Player.Stats, true);
        var enemyJsonStr = JsonUtility.ToJson(Enemy.Stats, true);
        List<SavedGameItemStats> itemsStatsJsonStrs = new();
        var playerInventoryItems = InventoryManager.Instance?.ListItems() ?? null;
        if (playerInventoryItems != null)
        {
            foreach (var item in playerInventoryItems)
            {
                string typeName = item.GetType().Name;
                string specificStats = string.Empty;
                switch (item)
                {
                    case Ammo ammo:
                        specificStats = JsonUtility.ToJson(ammo.AmmoStats);
                        break;
                    case Armor armor:
                        specificStats = JsonUtility.ToJson(armor.ArmorStats);
                        break;
                    case Potion potion:
                        specificStats = JsonUtility.ToJson(potion.PotionStats);
                        break;
                    case Weapon weapon:
                        specificStats = JsonUtility.ToJson(weapon.WeaponStats);
                        break;
                    default:
                        break;
                }
                itemsStatsJsonStrs.Add(new SavedGameItemStats(typeName, item.Stats, specificStats));
            }
        }
        string equippedWeapon = InventoryManager.Instance?.EquippedWeapon?.Stats.Name ?? string.Empty;
        List<EquippedArmor> equippedArmors = new();
        if (InventoryManager.Instance != null)
        {
            foreach (var equippedArmor in InventoryManager.Instance.EquippedArmor)
            {
                equippedArmors.Add(new(equippedArmor.Value.Stats.Name, equippedArmor.Key));
            }
        }
        SaveGameDataStruct serializedFields = new(State, playerJsonStr, enemyJsonStr, _roundCount, itemsStatsJsonStrs, equippedWeapon, equippedArmors);
        var jsonStr = JsonUtility.ToJson(serializedFields, true);
        System.IO.File.WriteAllText(_savedGameDataFileName, jsonStr);
    }
    private void LoadGameData()
    {
        if (System.IO.File.Exists(_savedGameDataFileName))
        {
            var jsonStr = System.IO.File.ReadAllText(_savedGameDataFileName);
            var serializedFields = JsonUtility.FromJson<SaveGameDataStruct>(jsonStr);
            Player.Stats = JsonUtility.FromJson<PlayerStats>(serializedFields.PlayerStatsJsonStr);
            Enemy.Stats = JsonUtility.FromJson<PlayerStats>(serializedFields.EnemyStatsJsonStr);
            _roundCount = serializedFields.RoundCount;
            // load _roundCount
            // load Player and his inventory
            // load Enemy
            // load GameState
            /*JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.General);
            options.Converters.Add(new JsonStringEnumConverter());
            var jsonStr = System.IO.File.ReadAllText(_savedGameDataFileName);
            var serializedFields = System.Text.Json.JsonSerializer.Deserialize<List<(string, object)>>(jsonStr, options);*/
            InventoryManager.Instance?.ClearItems();
            InventoryManager.Instance.EquippedArmor?.Clear();
            InventoryManager.Instance.EquippedWeapon = null;
            Dictionary<string, GameItem> existedGameItemsTemplates = new();
            foreach (var item in Player.Inventory.Items)
            {
                existedGameItemsTemplates[item.Stats.Name] = item;
            }
            List<string> equippedArmors = serializedFields.EquippedArmors.Select((armor) => armor.Name).ToList();
            foreach (var itemJson in serializedFields.PlayerItemsStatsJsonStrs)
            {
                bool isEquippedArmor = false, isEquippedWeapon = false;
                if (existedGameItemsTemplates.TryGetValue(itemJson.GameItemStats.Name, out var gameItem))
                {
                    var cloneItem = Instantiate(gameItem);
                    cloneItem.Stats = itemJson.GameItemStats;
                    switch (itemJson.GameItemType) // правильнее заменить switch на перебор наследников типа GameItem через рефлексию
                    {
                        case nameof(Ammo):
                            ((Ammo)cloneItem).AmmoStats = JsonUtility.FromJson<AmmoStats>(itemJson.GameItemJsonSpecificStatsStr);
                            break;
                        case nameof(Armor):
                            ((Armor)cloneItem).ArmorStats = JsonUtility.FromJson<ArmorStats>(itemJson.GameItemJsonSpecificStatsStr);
                            if (equippedArmors.Contains(cloneItem.Stats.Name))
                            {
                                isEquippedArmor = true;
                            }
                            break;
                        case nameof(Potion):
                            ((Potion)cloneItem).PotionStats = JsonUtility.FromJson<PotionStats>(itemJson.GameItemJsonSpecificStatsStr);
                            break;
                        case nameof(Weapon):
                            ((Weapon)cloneItem).WeaponStats = JsonUtility.FromJson<WeaponStats>(itemJson.GameItemJsonSpecificStatsStr);
                            if (serializedFields.EquippedWeaponName == cloneItem.Stats.Name)
                            {
                                isEquippedWeapon = true;
                            }
                            break;
                        default:
                            break;
                    }
                    InventoryManager.Instance.AddItem(cloneItem);
                    if (isEquippedArmor)
                    {
                        InventoryManager.Instance.EquipArmor((Armor)cloneItem);
                    }
                    else if (isEquippedWeapon)
                    {
                        InventoryManager.Instance.EquipWeapon((Weapon)cloneItem);
                    }
                }
            }
            State = serializedFields.GameState;
        }
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
        UpdateHealSlider(Player, PlayerLifeSliderParent);
        UpdateHealSlider(Enemy, EnemyLifeSliderParent);
    }
    public void ExitToMenu()
    {
        if (State == GameState.PlayerTurn || State == GameState.EnemyTurn)
        {
            SaveGameData();
            _LastGameIsNotOver = true;
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
                var health = player.Stats.Health / player.Stats.MaxHealth * playerLifeSlider.maxValue; // minvalue ignore (must be 0)
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
                var ammoCountPerShoot = weapon.WeaponStats.AmmoCountPerShoot < weapon.WeaponStats.AmmoCount ? weapon.WeaponStats.AmmoCountPerShoot : weapon.WeaponStats.AmmoCount;
                float damage = weapon.WeaponStats.Damage * ammoCountPerShoot;// calc damage
                _playerLastAim = _playerLastAim == ArmorType.Headgear ? ArmorType.Cuirass : ArmorType.Headgear;
                _playerLastAim = _random.Next(0, 3) == 0 ? ArmorType.Headgear : ArmorType.Cuirass;
                damage *= _playerLastAim == ArmorType.Headgear ? Enemy.Stats.HeadDamageMultiplier : Enemy.Stats.BodyDamageMultiplier;
                damage *= ((100f - 5 * (_playerLastAim == ArmorType.Headgear ? Enemy.Stats.HeadDefence : Enemy.Stats.BodyDefence)) / 100f); // armor attack reduction
                weapon.WeaponStats.AmmoCount -= ammoCountPerShoot;
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
                        Enemy.Stats.Health = Enemy.Stats.MaxHealth;
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
                Player.Stats.HeadDefence = headgear.ArmorStats.Defence;
            }
            else
            {
                Player.Stats.HeadDefence = 0;
            }
            if (InventoryManager.Instance.EquippedArmor.TryGetValue(ArmorType.Cuirass, out var cuirass))
            {
                Player.Stats.BodyDefence = cuirass.ArmorStats.Defence;
            }
            else
            {
                Player.Stats.BodyDefence = 0;
            }
        }
        var weapon = (Weapon)EnemyWeapon;
        float damage = weapon.WeaponStats.Damage;
        //_enemyLastAim = _enemyLastAim == ArmorType.Headgear ? ArmorType.Cuirass : ArmorType.Headgear;
        _enemyLastAim = _random.Next(0, 3) == 0 ? ArmorType.Headgear : ArmorType.Cuirass;
        damage *= _enemyLastAim == ArmorType.Headgear ? Player.Stats.HeadDamageMultiplier : Player.Stats.BodyDamageMultiplier;
        damage *= ((100f - 5 * (_enemyLastAim == ArmorType.Headgear ? Player.Stats.HeadDefence : Player.Stats.BodyDefence)) / 100f); // armor attack reduction
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
        _LastGameIsNotOver = System.IO.File.Exists(_savedGameDataFileName);
        /*if (_lastGameIsNotOver)
        {
            LoadGameData();
        }
        else
        {*/
            Player = Instantiate(Player);
            Enemy = Instantiate(Enemy);
        //}
    }
    void Start()
    {
        State = GameState.Menu;
    }
    private void OnDestroy()
    {
        if (State == GameState.PlayerTurn || State == GameState.EnemyTurn)
        {
            //SaveGameData(); // fails
        }
    }
}
