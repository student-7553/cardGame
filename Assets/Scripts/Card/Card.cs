using UnityEngine;
using System.Collections.Generic;
using Core;
using TMPro;
using Helpers;

public class CardCorners
{
	public Vector3 leftTopCorner;
	public Vector3 rightTopCorner;
	public Vector3 leftBottomCorner;
	public Vector3 rightBottomCorner;

	public CardCorners(Vector3 leftTopCorner_, Vector3 rightTopCorner_, Vector3 leftBottomCorner_, Vector3 rightBottomCorner_)
	{
		leftTopCorner = leftTopCorner_;
		rightTopCorner = rightTopCorner_;
		leftBottomCorner = leftBottomCorner_;
		rightBottomCorner = rightBottomCorner_;
	}
}

public class Card : MonoBehaviour, IStackable, Interactable
{
	// -------------------- Interactable Members -------------------------
	private bool _isDisabled;
	public bool isDisabled
	{
		get { return _isDisabled; }
		set
		{
			_isDisabled = value;
			reflectScreen();
		}
	}

	public SpriteRenderer spriteRenderer { get; set; }
	public CoreInteractableType interactableType { get; set; }

	public GameObject getGameObject()
	{
		return gameObject;
	}

	public Card getCard()
	{
		if (interactableType != CoreInteractableType.Cards)
		{
			return null;
		}
		return this.GetComponent(typeof(Card)) as Card;
	}

	public CardCorners corners;

	public int id;

	public bool isStacked;

	public CardStack joinedStack;

	public float timer;

	private TextMeshPro titleTextMesh;

	// --------------------Readonly Stats-------------------------
	public static float baseCardX = 5;
	public static float baseCardY = 8;

	private void Awake()
	{
		Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));

		if (textMeshes.Length > 0)
		{
			titleTextMesh = textMeshes[0] as TextMeshPro;
		}

		spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;

		this.computeCorners();
		isStacked = false;
		isDisabled = false;
		timer = 0;
		interactableType = CoreInteractableType.Cards;
	}

	public void moveCard(Vector3 newPosition)
	{
		gameObject.transform.position = newPosition;
		this.computeCorners();
	}

	public void computeCorners()
	{
		this.corners = this.generateTheCorners();
	}

	public void removeFromCardStack()
	{
		isStacked = false;
		joinedStack = null;
		if (gameObject.activeSelf == false)
		{
			gameObject.SetActive(true);
		}
	}

	public void addToCardStack(CardStack newCardStack)
	{
		isStacked = true;
		joinedStack = newCardStack;
	}

	public void stackOnThis(Card draggingCard)
	{
		if (isStacked)
		{
			CardStack existingstack = joinedStack;
			existingstack.addCardToStack(draggingCard);
		}
		else
		{
			List<Card> newCardStackCards = new List<Card>(new Card[] { this });
			newCardStackCards.Add(draggingCard);
			CardStack newStack = new CardStack(CardStackType.Cards, null);
			newStack.addCardToStack(newCardStackCards);
		}
	}

	public void init()
	{
		reflectScreen();
	}

	public void reflectScreen()
	{
		string cardTitle = "";
		if (CardDictionary.globalCardDictionary.ContainsKey(id))
		{
			if (titleTextMesh != null)
			{
				cardTitle = cardTitle + CardDictionary.globalCardDictionary[id].name;
			}
		}
		if (isDisabled)
		{
			cardTitle = "[DISABLED] " + cardTitle;
		}

		if (titleTextMesh != null)
		{
			titleTextMesh.text = cardTitle;
		}
	}

	private CardCorners generateTheCorners()
	{
		Vector3 leftTopCornerPoint = new Vector3(
			gameObject.transform.position.x - (baseCardX / 2),
			gameObject.transform.position.y + (baseCardY / 2),
			gameObject.transform.position.z
		);

		Vector3 rightTopCornerPoint = new Vector3(
			gameObject.transform.position.x + (baseCardX / 2),
			gameObject.transform.position.y + (baseCardY / 2),
			gameObject.transform.position.z
		);

		Vector3 leftBottomCornerPoint = new Vector3(
			gameObject.transform.position.x - (baseCardX / 2),
			gameObject.transform.position.y - (baseCardY / 2),
			gameObject.transform.position.z
		);

		Vector3 rightBottomCornerPoint = new Vector3(
			gameObject.transform.position.x + (baseCardX / 2),
			gameObject.transform.position.y - (baseCardY / 2),
			gameObject.transform.position.z
		);

		CardCorners newCorners = new CardCorners(leftTopCornerPoint, rightTopCornerPoint, leftBottomCornerPoint, rightBottomCornerPoint);
		return newCorners;
	}
}
