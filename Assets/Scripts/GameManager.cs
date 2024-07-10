using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;
using Assets.Scripts.Model.InventoryItems;
using Assets.Scripts.Model;

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
    public Weapon EnemyWeapon;
    public List<Assets.Scripts.Model.InventoryItems.InventoryItem> RewardItems;
    public GameObject PlayerLifeSliderParent;
    public GameObject EnemyLifeSliderParent;
    public Button ContinueMenuButton;
    public InventoryPanel InventoryPanel;
    private void OnGameStateChanged(GameState gameState)
    {
        if (gameState != GameState.Menu && ContinueMenuButton != null)
        {
            ContinueMenuButton.interactable = !(gameState == GameState.Win || gameState == GameState.Defeat);
        }
        GameStateChanged?.Invoke(gameState);
    }
    private void OnPlayerHealthChanged(Assets.Scripts.Model.Player player, Slider playerLifeSlider)
    {
        if (playerLifeSlider != null)
        {
            if (player.IsAlive)
            {
                var normalizedHealth = player.Health / player.MaxHealth * playerLifeSlider.maxValue; // minvalue ignore (must be 0)
                playerLifeSlider.value = normalizedHealth;
            }
            else playerLifeSlider.value = 0;
        }
    }
    private void OnInventoryChanged(Inventory inventory)
    {
        InventoryPanel.Inventory = inventory;
    }
    
    public event Action<GameState> GameStateChanged;
    public static GameManager Instance;

    public void StartNewGame()
    {
        GameDataManager.Instance.ResetToDefaults();
        GameDataManager.Instance.GameData.GameStateChanged += OnGameStateChanged;
        if (PlayerLifeSliderParent != null)
        {
            OnPlayerHealthChanged(GameDataManager.Instance.GameData.Player, PlayerLifeSliderParent.GetComponent<Slider>());
        }
        if (EnemyLifeSliderParent != null)
        {
            OnPlayerHealthChanged(GameDataManager.Instance.GameData.Enemy, EnemyLifeSliderParent.GetComponent<Slider>());
        }
        OnInventoryChanged(GameDataManager.Instance.GameData.Player.InventoryConfig);
        GameDataManager.Instance.GameData.State = GameState.PlayerTurn;
    }
    public void ContinueGame()
    {
        GameDataManager.Instance.Load();
        if (PlayerLifeSliderParent != null)
        {
            OnPlayerHealthChanged(GameDataManager.Instance.GameData.Player, PlayerLifeSliderParent.GetComponent<Slider>());
        }
        if (EnemyLifeSliderParent != null)
        {
            OnPlayerHealthChanged(GameDataManager.Instance.GameData.Enemy, EnemyLifeSliderParent.GetComponent<Slider>());
        }
        OnInventoryChanged(GameDataManager.Instance.GameData.Player.InventoryConfig);
        OnGameStateChanged(GameDataManager.Instance.GameData.State);
    }
    public void ExitToMenu()
    {
        if (GameDataManager.Instance.GameData.State == GameState.PlayerTurn || GameDataManager.Instance.GameData.State == GameState.EnemyTurn)
        {
            GameDataManager.Instance.Save();
        }
        GameDataManager.Instance.GameData.State = GameState.Menu;
    }
    public void Exit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    
    private ArmorType _playerLastAim, _enemyLastAim;
    private System.Random _random = new();
    public void Attack()
    {
        var weapon = GameDataManager.Instance.GameData.Player.EquippedWeapon;
        if (weapon != null)
        {
            if (GameDataManager.Instance.GameData.State == GameState.PlayerTurn)
            {
                var ammoCountPerShoot = weapon.AmmoCountPerShoot < weapon.AmmoCount ? weapon.AmmoCountPerShoot : weapon.AmmoCount;
                float damage = weapon.Damage * ammoCountPerShoot;// calc damage
                _playerLastAim = _playerLastAim == ArmorType.Headgear ? ArmorType.Cuirass : ArmorType.Headgear;
                _playerLastAim = _random.Next(0, 3) == 0 ? ArmorType.Headgear : ArmorType.Cuirass;
                damage *= _playerLastAim == ArmorType.Headgear ? GameDataManager.Instance.GameData.Enemy.HeadDamageMultiplier : GameDataManager.Instance.GameData.Enemy.BodyDamageMultiplier;
                damage *= ((100f - 5 * (_playerLastAim == ArmorType.Headgear ? GameDataManager.Instance.GameData.Enemy.HeadDefence : GameDataManager.Instance.GameData.Enemy.BodyDefence)) / 100f); // armor attack reduction
                weapon.AmmoCount -= ammoCountPerShoot;
                GameDataManager.Instance.GameData.Enemy.Damage(damage);
                if (EnemyLifeSliderParent != null)
                {
                    OnPlayerHealthChanged(GameDataManager.Instance.GameData.Enemy, EnemyLifeSliderParent.GetComponent<Slider>());
                }
                if (GameDataManager.Instance.GameData.Enemy.IsAlive)
                {
                    GameDataManager.Instance.GameData.State = GameState.EnemyTurn;
                    EnemyAttack();
                }
                else
                {
                    if (RewardItems != null && RewardItems.Count > 0)
                    {
                        System.Random random = new();
                        int rewardItemIndex = RewardItems.Count == 1 ? 0 : random.Next(0, RewardItems.Count - 1);
                        var rewardItem = Instantiate(RewardItems[rewardItemIndex]);
                        var rewardItemsCount = rewardItem is Ammo ? random.Next(1, 40) : 1;
                        GameDataManager.Instance.GameData.Player.InventoryConfig.AddItem(rewardItem, rewardItemsCount);
                    }
                    if (++GameDataManager.Instance.GameData.RoundNumber >= GameDataManager.Instance.GameData.RoundsCount)
                    {
                        GameDataManager.Instance.GameData.State = GameState.Win;
                    }
                    else
                    {
                        GameDataManager.Instance.GameData.Enemy.Health = GameDataManager.Instance.GameData.Enemy.MaxHealth;
                        Debug.Log($"Round {GameDataManager.Instance.GameData.RoundNumber + 1}/{GameDataManager.Instance.GameData.RoundsCount + 1}!");
                    }
                    if (EnemyLifeSliderParent != null)
                    {
                        OnPlayerHealthChanged(GameDataManager.Instance.GameData.Enemy, EnemyLifeSliderParent.GetComponent<Slider>());
                    }
                }
            }
        }
        else Debug.Log("Weapon isn't equipped!");
    }
    public void EnemyAttack()
    {
        if (GameDataManager.Instance.GameData.Player.EquippedArmors != null)
        {
            if (GameDataManager.Instance.GameData.Player.EquippedArmors.TryGetValue(ArmorType.Headgear, out var headgear) && headgear != null)
            {
                GameDataManager.Instance.GameData.Player.HeadDefence = headgear.Defence;
            }
            else GameDataManager.Instance.GameData.Player.HeadDefence = 0;
            if (GameDataManager.Instance.GameData.Player.EquippedArmors.TryGetValue(ArmorType.Cuirass, out var cuirass) && cuirass != null)
            {
                GameDataManager.Instance.GameData.Player.BodyDefence = cuirass.Defence;
            }
            else GameDataManager.Instance.GameData.Player.HeadDefence = 0;
        }
        var weapon = EnemyWeapon;
        float damage = weapon.Damage;
        //_enemyLastAim = _enemyLastAim == ArmorType.Headgear ? ArmorType.Cuirass : ArmorType.Headgear;
        _enemyLastAim = _random.Next(0, 3) == 0 ? ArmorType.Headgear : ArmorType.Cuirass;
        damage *= _enemyLastAim == ArmorType.Headgear ? GameDataManager.Instance.GameData.Player.HeadDamageMultiplier : GameDataManager.Instance.GameData.Player.BodyDamageMultiplier;
        damage *= ((100f - 5 * (_enemyLastAim == ArmorType.Headgear ? GameDataManager.Instance.GameData.Player.HeadDefence : GameDataManager.Instance.GameData.Player.BodyDefence)) / 100f); // armor attack reduction
        GameDataManager.Instance.GameData.Player.Damage(damage);
        if (PlayerLifeSliderParent != null)
        {
            OnPlayerHealthChanged(GameDataManager.Instance.GameData.Player, PlayerLifeSliderParent.GetComponent<Slider>());
        }
        if (GameDataManager.Instance.GameData.Player.IsDead)
        {
            GameDataManager.Instance.GameData.State = GameState.Defeat;
        }
        else GameDataManager.Instance.GameData.State = GameState.PlayerTurn;
    }
    public void Heal(float hp)
    {
        GameDataManager.Instance.GameData.Player.Heal(hp);
        if (PlayerLifeSliderParent != null)
        {
            OnPlayerHealthChanged(GameDataManager.Instance.GameData.Player, PlayerLifeSliderParent.GetComponent<Slider>());
        }
    }

    private void Awake()
    {
        Instance = this;
    }
    public void Init()
    {
        GameDataManager.Instance.GameData.GameStateChanged += OnGameStateChanged;
        if (ContinueMenuButton != null)
        {
            ContinueMenuButton.interactable = GameDataManager.Instance.IsSavedDataExists;
        }
        GameDataManager.Instance.GameData.State = GameState.Menu;
    }
    private void OnApplicationQuit()
    {
        if (GameDataManager.Instance.GameData.State == GameState.PlayerTurn || GameDataManager.Instance.GameData.State == GameState.EnemyTurn)
        {
            GameDataManager.Instance.Save();
        }
    }
}
