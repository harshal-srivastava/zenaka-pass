using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Class representing the grid selection screen for the game
/// Screen has two sliders, player can use them to customize the grid size
/// </summary>
public class GridSelectionScereen : MonoBehaviour
{
    [SerializeField]
    private Slider rowsSlider;

    [SerializeField]
    private Slider columnSlider;

    [SerializeField]
    private TextMeshProUGUI rowsText;

    [SerializeField]
    private TextMeshProUGUI columnsText;

    [SerializeField]
    private TextMeshProUGUI errorText;

    /// <summary>
    /// delegate to upadte the game grid parameters in the GameManager class
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public delegate void UpdateGameGrid(int x, int y);
    public static UpdateGameGrid updateGameGridCB;

    /// <summary>
    /// delegate to mark the start of the game
    /// </summary>
    public delegate void GameStart();
    public static GameStart gameStartCB;

    float rows;
    float columns;

    private void Awake()
    {
        rowsSlider.onValueChanged.AddListener(UpdateNumberOfRows);
        columnSlider.onValueChanged.AddListener(UpdateNumberOfColumns);
        //added for further customization codewise
        //if in future, dev wants to change the min grid size, they just need to change that variable in GameManager
        rowsSlider.value = GameManager.minGridX;
        columnSlider.value = GameManager.minGridY;
    }

    /// <summary>
    /// Function called when player changes the value of rowSlider
    /// </summary>
    /// <param name="value"></param>
    private void UpdateNumberOfRows(float value)
    {
        rowsText.text = value.ToString();
        rows = value;
        //make the delegate event call so that these values get updated for the game grid creation
        updateGameGridCB?.Invoke((int)rows, (int)columns);
    }

    /// <summary>
    /// Function called when player changes the value of columnSlider
    /// </summary>
    /// <param name="value"></param>
    void UpdateNumberOfColumns(float value)
    {
        columnsText.text = value.ToString();
        columns = value;
        //make the delegate event call so that these values get updated for the game grid creation
        updateGameGridCB?.Invoke((int)rows, (int)columns);
    }

    /// <summary>
    /// Function called when user presses the play game after setting the rows and columns
    /// </summary>
    public void PlayGamePressed()
    {
        //Extra condition added for following reason
        //If the resulting number of elements is odd, we cannot make pairs
        //There will be one card left which cannot be matched
        //It will be an invalid game state, hence this check
        if ((rows * columns)%2 != 0)
        {
            StartCoroutine(DisplayError("Please select an even row or column!"));
        }
        else
        {
            gameStartCB?.Invoke();
        }
    }

    /// <summary>
    /// Coroutine to disable the error text after some time
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    private IEnumerator DisplayError(string message)
    {
        errorText.text = message;
        yield return new WaitForSeconds(1.5f);
        errorText.text = "";
    }
}
