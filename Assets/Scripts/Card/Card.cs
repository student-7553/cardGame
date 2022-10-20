
using UnityEngine;
using System.Collections.Generic;
using Core;

public class Card : MonoBehaviour, Stackable
{

    // -------------------- Card Own Stats -------------------------

    // private int _inventoryCount;
    // public int inventoryCount
    // {
    //     get { return _inventoryCount; }
    //     set { _inventoryCount = value; }
    // }

    // private float _travelTime;
    // public float travelTime
    // {
    //     get { return _travelTime; }
    //     set { _travelTime = value; }
    // }


    // -------------------- Meta Stats -------------------------
    public bool isStacked;
    public CardStack joinedStack;
    [System.NonSerialized]
    public Vector3 leftTopCorner;
    [System.NonSerialized]
    public Vector3 rightTopCorner;
    [System.NonSerialized]
    public Vector3 leftBottomCorner;
    [System.NonSerialized]
    public Vector3 rightBottomCorner;
    [System.NonSerialized]

    public int id;

    public bool isDisabled;

    private float remainingDisabledTime;


    // --------------------Readonly Stats-------------------------
    private readonly float baseCardX = 5;
    private readonly float baseCardY = 8;

    private void Awake()
    {
        this.generateTheCorners();
        isStacked = false;
    }

    public void generateTheCorners()
    {
        Vector3 leftTopCornerPoint = new Vector3(gameObject.transform.position.x - (baseCardX / 2),
        gameObject.transform.position.y, gameObject.transform.position.z + (baseCardY / 2));

        Vector3 rightTopCornerPoint = new Vector3(gameObject.transform.position.x + (baseCardX / 2),
        gameObject.transform.position.y, gameObject.transform.position.z + (baseCardY / 2));

        Vector3 leftBottomCornerPoint = new Vector3(gameObject.transform.position.x - (baseCardX / 2),
        gameObject.transform.position.y, gameObject.transform.position.z - (baseCardY / 2));

        Vector3 rightBottomCornerPoint = new Vector3(gameObject.transform.position.x + (baseCardX / 2),
        gameObject.transform.position.y, gameObject.transform.position.z - (baseCardY / 2));

        leftTopCorner = leftTopCornerPoint;
        rightTopCorner = rightTopCornerPoint;
        leftBottomCorner = leftBottomCornerPoint;
        rightBottomCorner = rightBottomCornerPoint;
    }

    public void removeFromCardStack()
    {
        isStacked = false;
        joinedStack = null;
    }

    public void addToCardStack(CardStack newCardStack)
    {
        isStacked = true;
        joinedStack = newCardStack;
    }

    public void stackOnThis(List<Card> draggingCards)
    {
        if (isStacked)
        {
            CardStack existingstack = joinedStack;
            existingstack.addCardsToStack(draggingCards);
        }
        else
        {
            List<Card> newCardStackCards = new List<Card>(new Card[] { this });
            newCardStackCards.AddRange(draggingCards);
            CardStack newStack = new CardStack(CardStackType.Cards);
            newStack.addCardsToStack(newCardStackCards);
        }
    }
}
