using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    private void Awake()
    {
        GameScoreManager.PlayerScoreUpdatedCB += UpdatePlayerScoreUI;
        GameManager.PlayerTurnCompletedEventCB += UpdatePlayerTurnUI;
        GameManager.InitializeGameCB += DisplayDefaultScore;
    }

    void UpdatePlayerScoreUI()
    {
        scoreText.text = GameScoreManager.Instance.PlayerScore.ToString();
        comboText.text = GameScoreManager.Instance.PlayerScoreCombo.ToString();
        numberOfMatchesText.text = GameScoreManager.Instance.PlayerNumberOfMatches.ToString();
    }

    void UpdatePlayerTurnUI()
    {
        numberOfTurnsText.text = GameScoreManager.Instance.PlayerNumberOfTurns.ToString();
    }

    void DisplayDefaultScore()
    {
        scoreText.text = "0";
        comboText.text = "1";
        numberOfMatchesText.text = "0";
        numberOfTurnsText.text = "0";
    }


}
