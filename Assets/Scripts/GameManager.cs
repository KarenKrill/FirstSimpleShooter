using Assets.Scripts;
using Assets.Scripts.Model;
using Assets.Scripts.Model.InventoryItems;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public enum GameState
{
    None,
    Menu,
    PlayerTurn,
    PlayerAttack,
    EnemyTurn,
    EnemyAttack,
    RoundWin,
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
[Serializable]
public class GameManager : MonoBehaviour
{
    public Weapon EnemyWeapon;
    public List<InventoryItem> RewardItems;
    public GameObject PlayerLifeSliderParent;
    public GameObject EnemyLifeSliderParent;
    public Button ContinueMenuButton;
    public Button FireButton;
    public InventoryPanel InventoryPanel;
    public TextMeshProUGUI RoundText;
    [SerializeField]
    private Animator _playerAnimator, _enemyAnimator;
    private void OnGameStateChanged(GameState gameState)
    {
        if (gameState != GameState.Menu && ContinueMenuButton != null)
        {
            ContinueMenuButton.interactable = !(gameState == GameState.Win || gameState == GameState.Defeat);
            FireButton.interactable = (gameState == GameState.PlayerTurn);
        }
        else
        {
            string playerEquippedWeaponVarName = "IsPistolEquipped";
            if (GameDataManager.Instance.GameData.Player.EquippedWeapon != null && GameDataManager.Instance.GameData.Player.EquippedWeapon.Name == "AssaultRifle")
            {
                _playerAnimator.SetBool(playerEquippedWeaponVarName, false);
            }
            else
            {
                _playerAnimator.SetBool(playerEquippedWeaponVarName, true);
            }
            switch (gameState)
            {
                case GameState.PlayerTurn:
                    _playerAnimator.SetTrigger("Idle");
                    break;
                case GameState.PlayerAttack:
                    _playerAnimator.SetTrigger("Attack");
                    break;
                case GameState.EnemyTurn:
                    _enemyAnimator.SetTrigger("Idle");
                    break;
                case GameState.EnemyAttack:
                    _enemyAnimator.SetTrigger("Attack");
                    break;
                default:
                    break;
            }
        }
        GameStateChanged?.Invoke(gameState);
    }
    private void OnPlayerHealthChanged(Player player, Slider playerLifeSlider)
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
    private void OnEquippedWeaponChanged(Player player)
    {
        if (player != null)
        {
            if (player.EquippedWeapon != null)
            {
                if (_playerAnimator.isInitialized)
                {
                    _playerAnimator.SetBool("IsPistolEquipped", player.EquippedWeapon.Name == "Pistol");
                    _playerAnimator.SetTrigger("WeaponChanged");
                }
                Debug.Log($"Weapon changed on {player.EquippedWeapon.Name}");
            }
        }
    }

    public event Action<GameState> GameStateChanged;
    public static GameManager Instance;

    public void StartNewGame()
    {
        GameDataManager.Instance.ResetToDefaults();
        RoundText.text = $"Round {GameDataManager.Instance.GameData.RoundNumber + 1}/{GameDataManager.Instance.GameData.RoundsCount}!";
        GameDataManager.Instance.GameData.GameStateChanged += OnGameStateChanged;
        GameDataManager.Instance.GameData.Player.EquippedWeaponChanged += OnEquippedWeaponChanged;
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
        _playerAnimator.SetBool("IsPistolEquipped", GameDataManager.Instance.GameData.Player.EquippedWeapon.Name == "Pistol");
        _playerAnimator.SetTrigger("WeaponChanged");
    }
    public void ContinueGame()
    {
        GameDataManager.Instance.Load();
        RoundText.text = $"Round {GameDataManager.Instance.GameData.RoundNumber + 1}/{GameDataManager.Instance.GameData.RoundsCount}!";
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
        _playerAnimator.SetBool("IsPistolEquipped", GameDataManager.Instance.GameData.Player.EquippedWeapon.Name == "Pistol");
        _playerAnimator.SetTrigger("WeaponChanged");
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
        FireButton.interactable = false;
        _playerAnimator.SetTrigger("Attack");
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
                GameDataManager.Instance.GameData.State = GameState.PlayerAttack;
            }
        }
        else Debug.Log("Weapon isn't equipped!");
    }
    public void EnemyAttack()
    {
        _enemyAnimator.SetTrigger("Attack");
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
        GameDataManager.Instance.GameData.Player.EquippedWeaponChanged += OnEquippedWeaponChanged;
        if (GameDataManager.Instance.GameData.Player != null)
        {
            OnEquippedWeaponChanged(GameDataManager.Instance.GameData.Player);
        }
        if (ContinueMenuButton != null)
        {
            ContinueMenuButton.interactable = GameDataManager.Instance.IsSavedDataExists;
        }
        GameDataManager.Instance.GameData.State = GameState.Menu;
    }
    private void OnApplicationQuit()
    {
        if (GameDataManager.Instance != null && GameDataManager.Instance.GameData != null)
        {
            if (GameDataManager.Instance.GameData.State == GameState.PlayerTurn || GameDataManager.Instance.GameData.State == GameState.EnemyTurn)
            {
                GameDataManager.Instance.Save();
            }
        }
    }
    public float PlayerAttackAnimationDelay = 1;
    float _playerAttackAnimationTime = 0;
    public float EnemyAttackAnimationDelay = 1;
    float _enemyAttackAnimationTime = 0;
    public float EnemyDeathAnimationDelay = 1;
    public float EnemyLastDeathAnimationDelay = 1;
    float _enemyDeathAnimationTime = 0;
    public float PlayerDeathAnimationDelay = 1;
    float _playerDeathAnimationTime = 0;

    private void Update()
    {
        if (GameDataManager.Instance.GameData.State == GameState.PlayerAttack)
        {
            _playerAttackAnimationTime += Time.deltaTime;
            if (_playerAttackAnimationTime >= PlayerAttackAnimationDelay)
            {
                _playerAttackAnimationTime = 0;
                if (EnemyLifeSliderParent != null)
                {
                    OnPlayerHealthChanged(GameDataManager.Instance.GameData.Enemy, EnemyLifeSliderParent.GetComponent<Slider>());
                }
                GameDataManager.Instance.GameData.State = GameState.EnemyTurn;
                if (GameDataManager.Instance.GameData.Enemy.IsDead)
                {
                    _enemyAnimator.SetTrigger("TakeFatalDamage");
                }
            }
        }
        else if (GameDataManager.Instance.GameData.State == GameState.EnemyTurn)
        {
            if (GameDataManager.Instance.GameData.Enemy.IsAlive)
            {
                EnemyAttack();
                GameDataManager.Instance.GameData.State = GameState.EnemyAttack;
            }
            else
            {
                // Wait while plays enemy death animation
                _enemyDeathAnimationTime += Time.deltaTime;
                bool isLastRound = GameDataManager.Instance.GameData.RoundNumber + 1 >= GameDataManager.Instance.GameData.RoundsCount;
                if ((!isLastRound && _enemyDeathAnimationTime >= EnemyDeathAnimationDelay) || (isLastRound && _enemyDeathAnimationTime >= EnemyLastDeathAnimationDelay))
                {
                    _enemyDeathAnimationTime = 0;
                    if (RewardItems != null && RewardItems.Count > 0)
                    {
                        System.Random random = new();
                        int rewardItemIndex = RewardItems.Count == 1 ? 0 : random.Next(0, RewardItems.Count - 1);
                        if (GameDataManager.Instance.DefaultGameData.Player.InventoryConfig.ItemsDatabase.TryGetItemId(RewardItems[rewardItemIndex], out var itemId))
                        {
                            if (GameDataManager.Instance.GameData.Player.InventoryConfig.ItemsDatabase.TryGetItem(itemId, out var rewardItem))
                            {
                                var rewardItemsCount = rewardItem is Ammo ? random.Next(1, 40) : 1;
                                GameDataManager.Instance.GameData.Player.InventoryConfig.AddItem(rewardItem, rewardItemsCount, true);
                            }
                        }
                    }
                    if (++GameDataManager.Instance.GameData.RoundNumber >= GameDataManager.Instance.GameData.RoundsCount)
                    {
                        GameDataManager.Instance.GameData.State = GameState.Win;
                    }
                    else
                    {
                        GameDataManager.Instance.GameData.Enemy.Health = GameDataManager.Instance.GameData.Enemy.MaxHealth;
                        _enemyAnimator.SetTrigger("Resurrect");
                        GameDataManager.Instance.GameData.State = GameState.PlayerTurn;
                        RoundText.text = $"Round {GameDataManager.Instance.GameData.RoundNumber + 1}/{GameDataManager.Instance.GameData.RoundsCount}!";
                    }
                    if (EnemyLifeSliderParent != null)
                    {
                        OnPlayerHealthChanged(GameDataManager.Instance.GameData.Enemy, EnemyLifeSliderParent.GetComponent<Slider>());
                    }
                }
            }
        }
        else if (GameDataManager.Instance.GameData.State == GameState.EnemyAttack)
        {
            _enemyAttackAnimationTime += Time.deltaTime;
            if (_enemyAttackAnimationTime >= EnemyAttackAnimationDelay)
            {
                _enemyAttackAnimationTime = 0;
                if (PlayerLifeSliderParent != null)
                {
                    OnPlayerHealthChanged(GameDataManager.Instance.GameData.Player, PlayerLifeSliderParent.GetComponent<Slider>());
                }
                GameDataManager.Instance.GameData.State = GameState.PlayerTurn;
                if (GameDataManager.Instance.GameData.Player.IsDead)
                {
                    _playerAnimator.SetTrigger("TakeFatalDamage");
                }
            }
        }
        else if (GameDataManager.Instance.GameData.State == GameState.PlayerTurn)
        {
            if (GameDataManager.Instance.GameData.Player.IsDead)
            {
                // Wait while plays player death animation
                _playerDeathAnimationTime += Time.deltaTime;
                if (_playerDeathAnimationTime >= PlayerDeathAnimationDelay)
                {
                    _playerDeathAnimationTime = 0;
                    _playerAnimator.SetTrigger("Resurrect");
                    GameDataManager.Instance.GameData.State = GameState.Defeat;
                }
            }
        }
    }
}
