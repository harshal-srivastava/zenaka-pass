using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Main game class, responsible for getting the card data from the scriptable objects
/// Creating the grid, and populating with randomly selected card pairs from the scriptable objects data
/// </summary>
public class GameManager : MonoBehaviour
{
    //dictionary to store the card index and sprite from the scriptable object card data present
    private Dictionary<int, Sprite> gameCardStoredDictionary;

    //represents the cards chosen for the player to play with
    private List<GameCard> availableCardsShownToPlayer;

    private int gridX;
    private int gridY;

    [SerializeField]
    private RectTransform cardHolderPanel;

    [SerializeField]
    private GameObject cardPrefab;

    private float cardHolderPanelSizeX;
    private float cardHolderPanelSizeY;

    [SerializeField]
    private float gameStartCardShowingTime;

    public static bool hasGameStarted { get; private set; }

    //List to store the cards that user has clicked
    public List<GameCard> clickedCards;

    //used to prevent user invalid clicks
    private int cardClickCount;

    public static int minGridX = 2;
    public static int minGridY = 2;

    /// <summary>
    /// delegate representing the game won state
    /// </summary>
    public delegate void GameWonEvent();
    public static GameWonEvent GameWonCB;

    /// <summary>
    /// delegate representing the match of two cards
    /// </summary>
    /// <param name="combo"></param>
    public delegate void CardMatchEvent(int combo);
    public static CardMatchEvent CardMatchCB;

    /// <summary>
    /// delegate representing the mismatch of two cards
    /// </summary>
    public delegate void CardMisMatchEvent(int combo);
    public static CardMisMatchEvent CardMisMatchCB;

    /// <summary>
    /// delegate representing user turn completed
    /// </summary>
    public delegate void PlayerTurnCompletedEvent();
    public static PlayerTurnCompletedEvent PlayerTurnCompletedEventCB;

    /// <summary>
    /// delegate for game intialization
    /// </summary>
    public delegate void InitializeGameEvent();
    public static InitializeGameEvent InitializeGameCB;

    /// <summary>
    /// delegate representing the gamedata loading complete
    /// </summary>
    /// <param name="score"></param>
    /// <param name="combo"></param>
    /// <param name="matches"></param>
    /// <param name="turns"></param>
    public delegate void GameProgressLoadedEvent(int score, int combo, int matches, int turns);
    public static GameProgressLoadedEvent GameProgressLoadedCB;

    private int combo = 0;

    [SerializeField]
    private GameSaveLoadManager gameSaveManager;

    
    void Awake()
    {
        SetGameInitVariables();
    }

    /// <summary>
    /// Function to set the panel size
    /// Load the card information from the scriptable objects
    /// Attach game event listeners
    /// </summary>
    private void SetGameInitVariables()
    {
        SetCardPanelSizeVariables();
        LoadAllCardInformationData();
        AttachGameEventListeners();
        hasGameStarted = false;
        cardClickCount = 0;
    }

    /// <summary>
    /// Callback function to start the game
    /// </summary>
    private void StartGame()
    {
        combo = 0;
        InitializeGameCB?.Invoke();
        SetCards();
    }

    /// <summary>
    /// Calllback function to set the grid size variables
    /// Will be used to create the grid
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void UpdateGameGrid(int x, int y)
    {
        gridX = x;
        gridY = y;
    }

    /// <summary>
    /// Function to get the card holder UI panel size
    /// </summary>
    private void SetCardPanelSizeVariables()
    {
        cardHolderPanelSizeX = cardHolderPanel.sizeDelta.x;
        cardHolderPanelSizeY = cardHolderPanel.sizeDelta.y;   
    }

    /// <summary>
    /// Function to start the game
    /// create the grid
    /// populate the grid with game cards
    /// </summary>
    private void SetCards()
    {
        SetCardGrid();
        SetCardDetails();
        Invoke("HideAllCards", gameStartCardShowingTime);
    }

    /// <summary>
    /// Function to hide all cards
    /// This is part of game mechanism, at start, it will show all cards to user for 2 seconds
    /// then hide them
    /// </summary>
    private void HideAllCards()
    {
        for (int i=0;i<availableCardsShownToPlayer.Count;i++)
        {
            availableCardsShownToPlayer[i].HideCard();
        }
        hasGameStarted = true;
    }

