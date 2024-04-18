using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "CardGame/Card")]
public class GameCardSO : ScriptableObject
{
    public int cardID;
    public Sprite cardSprite;
}
