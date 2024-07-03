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
            }
        }
    }
    public event Action<GameState> GameStateChanged;
    public void SetState(GameState state) => State = state;
    public static GameManager Instance;
    public void StartNewGame()
    {
        _roundCount = 0;
        //Player.Health = Player.MaxHealth;
        //Enemy.Health = Enemy.MaxHealth;
        UpdateHealSlider(Player, PlayerLifeSliderParent);
        UpdateHealSlider(Enemy, EnemyLifeSliderParent);
        State = GameState.PlayerTurn;
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
    public void ContinueGame()
    {
        State = GameState.PlayerTurn;
    }
    public void ExitToMenu()
    {
        // SaveData
        State = GameState.Menu;
    }
    public void Exit()
    {
        // Save data
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void UseItem(int playerId, int itemId)
    {
        //if item consumable
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
    public void Attack()
    {
        if (InventoryManager.Instance.EquippedWeapon != null)
        {
            var weapon = InventoryManager.Instance.EquippedWeapon;
            if (_state == GameState.PlayerTurn)
            {
                var ammoCountPerShoot = weapon.AmmoCountPerShoot < weapon.AmmoCount ? weapon.AmmoCountPerShoot : weapon.AmmoCount;
                float damage = weapon.Damage * ammoCountPerShoot;// calc damage
                _playerLastAim = _playerLastAim == ArmorType.Headgear ? ArmorType.Cuirass : ArmorType.Headgear;
                damage *= _playerLastAim == ArmorType.Headgear ? Player.HeadDamageMultiplier : Player.BodyDamageMultiplier;
                damage *= ((100f - 5 * (_playerLastAim == ArmorType.Headgear ? Player.HeadDefence : Player.BodyDefence)) / 100f); // armor attack reduction
                weapon.AmmoCount -= ammoCountPerShoot;
                Enemy.Damage(damage);
                UpdateHealSlider(Enemy, EnemyLifeSliderParent);
                if (Enemy.IsAlive)
                {
                    State = GameState.EnemyTurn;
                    EnemyAttack();
                }
                else if(RewardItems != null && RewardItems.Count > 0)
                {
                    System.Random random = new();
                    int rewardItemIndex = RewardItems.Count == 1 ? 0 : random.Next(0, RewardItems.Count - 1);
                    var rewardItem = Instantiate(RewardItems[rewardItemIndex]);
                    InventoryManager.Instance.AddItem(rewardItem);
                    if (_roundCount++ >= RoundsCount)
                    {
                        State = GameState.Win;
                    }
                    else Enemy.Health = Enemy.MaxHealth;
                }
            }
        }
        else Debug.Log("Weapon isn't equipped!");
    }
    public void EnemyAttack()
    {
        var weapon = (Weapon)EnemyWeapon;
        float damage = weapon.Damage;
        _enemyLastAim = _enemyLastAim == ArmorType.Headgear ? ArmorType.Cuirass : ArmorType.Headgear;
        damage *= _enemyLastAim == ArmorType.Headgear ? Enemy.HeadDamageMultiplier : Enemy.BodyDamageMultiplier;
        damage *= ((100f - 5 * (_enemyLastAim == ArmorType.Headgear ? Enemy.HeadDefence : Enemy.BodyDefence)) / 100f); // armor attack reduction
        Player.Damage(damage);
        UpdateHealSlider(Player, PlayerLifeSliderParent);
        if (Player.IsDead)
        {
            State = GameState.Defeat;
        }
    }
    public void Heal(float hp)
    {
        Player.Heal(hp);
        UpdateHealSlider(Player, PlayerLifeSliderParent);
    }
    void Awake()
    {
        Instance = this;
        Player = Instantiate(Player);
        Enemy = Instantiate(Enemy);
    }
    void Start()
    {
        State = GameState.Menu;
    }
}
