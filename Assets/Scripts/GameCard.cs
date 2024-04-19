using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GameCard : MonoBehaviour
{
    private Button cardButton;
    private bool isCardBeingFlipped;
    private Transform cardTransform;
    public int cardID { get; set; }
    public Sprite cardSprite
    {
        set
        {
            UpdateCardImage(value);
        }
    }
    private Image cardImageComponent;

    [SerializeField]
    private Sprite defaultSprite;

    private void Awake()
    {
        cardButton = this.GetComponent<Button>();
        cardButton.onClick.AddListener(CardClicked);
        cardTransform = this.GetComponent<Transform>();
        cardImageComponent = this.GetComponent<Image>();
        cardID = -1;
    }

    void CardClicked()
    {
        //play card flip sound
        GameAudioManager.Instance.PlayCardFlippedSound();
        //flip the card
        StartCoroutine(FlipCardRotation(0.25f, true));
    }


    private IEnumerator FlipCardRotation(float time, bool doubleFlip = true)
    {
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

        if (doubleFlip)
        {
            StartCoroutine(FlipCardRotation(time, false));
        }
    }

    public void SetCardID(int id)
    {
        cardID = id;
    }

    private void UpdateCardImage(Sprite value)
    {
        cardImageComponent.sprite = value;
    }

    public void ResetCard()
    {
        cardID = -1;
        cardSprite = defaultSprite;
        this.transform.localEulerAngles = new Vector3(0, 0, 0);
    }
}