    #region GRID CREATION MECHANISM
    /// <summary>
    /// Function to create the card grid
    /// </summary>
    private void SetCardGrid()
    {
        //clear the current grid, for game restart purposes
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
    void ClearCurrentCardGrid()
    {
        foreach (Transform child in cardHolderPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
    #endregion

    #region CARD VALUES SETTING MECHANISM

    /// <summary>
    /// Function to populate the cards received with random pairs of IDs and images obtained 
    /// in the gameCardStoredDictionary dictionary
    /// </summary>
    private void SetCardDetails()
    {
        //reset all cards first, for restart purposes
        ResetAllCards();

        //get randomly selected cards ids to play from gameCardStoredDictionary
        //this dictionary is basically storehouse of all the available cards, which can be created via menu in unity
        int[] selectedCardsID = GetRandomGameCardsID();

        //using the above gotten cards, fill the availableCards list objects with ID and sprites
        SetGameCardValuesFromSelectedCards(selectedCardsID);
    }

    /// <summary>
    /// Function to return the array of IDs from the gameCardStoredDictionary
    /// Chosen on a random basis
    /// </summary>
    /// <returns></returns>
    private int[] GetRandomGameCardsID()
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

    /// <summary>
    /// Function to set the pairs of sprites based on the ID array received
    /// </summary>
    /// <param name="selectedGameCardsID"></param>
    private void SetGameCardValuesFromSelectedCards(int[] selectedGameCardsID)
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

    /// <summary>
    /// Function to set the cards to their original values
    /// </summary>
    private void ResetAllCards()
    {
        for (int i = 0; i < availableCardsShownToPlayer.Count; i++)
        {
            availableCardsShownToPlayer[i].ResetCard();
        }
    }
    #endregion

    #region GAMEPLAY AND CARD MATCHING MECHANISM

    /// <summary>
    /// Callback function for registering the user click
    /// </summary>
    /// <param name="card"></param>
    private void GameCardClicked(GameCard card)
    {
        clickedCards.Add(card);
        StartCoroutine(ProcessClickedCardsListForMatch());
    }

    /// <summary>
    /// Function to process the cards that the user has clicked
    /// Update number of turns if user has clicked 2 cards
    /// check two cards to see if they are a match or not
    /// </summary>
    /// <returns></returns>
    private IEnumerator ProcessClickedCardsListForMatch()
    {
        cardClickCount += 1;
        //add to prevent invalid user clicks
        if (cardClickCount != 2)
        {
            yield break;
        }
        //no need for any processing if user has clicked just one card
        if (clickedCards.Count < 2)
        {
            yield break;
        }
        //if code reaches here, then it means it is a legitimate turn
        //hence make the event call to update the number of turns
        PlayerTurnCompletedEventCB?.Invoke();

        //take last two cards in the clickedCard list
        //these two will be used for comparing
        GameCard lastCard = clickedCards[clickedCards.Count - 1];
        GameCard secondLastCard = clickedCards[clickedCards.Count - 2];

        //added extra condition to avoid null reference errors
        if (lastCard == null || secondLastCard == null)
        {
            yield break;
        }
        //cards match
        //if cards match, play the match successfull sound
        //remove the two cards from the list
        if (lastCard.cardID == secondLastCard.cardID)
        {
            GameAudioManager.Instance.PlayCardMatchSuccessSound();
            ResetCardClickCount();
            clickedCards.RemoveRange(clickedCards.Count - 2, 2);
            combo += 1;
            CardMatchCB?.Invoke(combo);
            //wait for 0.5 seconds so that player can see which cards matched
            //then start removing them
            yield return new WaitForSeconds(0.5f);
            HandleCardsMatchSuccess(new GameCard[] {lastCard, secondLastCard }); 
        }
        //cards mismatch
        else
        {
            GameAudioManager.Instance.PlayCardMatchFailSound();
            ResetCardClickCount();
            combo = 0;
            //calling this event to reset the user combo
            CardMisMatchCB?.Invoke(combo);
            //wait for 0.5 seconds so that player can see what card they clicked
            //then start flipping it back
            yield return new WaitForSeconds(0.5f);
            HandleCardMatchFail(new GameCard[] { lastCard, secondLastCard }); 
        }
    }

    /// <summary>
    /// Function to restart click count
    /// This needs to be reset everytime its value becomes 2
    /// </summary>
    private void ResetCardClickCount()
    {
        cardClickCount = 0;
    }

    /// <summary>
    /// Remove the cards matched from the availableCardsShownToPlayer list
    /// Then check for game over
    /// </summary>
    /// <param name="cards"></param>
    private void HandleCardsMatchSuccess(GameCard[] cards)
    {
        for (int i=0;i<cards.Length;i++)
        {
            availableCardsShownToPlayer.Remove(cards[i]);
            cards[i].RemoveCard();
        }
        CheckForGameOver();  
    }

    /// <summary>
    /// Function to check if player won the game
    /// </summary>
    private void CheckForGameOver()
    {
        //if there are no cards left in availableCardsShownToPlayer list
        //it means all cards have been matched and game won
        if (availableCardsShownToPlayer.Count == 0)
        {
            //game win
            GameAudioManager.Instance.PlayGameWonSound();
            GameWonCB?.Invoke();
        }
    }

    /// <summary>
    /// Function to reset the card or flip them back if they don't match
    /// </summary>
    /// <param name="cards"></param>
    private void HandleCardMatchFail(GameCard[] cards)
    {
        for (int i=0;i<cards.Length;i++)
        {
            cards[i].FlipCard();
        }
    }
    #endregion

    #region USER PROGRESS SAVING AND LOADING MECHANISM

    /// <summary>
    /// Function called when user presses Save and Quit button
    /// </summary>
    public void SaveUserProgressAndQuit()
    {
        SaveUserProgress();
        QuitApplication();
    }

    /// <summary>
    /// Function to quit the application or exit from playmode if in unity
    /// </summary>
    private void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }


    /// <summary>
    /// Function to convert the user progress into an object of GameData class
    /// And save that as json string file
    /// </summary>
    private void SaveUserProgress()
    {
        //take the game scores, cards and their current state
        //store them in object of GameData class
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

        //make the function call to convert it into json and write to a text file
        gameSaveManager.SaveGameData(data);
    }

    /// <summary>
    /// Function to load the user progress
    /// </summary>
    public void LoadUserProgress()
    {
        GameData loadedData = gameSaveManager.LoadGameData();
        //added to prevent null reference errors
        if (loadedData == null)
        {
            return;
        }
        //get the deserialized GameData object from the json file and populate the cards with that data
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
    #endregion

    /// <summary>
    /// Function to load all the available card information from the card scriptable objects
    /// Load all cards scriptable objects from Resources folder
    /// Then populate the dictionary with ID as key and sprite as value
    /// </summary>
    private void LoadAllCardInformationData()
    {
        GameCardSO[] gameCardScriptableObjectArray = Resources.LoadAll<GameCardSO>("GameCards");
        gameCardStoredDictionary = new Dictionary<int, Sprite>();
        for (int i = 0; i < gameCardScriptableObjectArray.Length; i++)
        {
            gameCardStoredDictionary.Add(gameCardScriptableObjectArray[i].cardID, gameCardScriptableObjectArray[i].cardSprite);
        }
    }

    /// <summary>
    /// Function to attach game event listeners
    /// Helps in decoupling the referencing to multiple classes
    /// </summary>
    private void AttachGameEventListeners()
    {
        GameCard.CardClickedCB += GameCardClicked;
        GridSelectionScereen.updateGameGridCB += UpdateGameGrid;
        GridSelectionScereen.gameStartCB += StartGame;
    }

    /// <summary>
    /// Function to detach listeners to respective class game events
    /// This is done as a safe keeping in future if a scene reload is required
    /// Static events couple with delegates don't work so well on scene reloads
    /// So detach them if object is destroyed and it will be attached again when instance of class is created
    /// </summary>
    private void DetachGameEventListeners()
    {
        GameCard.CardClickedCB -= GameCardClicked;
        GridSelectionScereen.updateGameGridCB -= UpdateGameGrid;
        GridSelectionScereen.gameStartCB -= StartGame;
    }

    private void OnDestroy()
    {
        DetachGameEventListeners();
    }
}

/// <summary>
/// Serializable class to represent user progress data
/// data will contain user score, combo, turns and matches
/// will contain array of the cards remaining when the game was saved
/// </summary>
[System.Serializable]
public class GameData
{
    public int score;
    public int combo;
    public int noOfMatches;
    public int noOfTurns;
    public List<CardSaveData> cardsSaved;
}

/// <summary>
/// Serializable class representing information about the card
/// Here we only need ID because we can fetch the sprite from the gameCardStoredDictionary
/// Need to store the card size and location so as to simulate the exact state the user saved their progress
/// </summary>
[System.Serializable]
public class CardSaveData
{
    public int cardId;
    public Vector2 cardSize;
    public Vector2 cardPosition;
}