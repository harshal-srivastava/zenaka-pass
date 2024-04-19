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

    [SerializeField]
    private GameObject quitGamePopUp;

    private void Awake()
    {
        GameScoreManager.PlayerScoreUpdatedCB += UpdatePlayerScoreUI;
        GameManager.PlayerTurnCompletedEventCB += UpdatePlayerTurnUI;
        GameManager.InitializeGameCB += DisplayDefaultScore;
        GameManager.GameProgressLoadedCB += DisplayLoadedProgressData;
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

    public void GamePlayScreenQuitGameButtonPressed()
    {
        quitGamePopUp.SetActive(true);
    }

    public void GamePlayQuitScreenCancelButtonPressed()
    {
        quitGamePopUp.SetActive(false);
    }


}
