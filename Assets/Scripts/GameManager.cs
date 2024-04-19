using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameCardSO[] gameCardScriptableObjectArray;
    private List<GameCard> availableCards;

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

    [SerializeField]
    private float gameStartCardShowingTime;

    public static bool hasGameStarted { get; private set; }

    public List<GameCard> clickedCards;

    private int cardClickCount;

    // Start is called before the first frame update
    void Awake()
    {
        SetCardPanelSizeVariables();
        LoadAllCardInformationData();
        hasGameStarted = false;
        GameCard.cardClickedCB += GameCardClicked;
        cardClickCount = 0;
    }

    private void Start()
    {
        SetCards();
    }

    void SetCardPanelSizeVariables()
    {
        cardHolderPanelSizeX = cardHolderPanel.sizeDelta.x;
        cardHolderPanelSizeY = cardHolderPanel.sizeDelta.y;
        
    }

    private void Update()
    {
        //only for quick testing on grid creation and value assignment on runtime
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetCards();
        }
    }

    void SetCards()
    {
        SetCardGrid();
        SetCardDetails();
        Invoke("HideAllCards", gameStartCardShowingTime);
    }

    void HideAllCards()
    {
        for (int i=0;i<availableCards.Count;i++)
        {
            availableCards[i].HideCard();
        }
        hasGameStarted = true;
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

        availableCards = new List<GameCard>();

        for (int i=0;i<gridY;i++)
        {
            currX = initialX;
            for (int j=0;j<gridX;j++)
            {
                GameCard card = GameObject.Instantiate(cardPrefab, cardHolderPanel).GetComponent<GameCard>();
                card.GetComponent<RectTransform>().sizeDelta = new Vector2(cardWidth, cardHeight);
                card.GetComponent<RectTransform>().anchoredPosition = new Vector2(currX, currY);
                availableCards.Add(card);
                currX += incrementX;
            }
            currY -= incrementY;
        }
    }

    void SetCardDetails()
    {
        ResetAllCards();

        //get randomly selected cards to play from gameCardScriptableObjectArray
        //this array is basically storehouse of all the available cards, which can be created via menu in unity
        GameCardSO[] selectedCards = GetRandomGameCards();

        //using the above gotten cards, fill the availableCards list objects with ID and sprites
        SetGameCardValuesFromSelectedCards(selectedCards);
    }


    GameCardSO[] GetRandomGameCards()
    {
        //we only need half the cards as other half will have same value to form pairs
        GameCardSO[] selectedCards = new GameCardSO[availableCards.Count / 2];
        for (int i = 0; i < availableCards.Count / 2; i++)
        {
            //logic used
            // 1. check if current card we selected is not selected just before
            // 2. if so, increase the value index (the random number generated)
            // 3. use modulus operator to make sure we don't get error if value becomes larger than information array length
            int value = Random.Range(0, gameCardScriptableObjectArray.Length - 1);
            for (int j = i; j > 0; j--)
            {
                if (selectedCards[j - 1].cardID == gameCardScriptableObjectArray[value].cardID)
                {
                    value = (value + 1) % gameCardScriptableObjectArray.Length;
                }
            }
            selectedCards[i] = gameCardScriptableObjectArray[value];
        }
        return selectedCards;
    }

    void SetGameCardValuesFromSelectedCards(GameCardSO[] selectedGameCards)
    {

        //logic
        //1. Iterate through half of the availableCards array
        //2. For each index, run an inner loop of 2
        //3. Once we get one value, then randomly search for a card whose value has not yet been assigned
        //4. If the availableCard element is already populated, just increase the value and use modulus operator
        //5. This ensures we won't get any error
        //6. then assign the card for both of those positions
        for (int i = 0; i < availableCards.Count / 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int value = Random.Range(0, availableCards.Count - 1);
                while (availableCards[value].cardID != -1)
                    value = (value + 1) % availableCards.Count;
                availableCards[value].InitializeCard(selectedGameCards[i].cardID, selectedGameCards[i].cardSprite);
            }
        }
    }

    void ResetAllCards()
    {
        for (int i = 0; i < availableCards.Count; i++)
        {
            availableCards[i].ResetCard();
        }
    }

    void ClearCurrentCardGrid()
    {
        foreach (Transform child in cardHolderPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    void GameCardClicked(GameCard card)
    {
        clickedCards.Add(card);
        StartCoroutine(ProcessClickedCardsListForMatch());
    }

    IEnumerator ProcessClickedCardsListForMatch()
    {
        cardClickCount += 1;
        if (cardClickCount != 2)
        {
            yield break;
        }
        if (clickedCards.Count < 2)
        {
            yield break;
        }
        GameCard lastCard = clickedCards[clickedCards.Count - 1];
        GameCard secondLastCard = clickedCards[clickedCards.Count - 2];
        if (lastCard == null || secondLastCard == null)
        {
            yield break;
        }
        //cards match
        if (lastCard.cardID == secondLastCard.cardID)
        {
            GameAudioManager.Instance.PlayCardMatchSuccessSound();
            ResetCardClickCount();
            clickedCards.RemoveRange(clickedCards.Count - 2, 2);
            yield return new WaitForSeconds(0.5f);
            HandleCardsMatchSuccess(new GameCard[] {lastCard, secondLastCard }); 
        }
        //cards mismatch
        else
        {
            GameAudioManager.Instance.PlayCardMatchFailSound();
            ResetCardClickCount();
            yield return new WaitForSeconds(0.5f);
            HandleCardMatchFail(new GameCard[] { lastCard, secondLastCard }); 
        }
    }

    void ResetCardClickCount()
    {
        cardClickCount = 0;
    }

    void HandleCardsMatchSuccess(GameCard[] cards)
    {

        for (int i=0;i<cards.Length;i++)
        {
            cards[i].RemoveCard();
        }
    }

    void HandleCardMatchFail(GameCard[] cards)
    {
        for (int i=0;i<cards.Length;i++)
        {
            cards[i].FlipCard();
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
