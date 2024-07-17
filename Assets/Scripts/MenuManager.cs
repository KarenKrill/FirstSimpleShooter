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
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameStateChanged -= OnGameStateChanged;
        }
    }
    private void OnGameStateChanged(GameState state)
    {
        static void SwitchMenu(List<Optional<GameObject>> turnOffMenus, GameObject turnOnMenu)
        {
            foreach (var menu in turnOffMenus)
            {
                if (menu.Enabled)
                {
                    menu.Value.SetActive(false);
                }
            }
            turnOnMenu.SetActive(true);
        }
        switch (state)
        {
            case GameState.Menu:
                if (MainMenu.Enabled && !MainMenu.Value.activeInHierarchy)
                {
                    SwitchMenu(new() { WinMenu, DefeatMenu, ConsumableMenu, InGameCanvas }, MainMenu.Value);
                }
                break;
            case GameState.Win:
                if (WinMenu.Enabled && !WinMenu.Value.activeInHierarchy)
                {
                    SwitchMenu(new() { DefeatMenu, ConsumableMenu, InGameCanvas, MainMenu }, WinMenu.Value);
                }
                break;
            case GameState.Defeat:
                if (DefeatMenu.Enabled && !DefeatMenu.Value.activeInHierarchy)
                {
                    SwitchMenu(new() { WinMenu, ConsumableMenu, InGameCanvas, MainMenu }, DefeatMenu.Value);
                }
                break;
            default:
                if (InGameCanvas.Enabled && !InGameCanvas.Value.activeInHierarchy)
                {
                    SwitchMenu(new() { WinMenu, DefeatMenu, ConsumableMenu, MainMenu }, InGameCanvas.Value);
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
