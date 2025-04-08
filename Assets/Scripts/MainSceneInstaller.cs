using Assets.Scripts;
using Assets.Scripts.Model;
using Assets.Scripts.Model.InventoryItems;
using Assets.Scripts.Utils;
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
    private Player _enemyDefaultConfig;
    [SerializeField]
    private int _roundsCount = 3;
    bool _initializedIsFinished = false;
    private void Awake()
    {
#if UNITY_EDITOR
        Cursor.SetCursor(UnityEditor.PlayerSettings.defaultCursor, Vector2.zero, CursorMode.ForceSoftware);
#endif
        GameDataManager.Instance.Init(new(GameState.Menu, 0, _roundsCount, _playerDefaultConfig, _enemyDefaultConfig));
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
