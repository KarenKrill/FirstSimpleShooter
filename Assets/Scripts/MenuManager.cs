using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MenuManager : MonoBehaviour
{
    [SerializeField]
    public Optional<GameObject> MainMenu;
    public Optional<GameObject> WinMenu;
    public Optional<GameObject> DefeatMenu;
    public Optional<GameObject> InGameCanvas;
    public Optional<GameObject> ConsumableMenu;
    private void Awake()
    {
        GameManager.Instance.GameStateChanged += OnGameStateChanged;
    }
    private void OnDestroy()
    {
        GameManager.Instance.GameStateChanged -= OnGameStateChanged;
    }
    private void OnGameStateChanged(GameState state)
    {
        if (MainMenu.Enabled)
        {
            MainMenu.Value.SetActive(false);
        }
        if (WinMenu.Enabled)
        {
            WinMenu.Value.SetActive(false);
        }
        if(DefeatMenu.Enabled)
        {
            DefeatMenu.Value.SetActive(false);
        }
        if(ConsumableMenu.Enabled)
        {
            ConsumableMenu.Value.SetActive(false);
        }
        if(InGameCanvas.Enabled)
        {
            InGameCanvas.Value.SetActive(false);
        }
        switch (state)
        {
            case GameState.Menu:
                if (MainMenu.Enabled)
                {
                    MainMenu.Value.SetActive(true);
                }
                break;
            case GameState.PlayerTurn:
            case GameState.EnemyTurn:
                if (InGameCanvas.Enabled)
                {
                    InGameCanvas.Value.SetActive(true);
                }
                break;
            case GameState.Win:
                if (WinMenu.Enabled)
                {
                    WinMenu.Value.SetActive(true);
                }
                break;
            case GameState.Defeat:
                if (DefeatMenu.Enabled)
                {
                    DefeatMenu.Value.SetActive(true);
                }
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnExitButtonClick()
    {
    }
}
