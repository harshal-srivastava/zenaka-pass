using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private Dictionary<int, Sprite> gameCardStoredDictionary;
    private List<GameCard> availableCardsShownToPlayer;

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

    public static int minGridX = 2;
    public static int minGridY = 2;

    public delegate void GameWonEvent();
    public static GameWonEvent GameWonCB;

    public delegate void CardMatchEvent(int combo);
    public static CardMatchEvent CardMatchCB;

    public delegate void CardMisMatchEvent(int combo);
    public static CardMisMatchEvent CardMisMatchCB;

    public delegate void PlayerTurnCompletedEvent();
    public static PlayerTurnCompletedEvent PlayerTurnCompletedEventCB;

    public delegate void InitializeGameEvent();
    public static InitializeGameEvent InitializeGameCB;

    public delegate void GameProgressLoadedEvent(int score, int combo, int matches, int turns);
    public static GameProgressLoadedEvent GameProgressLoadedCB;

    private int combo = 0;

    [SerializeField]
    private GameSaveLoadManager gameSaveManager;

    // Start is called before the first frame update
    void Awake()
    {
        SetCardPanelSizeVariables();
        LoadAllCardInformationData();
        hasGameStarted = false;
        GameCard.cardClickedCB += GameCardClicked;
        GridSelectionScereen.updateGameGridCB += UpdateGameGrid;
        GridSelectionScereen.gameStartCB += StartGame;
        cardClickCount = 0;
    }

    void StartGame()
    {
        combo = 0;
        InitializeGameCB?.Invoke();
        SetCards();
    }

    void UpdateGameGrid(int x, int y)
    {
        gridX = x;
        gridY = y;
    }

    void SetCardPanelSizeVariables()
    {
        cardHolderPanelSizeX = cardHolderPanel.sizeDelta.x;
        cardHolderPanelSizeY = cardHolderPanel.sizeDelta.y;
        
    }

    private void Update()
    {
        //only for quick testing on grid creation and value assignment on runtime
        //if (Input.GetKeyDown(KeyCode.Space))
       // {
        //    SetCards();
       // }
    }

    void SetCards()
    {
        SetCardGrid();
        SetCardDetails();
        Invoke("HideAllCards", gameStartCardShowingTime);
    }

    void HideAllCards()
    {
        for (int i=0;i<availableCardsShownToPlayer.Count;i++)
        {
            availableCardsShownToPlayer[i].HideCard();
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

        availableCardsShownToPlayer = new List<GameCard>();

        for (int i=0;i<gridY;i++)
        {
            currX = initialX;
            for (int j=0;j<gridX;j++)
            {
                GameCard card = GameObject.Instantiate(cardPrefab, cardHolderPanel).GetComponent<GameCard>();
                card.GetComponent<RectTransform>().sizeDelta = new Vector2(cardWidth, cardHeight);
                card.GetComponent<RectTransform>().anchoredPosition = new Vector2(currX, currY);
                availableCardsShownToPlayer.Add(card);
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
        int[] selectedCardsID = GetRandomGameCardsID();

        //using the above gotten cards, fill the availableCards list objects with ID and sprites
        SetGameCardValuesFromSelectedCards(selectedCardsID);
    }


    int[] GetRandomGameCardsID()
    {
        //we only need half the cards as other half will have same value to form pairs
        int[] selectedCardsID = new int[availableCardsShownToPlayer.Count / 2];
        List<int> gameCardStordDicKeys = new List<int>(gameCardStoredDictionary.Keys);
        for (int i = 0; i < availableCardsShownToPlayer.Count / 2; i++)
        {
            //logic used
            // 1. check if current card we selected is not selected just before
            // 2. if so, increase the value index (the random number generated)
            // 3. use modulus operator to make sure we don't get error if value becomes larger than information array length
            int value = Random.Range(0, gameCardStoredDictionary.Count - 1);
           
            for (int j = i; j > 0; j--)
            {
                if (selectedCardsID[j - 1] == gameCardStordDicKeys[value]) ;
                {
                    value = (value + 1) % gameCardStoredDictionary.Count;
                }
            }
            selectedCardsID[i] = gameCardStordDicKeys[value];
        }
        return selectedCardsID;
    }

    void SetGameCardValuesFromSelectedCards(int[] selectedGameCardsID)
    {

        //logic
        //1. Iterate through half of the availableCards array
        //2. For each index, run an inner loop of 2
        //3. Once we get one value, then randomly search for a card whose value has not yet been assigned
        //4. If the availableCard element is already populated, just increase the value and use modulus operator
        //5. This ensures we won't get any error
        //6. then assign the card for both of those positions
        for (int i = 0; i < availableCardsShownToPlayer.Count / 2; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int value = Random.Range(0, availableCardsShownToPlayer.Count - 1);
                while (availableCardsShownToPlayer[value].cardID != -1)
                    value = (value + 1) % availableCardsShownToPlayer.Count;
                availableCardsShownToPlayer[value].InitializeCard(selectedGameCardsID[i],gameCardStoredDictionary.GetValueOrDefault(selectedGameCardsID[i]));
            }
        }
    }

    void ResetAllCards()
    {
        for (int i = 0; i < availableCardsShownToPlayer.Count; i++)
        {
            availableCardsShownToPlayer[i].ResetCard();
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
        PlayerTurnCompletedEventCB?.Invoke();
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
            combo += 1;
            CardMatchCB?.Invoke(combo);
            yield return new WaitForSeconds(0.5f);
            HandleCardsMatchSuccess(new GameCard[] {lastCard, secondLastCard }); 
        }
        //cards mismatch
        else
        {
            GameAudioManager.Instance.PlayCardMatchFailSound();
            ResetCardClickCount();
            combo = 0;
            CardMisMatchCB?.Invoke(combo);
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
            availableCardsShownToPlayer.Remove(cards[i]);
            cards[i].RemoveCard();
        }
        CheckForGameOver();
        
    }

    void CheckForGameOver()
    {
        if (availableCardsShownToPlayer.Count == 0)
        {
            //game win
            GameAudioManager.Instance.PlayGameWonSound();
            GameWonCB?.Invoke();
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
        GameCardSO[] gameCardScriptableObjectArray = Resources.LoadAll<GameCardSO>("GameCards");
        gameCardStoredDictionary = new Dictionary<int, Sprite>();
        for (int i=0;i<gameCardScriptableObjectArray.Length;i++)
        {
            gameCardStoredDictionary.Add(gameCardScriptableObjectArray[i].cardID, gameCardScriptableObjectArray[i].cardSprite);
        }
    }

    public void SaveUserProgressAndQuit()
    {
        SaveUserProgress();
        QuitApplication();
    }

    void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void SaveUserProgress()
    {
        GameData data = new GameData();
        data.score = GameScoreManager.Instance.PlayerScore;
        data.combo = GameScoreManager.Instance.PlayerScoreCombo;
        data.noOfMatches = GameScoreManager.Instance.PlayerNumberOfMatches;
        data.noOfTurns = GameScoreManager.Instance.PlayerNumberOfTurns;
        data.cardsSaved = new List<CardSaveData>();
        for (int i=0;i<availableCardsShownToPlayer.Count;i++)
        {
            CardSaveData card = new CardSaveData();
            card.cardId = availableCardsShownToPlayer[i].cardID;
            card.cardSize = availableCardsShownToPlayer[i].GetComponent<RectTransform>().sizeDelta;
            card.cardPosition = availableCardsShownToPlayer[i].GetComponent<RectTransform>().anchoredPosition;
            data.cardsSaved.Add(card);
        }

        gameSaveManager.ReadGameData(data);
    }

    public void LoadUserProgress()
    {
        GameData loadedData = gameSaveManager.LoadGameData();
        if (loadedData == null)
        {
            return;
        }
        availableCardsShownToPlayer = new List<GameCard>();
        combo = loadedData.combo;
        for (int i=0;i<loadedData.cardsSaved.Count;i++)
        {
            GameCard card = GameObject.Instantiate(cardPrefab, cardHolderPanel).GetComponent<GameCard>();
            card.InitializeCard(loadedData.cardsSaved[i].cardId, gameCardStoredDictionary.GetValueOrDefault(loadedData.cardsSaved[i].cardId));
            card.GetComponent<RectTransform>().sizeDelta = new Vector2(loadedData.cardsSaved[i].cardSize.x, loadedData.cardsSaved[i].cardSize.y);
            card.GetComponent<RectTransform>().anchoredPosition = new Vector2(loadedData.cardsSaved[i].cardPosition.x, loadedData.cardsSaved[i].cardPosition.y);
            availableCardsShownToPlayer.Add(card);
        }
        GameProgressLoadedCB?.Invoke(loadedData.score, loadedData.combo, loadedData.noOfMatches, loadedData.noOfTurns);
        Invoke("HideAllCards", gameStartCardShowingTime);
    }
}

[System.Serializable]
public class GameData
{
    public int score;
    public int combo;
    public int noOfMatches;
    public int noOfTurns;
    public List<CardSaveData> cardsSaved;
}

[System.Serializable]
public class CardSaveData
{
    public int cardId;
    public Vector2 cardSize;
    public Vector2 cardPosition;
}