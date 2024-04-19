using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mainGameHomeScren;

    [SerializeField]
    private GameObject gamePlayScreen;

    [SerializeField]
    private GameObject gameWonScreen;

    [SerializeField]
    private GameObject gridScreen;

    private void Awake()
    {
        GridSelectionScereen.gameStartCB += StartGame;
        GameManager.GameWonCB += ShowGameWonScreen;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void HomeScreenPlayButtonPressed()
    {
        mainGameHomeScren.SetActive(false);
        gridScreen.SetActive(true);
    }

    void StartGame()
    {
        gridScreen.SetActive(false);
        gamePlayScreen.SetActive(true);
    }

    void ShowGameWonScreen()
    {
        gamePlayScreen.SetActive(false);
        gameWonScreen.SetActive(true);
    }

    public void GameWonScreenRetryButtonPressed()
    {
        gameWonScreen.SetActive(false);
        gridScreen.SetActive(true);
    }

    public void LoadGameButtonPressed()
    {
        mainGameHomeScren.SetActive(false);
        gamePlayScreen.SetActive(true);
    }
}
