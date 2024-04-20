using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for calcualting and storing the user progress
/// </summary>
public class GameScoreManager : MonoBehaviour
{
    //setting class as single to as to provide easy access to score variables
    private static GameScoreManager instance;

    public static GameScoreManager Instance
    {
        get
        {
            return instance;
        }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
        AttachGameEventListeners();
    }

    //declaring all score related variables as property with private setters
    //this will help in preventing other classes from changing these values by mistake
    public int PlayerScore { get; private set; }
    public int PlayerScoreCombo { get; private set; }
    public int PlayerNumberOfTurns { get; private set; }
    public int PlayerNumberOfMatches { get; private set; }

    //constant variable for score awarded after each match, in future if this needs to be changed
    //only need to change it in here
    public const int PERMATCHSCOREAWARDED = 10;

    /// <summary>
    /// delegate for calling player score updating event
    /// </summary>
    public delegate void PlayerScoreUpdatedEvent();
    public static PlayerScoreUpdatedEvent PlayerScoreUpdatedCB;


    /// <summary>
    /// Callback function to update player score if cards are matched
    /// Score will be calculated based on combo
    /// combo value will come from GameManager
    /// </summary>
    /// <param name="combo"></param>
    private void UpdatePlayerScore(int combo)
    {
        PlayerScoreCombo = combo;
        PlayerScore += (PERMATCHSCOREAWARDED * PlayerScoreCombo);
        PlayerNumberOfMatches += 1;
        PlayerScoreUpdatedCB?.Invoke();
    }

    /// <summary>
    /// Callback function to restore the combo value to 1 in case the combo breaks
    /// </summary>
    /// <param name="combo"></param>
    private void HandleCardMisMatch(int combo)
    {
        PlayerScoreCombo = combo;
        PlayerScoreUpdatedCB?.Invoke();
    }

    /// <summary>
    /// Callback function to increase the player number of turns
    /// either cards match or not, this variable will be increased
    /// </summary>
    private void PlayerTurnCompleted()
    {
        PlayerNumberOfTurns += 1;
    }

    /// <summary>
    /// Function to set scores to default value
    /// </summary>
    private void SetScoresToDefault()
    {
        PlayerScore = 0;
        PlayerScoreCombo = 0;
        PlayerNumberOfMatches = 0;
        PlayerNumberOfTurns = 0;
    }

    /// <summary>
    /// Function to set the score values based on the progress data loaded
    /// </summary>
    /// <param name="score"></param>
    /// <param name="combo"></param>
    /// <param name="matches"></param>
    /// <param name="turns"></param>
    void SetProgressLoadedScores(int score, int combo, int matches, int turns)
    {
        PlayerScore = score;
        PlayerScoreCombo = combo;
        PlayerNumberOfMatches = matches;
        PlayerNumberOfTurns = turns;
    }

    /// <summary>
    /// Function to attach game event listeners
    /// Helps in decoupling the referencing to multiple classes
    /// </summary>
    private void AttachGameEventListeners()
    {
        GameManager.CardMatchCB += UpdatePlayerScore;
        GameManager.PlayerTurnCompletedEventCB += PlayerTurnCompleted;
        GameManager.InitializeGameCB += SetScoresToDefault;
        GameManager.GameProgressLoadedCB += SetProgressLoadedScores;
        GameManager.CardMisMatchCB += HandleCardMisMatch;
    }

    /// <summary>
    /// Function to detach listeners to respective class game events
    /// This is done as a safe keeping in future if a scene reload is required
    /// Static events couple with delegates don't work so well on scene reloads
    /// So detach them if object is destroyed and it will be attached again when instance of class is created
    /// </summary>
    private void DetacheGameEventListeners()
    {
        GameManager.CardMatchCB -= UpdatePlayerScore;
        GameManager.PlayerTurnCompletedEventCB -= PlayerTurnCompleted;
        GameManager.InitializeGameCB -= SetScoresToDefault;
        GameManager.GameProgressLoadedCB -= SetProgressLoadedScores;
        GameManager.CardMisMatchCB -= HandleCardMisMatch;
    }


    private void OnDestroy()
    {
        DetacheGameEventListeners();
    }
}
