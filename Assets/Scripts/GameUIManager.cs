using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class responsible for overall UI/UX flow of the game
/// </summary>
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

    [SerializeField]
    private GameObject gameDataLoadingFailedPopup;

    private void Awake()
    {
        AttachGameEventListeners();
    }

    /// <summary>
    /// Function to exit play mode in unity editor
    /// And close the application if not in unity editor
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    /// <summary>
    /// Function call if user presses play new game in home screen menu
    /// </summary>
    public void HomeScreenPlayButtonPressed()
    {
        mainGameHomeScren.SetActive(false);
        //added condition to cover case if user plays a new game from load data fail UI popup
        if (gameDataLoadingFailedPopup.activeSelf)
        {
            gameDataLoadingFailedPopup.SetActive(false);
        }
        gridScreen.SetActive(true);
    }

    /// <summary>
    /// Callback function to update the game UI when the game starts
    /// </summary>
    private void StartGame()
    {
        gridScreen.SetActive(false);
        gamePlayScreen.SetActive(true);
    }

    /// <summary>
    /// Callback function to show game won screen if player wins
    /// </summary>
    private void ShowGameWonScreen()
    {
        gamePlayScreen.SetActive(false);
        gameWonScreen.SetActive(true);
    }

    /// <summary>
    /// Function called when user presses replay game button in the game over screen
    /// </summary>
    public void GameWonScreenRetryButtonPressed()
    {
        gameWonScreen.SetActive(false);
        gridScreen.SetActive(true);
    }

    /// <summary>
    /// Function called when user presses the load game button on home screen menu
    /// </summary>
    public void LoadGameButtonPressed()
    {
        mainGameHomeScren.SetActive(false);
        gamePlayScreen.SetActive(true);
    }

    /// <summary>
    /// Callback function to show the load game failed state of the game
    /// </summary>
    private void GameDataLoadingFailed()
    {
        gamePlayScreen.SetActive(false);
        mainGameHomeScren.SetActive(true);
        gameDataLoadingFailedPopup.SetActive(true);
    }

    /// <summary>
    /// Function called when user presses close button in the load failed UI popup
    /// </summary>
    public void GameLoadFailedPopUpCloseButtonPressed()
    {
        gameDataLoadingFailedPopup.SetActive(false);
    }

    /// <summary>
    /// Function to attach game event listeners
    /// Helps in decoupling the referencing to multiple classes
    /// </summary>
    private void AttachGameEventListeners()
    {
        GridSelectionScereen.gameStartCB += StartGame;
        GameManager.GameWonCB += ShowGameWonScreen;
        GameSaveLoadManager.GameSaveDataLoadFailedCB += GameDataLoadingFailed;
    }

    /// <summary>
    /// Function to detach listeners to respective class game events
    /// This is done as a safe keeping in future if a scene reload is required
    /// Static events couple with delegates don't work so well on scene reloads
    /// So detach them if object is destroyed and it will be attached again when instance of class is created
    /// </summary>
    private void DetachGameEventListeners()
    {
        GridSelectionScereen.gameStartCB -= StartGame;
        GameManager.GameWonCB -= ShowGameWonScreen;
        GameSaveLoadManager.GameSaveDataLoadFailedCB -= GameDataLoadingFailed;
    }

    private void OnDestroy()
    {
        DetachGameEventListeners();
    }
}
