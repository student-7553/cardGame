using UnityEngine;
using System.Collections.Generic;
using Core;
using TMPro;

public class Card : MonoBehaviour, IStackable
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

	private CoreInteractable coreInteractable;

	public bool isStacked;

	public CardStack joinedStack;

	public int id;

	private bool _isDisabled;
	public bool isDisabled
	{
		get { return _isDisabled; }
		set
		{
			if (_isDisabled != value)
			{
				coreInteractable.isDisabled = value;
			}
			_isDisabled = value;
			reflectScreen();
		}
	}

	public float timer;

	private TextMeshPro titleTextMesh;

	// --------------------Readonly Stats-------------------------
	public static float baseCardX = 5;
	public static float baseCardY = 8;

	private void Awake()
	{
		this.generateTheCorners();
		isStacked = false;
		isDisabled = false;
		timer = 0;

		Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));
		Debug.Log(textMeshes.Length);
		if (textMeshes.Length > 0)
		{
			titleTextMesh = textMeshes[0] as TextMeshPro;
		}
	}

	public void moveCard(Vector3 newPosition)
	{
		gameObject.transform.position = newPosition;
		generateTheCorners();
	}

	private void FixedUpdate()
	{
		// if (timer != 0f)
		// {
		//     Debug.Log(timer);
		//     if (timer > 0.1f)
		//     {
		//         timer = timer - Time.deltaTime;
		//     }
		//     else
		//     {
		//         timer = 0f;
		//     }
		//     reflectScreen();
		// }
	}

	public void generateTheCorners()
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

		leftTopCorner = leftTopCornerPoint;
		rightTopCorner = rightTopCornerPoint;
		leftBottomCorner = leftBottomCornerPoint;
		rightBottomCorner = rightBottomCornerPoint;
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
			CardStack newStack = new CardStack(CardStackType.Cards, null);
			newStack.addCardsToStack(newCardStackCards);
		}
	}

	public void init()
	{
		reflectScreen();
		coreInteractable = gameObject.GetComponent(typeof(CoreInteractable)) as CoreInteractable;
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
}
