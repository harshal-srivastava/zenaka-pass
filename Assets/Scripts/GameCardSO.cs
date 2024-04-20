using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class representing the scriptable object of cards
/// Using scriptable object would streamline the process of adding any more cards to game
/// If user wants to add any more card, just create a scriptable object of it, assign ID and sprite
/// Code will pick this change automatically
/// Hence, no code or scene change required to add any new card to the game
/// </summary>
[CreateAssetMenu(fileName = "Card", menuName = "CardGame/Card")]
public class GameCardSO : ScriptableObject
{
    public int cardID;
    public Sprite cardSprite;
}
