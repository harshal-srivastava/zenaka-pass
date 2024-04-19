using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public delegate void UpdateGameGrid(int x, int y);
    public static UpdateGameGrid updateGameGridCB;

    public delegate void GameStart();
    public static GameStart gameStartCB;

    float rows;
    float columns;

    private void Awake()
    {
        rowsSlider.onValueChanged.AddListener(UpdateNumberOfRows);
        columnSlider.onValueChanged.AddListener(UpdateNumberOfColumns);
        rowsSlider.value = GameManager.minGridX;
        columnSlider.value = GameManager.minGridY;
    }

    void UpdateNumberOfRows(float value)
    {
        rowsText.text = value.ToString();
        rows = value;
        updateGameGridCB?.Invoke((int)rows, (int)columns);
    }

    void UpdateNumberOfColumns(float value)
    {
        columnsText.text = value.ToString();
        columns = value;
        updateGameGridCB?.Invoke((int)rows, (int)columns);
    }

    public void PlayGamePressed()
    {
        if ((rows * columns)%2 != 0)
        {
            StartCoroutine(DisplayError("Please select an even row or column!"));
        }
        else
        {
            gameStartCB?.Invoke();
        }
    }

    IEnumerator DisplayError(string message)
    {
        errorText.text = message;
        yield return new WaitForSeconds(3);
        errorText.text = "";
    }

}
