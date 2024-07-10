using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MainSceneInstaller : MonoBehaviour
{
    [SerializeField]
    private Assets.Scripts.Model.Player _playerConfig;
    [SerializeField]
    private Assets.Scripts.Model.Player _enemyConfig;
    [SerializeField]
    private int _roundsCount = 3;
    bool _initializedIsFinished = false;
    private void Awake()
    {
        GameDataManager.Instance.Init(new(GameState.Menu, 0, _roundsCount, _playerConfig, _enemyConfig));
    }
    private void Update()
    {
        if (!_initializedIsFinished)
        {
            try
            {
                GameManager.Instance.Init();
                _initializedIsFinished = true;
                Destroy(gameObject);
            }
            catch (Exception ex)
            {
                Debug.LogError($"{nameof(GameManager)} initialization failed with {ex.GetType()}:{Environment.NewLine}{ex}");
            }
        }
    }
}
