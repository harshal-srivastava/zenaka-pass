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
    public int cardID { get; private set; }
    public Sprite cardSprite { get; set; }
    private Image cardImageComponent;

    private void Awake()
    {
        cardButton = this.GetComponent<Button>();
        cardButton.onClick.AddListener(CardClicked);
        cardTransform = this.GetComponent<Transform>();
        cardImageComponent = this.GetComponent<Image>();
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
}
