using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Core;
using Helpers;

public class Node : MonoBehaviour, IStackable, IClickable, Interactable
{
	// -------------------- Interactable Members -------------------------
	public bool isInteractiveDisabled { get; set; }
	public SpriteRenderer spriteRenderer { get; set; }
	public CoreInteractableType interactableType { get; set; }

	// -------------------- Custom Class -------------------------
	[System.NonSerialized]
	public NodeCardQue nodeCardQue;

	[System.NonSerialized]
	public NodeTextHandler nodeTextHandler;

	[System.NonSerialized]
	public NodeStats nodeStats;

	[System.NonSerialized]
	public NodeProcess nodeProcess;

	[System.NonSerialized]
	public NodePlaneHandler nodePlaneManager;

	[System.NonSerialized]
	public NodeHungerHandler nodeHungerHandler;

	public CardStack storageCardStack;

	public CardStack processCardStack;

	// -------------------- Node Stats -------------------------

	private int _id;
	public int id
	{
		get { return _id; }
		set
		{
			_id = value;
			nodeStats = new NodeStats(this);
		}
	}

	public bool isActive;

	private void Awake()
	{
		storageCardStack = new CardStack(this);
		storageCardStack.originPointAdjustment = new Vector3(10f, 25f, 0);

		processCardStack = new CardStack(this);
		processCardStack.originPointAdjustment = new Vector3(-10f, 25f, 0);

		nodeStats = new NodeStats(this);
		nodeTextHandler = new NodeTextHandler(this);

		isInteractiveDisabled = false;
		isActive = true;

		interactableType = CoreInteractableType.Nodes;
	}

	public void init(NodePlaneHandler nodePlane)
	{
		nodePlaneManager = nodePlane;

		nodeCardQue = gameObject.GetComponent(typeof(NodeCardQue)) as NodeCardQue;

		spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;

		nodeProcess = gameObject.GetComponent(typeof(NodeProcess)) as NodeProcess;
		nodeProcess.init(this);

		nodeHungerHandler = gameObject.GetComponent(typeof(NodeHungerHandler)) as NodeHungerHandler;
		nodeHungerHandler.init(this);
	}

	public void OnClick()
	{
		if (nodePlaneManager.gameObject.activeSelf == true)
		{
			nodePlaneManager.gameObject.SetActive(false);
		}
		else
		{
			nodePlaneManager.gameObject.SetActive(true);
		}
	}

	public void stackOnThis(Card newCard, Node prevNode)
	{
		if (isMarket() && !CardDictionary.globalCardDictionary[newCard.id].isSellable)
		{
			return;
		}

		if (prevNode != this)
		{
			nodeCardQue.addCard(newCard);
		}

		storageCardStack.addCardToStack(newCard);
	}

	public void addCardToProcessCardStack(Card newCard)
	{
		processCardStack.addCardToStack(newCard);
	}

	private void FixedUpdate()
	{
		nodeStats.computeStats();
		nodeStats.handleLimits();
		nodeTextHandler.reflectToScreen();
	}

	public Card getCard()
	{
		if (interactableType != CoreInteractableType.Cards)
		{
			return null;
		}
		return this.GetComponent(typeof(Card)) as Card;
	}

	public bool isMarket()
	{
		if (id == 3003)
		{
			return true;
		}
		return false;
	}

	public IEnumerator queUpTypeDeletion(CardsTypes cardType, int typeValue, float timer, Action callback)
	{
		List<int> removingCardIds = new List<int>();
		List<int> addingCardIds = new List<int>();
		List<int> cardIds = storageCardStack.getActiveCardIds();

		this.handleTypeAdjusting(cardIds, cardType, typeValue, ref removingCardIds, ref addingCardIds);

		List<Card> removingCards = this.getCards(removingCardIds);
		foreach (Card card in removingCards)
		{
			card.isInteractiveDisabled = true;
			card.cardDisable.timer = timer;
			card.cardDisable.disableType = CardDisableType.Process;
		}

		yield return new WaitForSeconds(timer);

		this.hadleRemovingCards(removingCards);
		this.handleCreatingCards(addingCardIds);

		if (callback != null)
		{
			callback.Invoke();
		}
	}

	public void handleTypeAdjusting(
		List<int> availableCardIds,
		CardsTypes cardType,
		int requiredTypeValue,
		ref List<int> removingCardIds,
		ref List<int> addingCardIds
	)
	{
		int totalSum = 0;
		List<int> ascTypeCardIds = CardHelpers.getAscTypeValueCardIds(cardType, availableCardIds);
		foreach (int typeCardId in ascTypeCardIds)
		{
			removingCardIds.Add(typeCardId);
			totalSum = totalSum + CardDictionary.globalCardDictionary[typeCardId].typeValue;
			if (totalSum == requiredTypeValue)
			{
				break;
			}
			if (totalSum > requiredTypeValue)
			{
				int addingTypeValue = totalSum - requiredTypeValue;
				List<int> newCardIds = CardHelpers.generateTypeValueCards(cardType, addingTypeValue);
				addingCardIds.AddRange(newCardIds);
				break;
			}
		}
	}

	public void hadleRemovingCards(List<Card> removingCards)
	{
		storageCardStack.removeCardsFromStack(removingCards);

		foreach (Card singleRemovingCard in removingCards)
		{
			Destroy(singleRemovingCard.gameObject);
		}
	}

	public void handleCreatingCards(List<int> cardIds)
	{
		List<Card> addingCards = new List<Card>();
		foreach (int singleAddingCardId in cardIds)
		{
			if (CardDictionary.globalCardDictionary[singleAddingCardId].type == CardsTypes.Node)
			{
				CardHandler.current.createNode(singleAddingCardId);
			}
			else
			{
				Card createdCard = CardHandler.current.createCard(singleAddingCardId);
				addingCards.Add(createdCard);
			}
		}
		storageCardStack.addCardToStack(addingCards);
	}

	public List<Card> getCards(List<int> cardIds)
	{
		Dictionary<int, int> indexedCardIds = this.indexCardIds(cardIds);
		List<Card> cards = new List<Card>();

		foreach (Card singleCard in storageCardStack.cards)
		{
			if (indexedCardIds.ContainsKey(singleCard.id))
			{
				indexedCardIds[singleCard.id]--;
				if (indexedCardIds[singleCard.id] == 0)
				{
					indexedCardIds.Remove(singleCard.id);
				}
				cards.Add(singleCard);
			}
		}

		return cards;
	}

	public void ejectCards(List<Card> cards)
	{
		int positionMinusInterval = 4;
		Vector3 startingPosition = new Vector3(
			gameObject.transform.position.x,
			gameObject.transform.position.y - 25,
			gameObject.transform.position.z
		);
		storageCardStack.removeCardsFromStack(cards);

		foreach (Card card in cards)
		{
			Vector3 cardPostion = startingPosition;
			cardPostion.y = cardPostion.y - positionMinusInterval;
			card.moveCard(cardPostion);
		}
	}

	public Dictionary<int, int> indexCardIds(List<int> requiredIds)
	{
		Dictionary<int, int> indexedRequiredIds = new Dictionary<int, int>();
		foreach (int requiredId in requiredIds)
		{
			if (indexedRequiredIds.ContainsKey(requiredId))
			{
				indexedRequiredIds[requiredId] = indexedRequiredIds[requiredId] + 1;
			}
			else
			{
				indexedRequiredIds.Add(requiredId, 1);
			}
		}
		return indexedRequiredIds;
	}
}
