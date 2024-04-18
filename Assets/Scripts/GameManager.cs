using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameCardSO[] gameCardScriptableObjectArray;
    private GameCard[] availableCards;
    // Start is called before the first frame update
    void Start()
    {
        LoadAllCardInformationData();
    }

    /// <summary>
    /// Function to load all the available card information from the card scriptable objects
    /// </summary>
    void LoadAllCardInformationData()
    {
        gameCardScriptableObjectArray = Resources.LoadAll<GameCardSO>("GameCards");

    }
}
