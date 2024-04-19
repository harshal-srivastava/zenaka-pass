using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameCardSO[] gameCardScriptableObjectArray;
    private GameCard[] availableCards;

    [SerializeField]
    private int gridX;

    [SerializeField]
    private int gridY;

    [SerializeField]
    private RectTransform cardHolderPanel;

    [SerializeField]
    private GameObject cardPrefab;

    private float cardHolderPanelSizeX;

    private float cardHolderPanelSizeY;

    [SerializeField]
    private float paddingX;

    [SerializeField]
    private float paddingY;

    // Start is called before the first frame update
    void Awake()
    {
        SetCardPanelSizeVariables();
        LoadAllCardInformationData();
    }

    private void Start()
    {
        SetCardGrid();
    }

    void SetCardPanelSizeVariables()
    {
        cardHolderPanelSizeX = cardHolderPanel.sizeDelta.x;
        cardHolderPanelSizeY = cardHolderPanel.sizeDelta.y;
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetCardGrid();
        }
    }

    void SetCardGrid()
    {
        ClearCurrentCardGrid();

        //get increment value, to add to x and y for grid
        float incrementX = cardHolderPanelSizeX / gridX;
        float incrementY = cardHolderPanelSizeY / gridY;

        //set card height and width
        float cardWidth = incrementX;
        float cardHeight = incrementY;

        //math used 
        // 1. Getholderpanel size, divide by 2
        // 2. This will give initial position
        // 3. add width/2 to x position
        // 4. subtract y/2 to y position
        float currX = -cardHolderPanelSizeX/2 +cardWidth/2;
        float currY = cardHolderPanelSizeY/2-cardHeight/2;

        //used for resetting X value after filling each row
        float initialX = currX;
        for (int i=0;i<gridY;i++)
        {
            currX = initialX;
            for (int j=0;j<gridX;j++)
            {
                GameCard card = GameObject.Instantiate(cardPrefab, cardHolderPanel).GetComponent<GameCard>();
                card.GetComponent<RectTransform>().sizeDelta = new Vector2(cardWidth, cardHeight);
                card.GetComponent<RectTransform>().anchoredPosition = new Vector2(currX, currY);
                //card.transform.localScale = new Vector3(scale, scale, 1);
                currX += incrementX;
            }
            currY -= incrementY;
        }
    }

    void ClearCurrentCardGrid()
    {
        foreach (Transform child in cardHolderPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Function to load all the available card information from the card scriptable objects
    /// </summary>
    void LoadAllCardInformationData()
    {
        gameCardScriptableObjectArray = Resources.LoadAll<GameCardSO>("GameCards");
    }
}
