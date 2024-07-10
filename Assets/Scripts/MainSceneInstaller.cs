using Assets.Scripts;
using Assets.Scripts.Model;
using Assets.Scripts.Model.InventoryItems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MainSceneInstaller : MonoBehaviour
{
    [SerializeField]
    private Player _playerDefaultConfig;
    [SerializeField]
    private Inventory _playerDefaultInventory;
    [SerializeField]
    private Inventory _enemyDefaultInventory;
    [SerializeField]
    private Player _enemyDefaultConfig;
    [SerializeField]
    private Weapon _weaponTemplate;
    [SerializeField]
    private Armor _armorTemplate;
    [SerializeField]
    private int _roundsCount = 3;
    bool _initializedIsFinished = false;
    private void Awake()
    {
        GameDataManager.Instance.Init(new(GameState.Menu, 0, _roundsCount, _playerDefaultConfig, _enemyDefaultConfig, _weaponTemplate, _armorTemplate, _playerDefaultInventory, _enemyDefaultInventory));
    }
    private void Update()
    {
        if (!_initializedIsFinished)
        {
            try
            {
                GameManager.Instance.Init();
                _initializedIsFinished = true;
                //Destroy(gameObject);
            }
            catch (Exception ex)
            {
                Debug.LogError($"{nameof(GameManager)} initialization failed with {ex.GetType()}:{Environment.NewLine}{ex}");
            }
        }
    }
}
