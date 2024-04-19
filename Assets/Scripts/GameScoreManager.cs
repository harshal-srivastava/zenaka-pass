using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScoreManager : MonoBehaviour
{
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
        GameManager.CardMatchCB += UpdatePlayerScore;
        GameManager.PlayerTurnCompletedEventCB += PlayerTurnCompleted;
    }

    public int PlayerScore { get; private set; }

    public int PlayerScoreCombo { get; private set; }
    public int PlayerNumberOfTurns { get; private set; }
    public int PlayerNumberOfMatches { get; private set; }

    public const int PERMATCHSCOREAWARDED = 10;

    public delegate void PlayerScoreUpdatedEvent();
    public static PlayerScoreUpdatedEvent PlayerScoreUpdatedCB;

    private void UpdatePlayerScore(int combo)
    {
        PlayerScoreCombo = combo;
        PlayerScore += (PERMATCHSCOREAWARDED * PlayerScoreCombo);
        PlayerNumberOfMatches += 1;
        PlayerScoreUpdatedCB?.Invoke();
    }

    private void PlayerTurnCompleted()
    {
        PlayerNumberOfTurns += 1;
    }
}
