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

public enum CardDisableType
{
	Que,
	Process,
	Dead,
	AutoMoving
}

public class CardDisable
{
	public CardDisableType? disableType;

	public CardDisable()
	{
		disableType = null;
	}
}

public class Card : MonoBehaviour, IStackable, Interactable
{
	// -------------------- Interactable Members -------------------------
	private bool _isInteractiveDisabled = false;
	public bool isInteractiveDisabled
	{
		get { return _isInteractiveDisabled; }
		set { _isInteractiveDisabled = value; }
	}

	public SpriteRenderer spriteRenderer { get; set; }
	public CoreInteractableType interactableType
	{
		get { return CoreInteractableType.Cards; }
	}

	public Card getCard()
	{
		return this;
	}

	public CardCorners corners;

	public int id;

	public CardStack joinedStack;

	private bool _isStacked = false;

	public bool isStacked
	{
		get { return _isStacked; }
		set
		{
			bool preValue = _isStacked;
			_isStacked = value;

			if (value == false && preValue == true)
			{
				if (joinedStack != null)
				{
					joinedStack.removeCardsFromStack(new List<Card>() { this });
					joinedStack = null;
				}
				gameObject.SetActive(true);
				gameObject.transform.SetParent(null);
			}
		}
	}

	public CardDisable cardDisable;

	private TextMeshPro titleTextMesh;

	public InteractableManagerScriptableObject interactableManagerScriptableObject;

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

		computeCorners();
		cardDisable = new CardDisable();
	}

	private void FixedUpdate()
	{
		reflectScreen();
	}

	public void moveCard(Vector3 newPosition)
	{
		gameObject.transform.position = newPosition;
		computeCorners();
	}

	public void destroyCard()
	{
		if (isStacked)
		{
			isStacked = false;
		}
		interactableManagerScriptableObject.removeCard(this);
		Destroy(gameObject);
	}

	public void computeCorners()
	{
		if (this == null)
		{
			return;
		}
		corners = generateTheCorners();
	}

	public void addToCardStack(CardStack newCardStack)
	{
		isStacked = true;
		joinedStack = newCardStack;
	}

	public void stackOnThis(Card draggingCard, Node _prevNode)
	{
		if (isStacked)
		{
			CardStack existingstack = joinedStack;
			existingstack.addCardToStack(draggingCard);
		}
		else
		{
			List<Card> newCardStackCards = new List<Card> { this, draggingCard };
			CardStack newStack = new CardStack(null);
			newStack.addCardToStack(newCardStackCards);
		}
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
		if (isInteractiveDisabled)
		{
			string disabledTitle = "[DISABLED] ";

			if (cardDisable.disableType != null)
			{
				disabledTitle = disabledTitle + $"[{cardDisable.disableType}]";
				if (cardDisable.disableType == CardDisableType.AutoMoving)
				{
					spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.3f);
				}
				else
				{
					spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
				}
			}
			cardTitle = disabledTitle + cardTitle;
		}

		if (titleTextMesh != null)
		{
			titleTextMesh.text = cardTitle;
		}
	}

	public void disableInteractiveForATime(float timer, CardDisableType disableType)
	{
		cardDisable.disableType = disableType;
		isInteractiveDisabled = true;
	}

	private CardCorners generateTheCorners()
	{
		if (gameObject == null)
		{
			return null;
		}
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
