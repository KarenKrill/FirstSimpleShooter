using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
public class GameManager : MonoBehaviour
{
    public event Action<GameState> GameStateChanged;
    private GameState _state;
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
    public void SetState(GameState state) => State = state;
    public static GameManager Instance;
    public void StartNewGame()
    {
        State = GameState.PlayerTurn;
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
    void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        State = GameState.Menu;
    }
    // Update is called once per frame
    void Update()
    {

    }
}
