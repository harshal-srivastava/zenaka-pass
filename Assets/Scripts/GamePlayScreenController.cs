using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Class to display the user score in the gameplay screen
/// Decoupling it from the GameUIManager class helps in scaling as we won't need to change anything in that class
/// </summary>
public class GamePlayScreenController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI scoreText;

    [SerializeField]
    private TextMeshProUGUI comboText;

    [SerializeField]
    private TextMeshProUGUI numberOfMatchesText;

    [SerializeField]
    private TextMeshProUGUI numberOfTurnsText;

    [SerializeField]
    private GameObject quitGamePopUp;

    private void Awake()
    {
        AttachGameEventListeners();
    }

    /// <summary>
    /// Function to attach listeners to respective class game events
    /// </summary>
    private void AttachGameEventListeners()
    {
        GameScoreManager.PlayerScoreUpdatedCB += UpdatePlayerScoreUI;
        GameManager.PlayerTurnCompletedEventCB += UpdatePlayerTurnUI;
        GameManager.InitializeGameCB += DisplayDefaultScore;
        GameManager.GameProgressLoadedCB += DisplayLoadedProgressData;
    }

    /// <summary>
    /// Function to detach listeners to respective class game events
    /// This is done as a safe keeping in future if a scene reload is required
    /// Static events couple with delegates don't work so well on scene reloads
    /// So detach them if object is destroyed and it will be attached again when instance of class is created
    /// </summary>
    private void DetachGameEventListeners()
    {
        GameScoreManager.PlayerScoreUpdatedCB -= UpdatePlayerScoreUI;
        GameManager.PlayerTurnCompletedEventCB -= UpdatePlayerTurnUI;
        GameManager.InitializeGameCB -= DisplayDefaultScore;
        GameManager.GameProgressLoadedCB -= DisplayLoadedProgressData;
    }

    /// <summary>
    /// Callback function for updating the player score visuals in gameplay screen
    /// </summary>
    private void UpdatePlayerScoreUI()
    {
        scoreText.text = GameScoreManager.Instance.PlayerScore.ToString();
        //extra condition added so as to display the combo as 1 if its value is 0
        comboText.text = (GameScoreManager.Instance.PlayerScoreCombo>0?GameScoreManager.Instance.PlayerScoreCombo : 1).ToString();
        numberOfMatchesText.text = GameScoreManager.Instance.PlayerNumberOfMatches.ToString();
    }

    /// <summary>
    /// Callback function to update the number of turns user took
    /// It is called as a separate function because turn will be updated irrespective of the scores
    /// </summary>
    private void UpdatePlayerTurnUI()
    {
        numberOfTurnsText.text = GameScoreManager.Instance.PlayerNumberOfTurns.ToString();
    }

    /// <summary>
    /// Function to display the default score on gameplay screen
    /// Used in case of game restart
    /// </summary>
    private void DisplayDefaultScore()
    {
        scoreText.text = "0";
        comboText.text = "1";
        numberOfMatchesText.text = "0";
        numberOfTurnsText.text = "0";
    }

    /// <summary>
    /// Function to display the user score based on the data received from loading the user progress
    /// </summary>
    /// <param name="score"></param>
    /// <param name="combo"></param>
    /// <param name="matches"></param>
    /// <param name="turns"></param>
    void DisplayLoadedProgressData(int score, int combo, int matches, int turns)
    {
        scoreText.text = score.ToString();
        //only added to visually show the combo as 1, 0 might be confusing to the player
        if (combo == 0)
        {
            combo++;
        }
        comboText.text = combo.ToString();
        numberOfMatchesText.text = matches.ToString();
        numberOfTurnsText.text = turns.ToString();
    }

    /// <summary>
    /// Function to enable the game quit popup if the user presses the Quit button from gameplay screen
    /// </summary>
    public void GamePlayScreenQuitGameButtonPressed()
    {
        quitGamePopUp.SetActive(true);
    }

    /// <summary>
    /// Function to disable the quit game popup if the user presses the cancel button
    /// </summary>
    public void GamePlayQuitScreenCancelButtonPressed()
    {
        quitGamePopUp.SetActive(false);
    }

    private void OnDestroy()
    {
        DetachGameEventListeners();
    }


}
