using UnityEngine;
using System.Collections.Generic;
using Core;
using System.Linq;
using Helpers;

public enum NodeCardStackType
{
	storage,
	process
}

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

	// -------------------- Node Stats -------------------------


	[System.NonSerialized]
	public CardStack processCardStack;

	private int _id;
	public int id
	{
		get { return _id; }
		set
		{
			_id = value;
			if (this.nodeHungerHandler != null)
			{
				this.nodeHungerHandler.intervalTimer = 0;
			}
			nodeStats = new NodeStats(this);
		}
	}

	public bool isActive;

	// ---------------------------------------------------------

	private void Awake()
	{
		processCardStack = new CardStack(this);
		processCardStack.originPointAdjustment = new Vector3(0f, 35f, 0);

		nodeTextHandler = new NodeTextHandler(this);

		isInteractiveDisabled = false;
		isActive = true;

		interactableType = CoreInteractableType.Nodes;
	}

	private void FixedUpdate()
	{
		nodeStats.computeStats();
		nodeStats.handleLimits();
		nodeTextHandler.reflectToScreen();
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
		if (isMarket())
		{
			if (!CardDictionary.globalCardDictionary[newCard.id].isSellable)
			{
				return;
			}

			if (prevNode != this)
			{
				nodeCardQue.addCard(newCard);
			}
			processCardStack.addCardToStack(newCard);
			return;
		}

		if (
			CardDictionary.globalCardDictionary[newCard.id].resourceInventoryCount + nodeStats.currentNodeStats.resourceInventoryUsed
			>= nodeStats.currentNodeStats.resourceInventoryLimit
		)
		{
			return;
		}

		if (
			CardDictionary.globalCardDictionary[newCard.id].infraInventoryCount + +nodeStats.currentNodeStats.infraInventoryUsed
			>= nodeStats.currentNodeStats.infraInventoryLimit
		)
		{
			return;
		}

		if (prevNode != this)
		{
			nodeCardQue.addCard(newCard);
		}
		processCardStack.addCardToStack(newCard);
	}

	public void hadleRemovingCards(List<Card> removingCards)
	{
		if (removingCards.Count == 0)
		{
			return;
		}
		processCardStack.removeCardsFromStack(removingCards);

		foreach (Card singleRemovingCard in removingCards)
		{
			Destroy(singleRemovingCard.gameObject);
		}
	}

	public void handleCreatingCards(List<int> cardIds)
	{
		if (cardIds.Count == 0)
		{
			return;
		}

		List<Card> addingNonTypeCards = new List<Card>();
		List<Card> addingTypeCards = new List<Card>();
		foreach (int singleAddingCardId in cardIds)
		{
			if (CardDictionary.globalCardDictionary[singleAddingCardId].type == CardsTypes.Node)
			{
				CardHandler.current.createNode(singleAddingCardId);
			}
			else
			{
				Card createdCard = CardHandler.current.createCard(singleAddingCardId);

				if (CardHelpers.isNonValueTypeCard(CardDictionary.globalCardDictionary[singleAddingCardId].type))
				{
					addingNonTypeCards.Add(createdCard);
				}
				else
				{
					addingTypeCards.Add(createdCard);
				}
			}
		}

		this.ejectCards(addingNonTypeCards);

		processCardStack.addCardToStack(addingTypeCards);
	}

	public List<Card> getCards(List<int> cardIds)
	{
		Dictionary<int, int> indexedCardIds = this.indexCardIds(cardIds);
		List<Card> cards = new List<Card>();

		foreach (Card singleCard in processCardStack.cards)
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
		int positionMinusInterval = 9;

		Vector3 startingPosition = new Vector3(
			gameObject.transform.position.x,
			gameObject.transform.position.y - 4,
			gameObject.transform.position.z
		);
		processCardStack.removeCardsFromStack(cards);

		foreach (Card card in cards)
		{
			startingPosition.y = startingPosition.y - positionMinusInterval;
			card.moveCard(startingPosition);
		}
	}

	public Dictionary<int, int> indexCardIds(List<int> requiredIds)
	{
		Dictionary<int, int> indexedRequiredIds = new Dictionary<int, int>();
		foreach (int baseRequiredId in requiredIds)
		{
			if (indexedRequiredIds.ContainsKey(baseRequiredId))
			{
				indexedRequiredIds[baseRequiredId] = indexedRequiredIds[baseRequiredId] + 1;
			}
			else
			{
				indexedRequiredIds.Add(baseRequiredId, 1);
			}
		}
		return indexedRequiredIds;
	}

	public void consolidateTypeCards()
	{
		CardsTypes[] types = new CardsTypes[] { CardsTypes.Gold, CardsTypes.Electricity, CardsTypes.Food };
		foreach (CardsTypes cardType in types)
		{
			List<int> cardIds = processCardStack.getTypeActiveCards(cardType);

			int value = CardHelpers.getTypeValueFromCardIds(cardType, cardIds);

			List<int> generatingCardIds = CardHelpers.generateTypeValueCards(cardType, value);

			List<int> newCardIds = this.getListEdge(generatingCardIds, cardIds);

			List<int> removingCardIds = this.getListEdge(cardIds, generatingCardIds);

			List<Card> removingCards = this.getCards(removingCardIds);

			if (removingCards.Count > 0)
			{
				this.hadleRemovingCards(removingCards);
			}

			Debug.Log("newCardIds/" + newCardIds.Count);

			if (newCardIds.Count > 0)
			{
				Debug.Log(newCardIds[0]);
				this.handleCreatingCards(newCardIds);
			}
		}
	}

	private List<int> getListEdge(List<int> mainlist, List<int> excludeList)
	{
		List<int> newCardIds = new List<int>(mainlist);

		foreach (int removingCardId in excludeList)
		{
			int foundIndex = newCardIds.FindIndex((newCardId) => newCardId == removingCardId);
			if (foundIndex != -1)
			{
				newCardIds.RemoveAt(foundIndex);
			}
		}
		return newCardIds;
	}

	public bool isMarket()
	{
		if (id == 3003)
		{
			return true;
		}
		return false;
	}

	public Card getCard()
	{
		return null;
	}
}
