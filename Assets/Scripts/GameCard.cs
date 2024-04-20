using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class representing a playable card inside the game
/// All the cards that are visible in the gameplay are instances of this class
/// </summary>
[RequireComponent(typeof(Button))]
public class GameCard : MonoBehaviour
{
    private Button cardButton;
    private Transform cardTransform;
    public int cardID { get; private set; }
    private Sprite cardSprite;
    public Sprite CardSprite
    {
        get
        {
            return cardSprite;
        }
        set
        {
            cardSprite = value;
            UpdateCardImage(value);
        }
    }
    private Image cardImageComponent;

    [SerializeField]
    private Sprite defaultSprite;

    private bool isTurning = false;
    private bool isCardHidden = true;

    /// <summary>
    /// delegate event for when a card is clicked
    /// </summary>
    /// <param name="card"></param>
    public delegate void GameCardClicked(GameCard card);
    public static GameCardClicked CardClickedCB;

    private void Awake()
    {
        SetComponentVariables();
    }

    /// <summary>
    /// Function to set the initial UI and other components of the gameobject instance
    /// </summary>
    private void SetComponentVariables()
    {
        cardButton = this.GetComponent<Button>();
        cardButton.onClick.AddListener(CardClicked);
        cardTransform = this.GetComponent<Transform>();
        cardImageComponent = this.GetComponent<Image>();
        cardID = -1;
    }

    /// <summary>
    /// Function called when player has clicked on this particular card
    /// </summary>
    private void CardClicked()
    {
        if (isTurning || !isCardHidden || !GameManager.hasGameStarted)
        {
            return;
        }
        FlipCard();
        //calling the delegate event
        //this helps in decoupling the reference assignment to GameManager or vice versa
        CardClickedCB?.Invoke(this);
    }

    /// <summary>
    /// Function to play the card flip audio and start animating the card flipping
    /// </summary>
    public void FlipCard()
    {
        isTurning = true;
        //play card flip sound
        GameAudioManager.Instance.PlayCardFlippedSound();
        //flip the card
        StartCoroutine(FlipCardRotation(0.25f, true));
    }

    /// <summary>
    /// Coroutine to slip the card and simultaneously assign the respective sprite
    /// based on the "changeSprite" function
    /// </summary>
    /// <param name="time"></param>
    /// <param name="changeSprite"></param>
    /// <returns></returns>
    private IEnumerator FlipCardRotation(float time, bool changeSprite)
    {
        //Animation for card flipping
        Quaternion startRotation = cardTransform.rotation;
        Quaternion endRotation = cardTransform.rotation * Quaternion.Euler(new Vector3(0, 90, 0));
        float rate = 1.0f / time;
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * rate;
            cardTransform.rotation = Quaternion.Slerp(startRotation, endRotation, t);

            yield return null;
        }

        //if sprite needs to be changed, then set the isCardHidden variable and call ChangeCardSprite function
        if (changeSprite)
        {
            isCardHidden = !isCardHidden;
            ChangeCardSprite();
            StartCoroutine(FlipCardRotation(time, false));
        }
        else
        {
            isTurning = false;
        }
    }

    /// <summary>
    /// Change the card sprite based on whether the card is flipped or not
    /// </summary>
    private void ChangeCardSprite()
    {
        if (cardID == -1 || cardSprite == null)
        {
            return;
        }
        if (isCardHidden)
        {
            UpdateCardImage(defaultSprite);
        }
        else
        {
            UpdateCardImage(cardSprite);
        }
    }

    /// <summary>
    /// Assign the sprite to the Image component of the card
    /// </summary>
    /// <param name="value"></param>
    private void UpdateCardImage(Sprite value)
    {
        cardImageComponent.sprite = value;
    }

    /// <summary>
    /// Called by GameManager to initialize and assign the cardID and sprite
    /// cardID will be used to compare for match making
    /// </summary>
    /// <param name="id"></param>
    /// <param name="sprite"></param>
    public void InitializeCard(int id, Sprite sprite)
    {
        cardID = id;
        CardSprite = sprite;
        isCardHidden = false;
        this.transform.localEulerAngles = new Vector3(0, 0, 0);
        //added this just for debug purposes
        this.name = "Card" + cardID;
    }

    /// <summary>
    /// Reset the card to default value
    /// </summary>
    public void ResetCard()
    {
        cardID = -1;
        cardSprite = defaultSprite;
        this.transform.localEulerAngles = new Vector3(0, 180, 0);
    }

    /// <summary>
    /// Flip the card back and hide it
    /// </summary>
    public void HideCard()
    {
        FlipCard();
    }

    /// <summary>
    /// Function to animate card fading away and then destroy the instance
    /// </summary>
    public void RemoveCard()
    {
        StartCoroutine(Fade());
    }

    /// <summary>
    /// Coroutine to animate the card fade away and then destroy the instance
    /// </summary>
    /// <returns></returns>
    private IEnumerator Fade()
    {
        float rate = 0.01f;
        float t = 0.0f;
        while (t < 2.5f)
        {
            t += Time.deltaTime * rate;
            cardImageComponent.color = Color.Lerp(cardImageComponent.color, Color.clear, t);

            yield return null;
        }
        DestroyImmediate(this.gameObject);
    }
}
