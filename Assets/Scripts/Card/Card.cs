
using UnityEngine;
using System.Collections.Generic;
using Core;
using TMPro;

public class Card : MonoBehaviour, Stackable
{

    // -------------------- Meta Stats -------------------------
    [System.NonSerialized]
    public Vector3 leftTopCorner;
    [System.NonSerialized]
    public Vector3 rightTopCorner;
    [System.NonSerialized]
    public Vector3 leftBottomCorner;
    [System.NonSerialized]
    public Vector3 rightBottomCorner;
    [System.NonSerialized]

    public bool isStacked;

    public CardStack joinedStack;

    public int id;

    public bool isGettingProccessed;

    // private float remainingDisabledTime;

    private TextMeshPro titleTextMesh;


    // --------------------Readonly Stats-------------------------
    public static float baseCardX = 5;
    public static float baseCardY = 8;

    private void Awake()
    {
        this.generateTheCorners();
        isStacked = false;
        isGettingProccessed = false;

        Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));
        if (textMeshes.Length > 0)
        {
            titleTextMesh = textMeshes[0] as TextMeshPro;
        }

    }

    public void generateTheCorners()
    {
        Vector3 leftTopCornerPoint = new Vector3(gameObject.transform.position.x - (baseCardX / 2),
            gameObject.transform.position.y + (baseCardY / 2),
            gameObject.transform.position.z);

        Vector3 rightTopCornerPoint = new Vector3(gameObject.transform.position.x + (baseCardX / 2),
            gameObject.transform.position.y + (baseCardY / 2),
            gameObject.transform.position.z);

        Vector3 leftBottomCornerPoint = new Vector3(gameObject.transform.position.x - (baseCardX / 2),
            gameObject.transform.position.y - (baseCardY / 2),
            gameObject.transform.position.z);

        Vector3 rightBottomCornerPoint = new Vector3(gameObject.transform.position.x + (baseCardX / 2),
            gameObject.transform.position.y - (baseCardY / 2),
            gameObject.transform.position.z);

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

    public void init()
    {
        reflectScreen();
    }

    private void reflectScreen()
    {
        if (CardDictionary.globalCardDictionary.ContainsKey(id))
        {
            if (titleTextMesh != null)
            {
                titleTextMesh.text = CardDictionary.globalCardDictionary[id].name;
            }
        }
    }


}
