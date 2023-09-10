using System.Collections.Generic;
using UnityEngine;
using Core;
using Helpers;

public class CardStack
{
	private static float stackDistance = 5;
	private static float zDistancePerCards = 0.01f;

	public BaseNode connectedNode;

	public CardStackType cardStackType;

	public Vector3 originPointAdjustment;

	private List<Card> _cards;

	public List<Card> cards
	{
		get { return _cards; }
		set
		{
			_cards = value;
			alignCards();
		}
	}

	public CardStack(BaseNode spawningNode)
	{
		originPointAdjustment = new Vector3();
		cards = new List<Card>();
		if (spawningNode == null)
		{
			cardStackType = CardStackType.Cards;
		}
		else
		{
			cardStackType = CardStackType.Nodes;
			connectedNode = spawningNode;
		}
	}

	public void alignCards(Vector3 originPoint)
	{
		if (cards.Count == 0)
		{
			return;
		}

		float paddingCounter = 0;
		foreach (Card singleCard in cards)
		{
			if (singleCard == null)
			{
				continue;
			}

			Vector3 newPostionForCardInSubject = new Vector3(originPoint.x, originPoint.y, getPositionZ());
			newPostionForCardInSubject.y = newPostionForCardInSubject.y - (paddingCounter * stackDistance);
			newPostionForCardInSubject.z = newPostionForCardInSubject.z - (paddingCounter * zDistancePerCards);

			paddingCounter++;
			singleCard.transform.position = newPostionForCardInSubject;
			singleCard.computeCorners();
		}
	}

	public void alignCards()
	{
		if (cards.Count == 0)
		{
			return;
		}
		Vector3 originPoint =
			cardStackType == CardStackType.Nodes
				? new Vector3(
					connectedNode.nodePlaneManager.gameObject.transform.position.x,
					connectedNode.nodePlaneManager.gameObject.transform.position.y,
					getPositionZ()
				)
				: getRootCard().transform.position;

		originPoint = originPoint + originPointAdjustment;

		alignCards(originPoint);
	}

	public void removeCardsFromStack(List<Card> removingCards)
	{
		bool changed = false;

		foreach (Card singleCard in removingCards)
		{
			if (!cards.Contains(singleCard))
			{
				continue;
			}
			changed = true;
			cards.Remove(singleCard);
			singleCard.isStacked = false;
		}
		if (changed)
		{
			alignCards();
		}
	}

	public List<Card> handleCreatingCards(List<int> cardIds)
	{
		if (cardIds.Count == 0)
		{
			return new List<Card>();
		}

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
		addCardToStack(addingCards);
		return addingCards;
	}

	public void consolidateTypeCards()
	{
		foreach (CardsTypes cardType in CardHelpers.valueCardTypes)
		{
			List<int> cardIds = getTypeActiveCards(cardType);

			int value = CardHelpers.getTypeValueFromCardIds(cardType, cardIds);

			List<int> generatingCardIds = CardHelpers.generateTypeValueCards(cardType, value);

			List<int> newCardIds = getListEdge(generatingCardIds, cardIds);

			List<int> removingCardIds = getListEdge(cardIds, generatingCardIds);

			List<Card> removingCards = getCards(removingCardIds);

			if (removingCards.Count > 0)
			{
				foreach (Card singleRemovingCard in removingCards)
				{
					singleRemovingCard.destroyCard();
				}
			}

			if (newCardIds.Count > 0)
			{
				handleCreatingCards(newCardIds);
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

	public void addCardToStack(List<Card> addingCards)
	{
		cards.AddRange(addingCards);
		// alignCards();
		foreach (Card singleCard in addingCards)
		{
			singleCard.addToCardStack(this);

			if (cardStackType == CardStackType.Nodes)
			{
				singleCard.gameObject.transform.SetParent(connectedNode.nodePlaneManager.gameObject.transform);
			}
		}

		// consolidateTypeCards();
		alignCards();
	}

	public void addCardToStack(Card addingCard)
	{
		cards.Add(addingCard);

		addingCard.addToCardStack(this);
		if (cardStackType == CardStackType.Nodes)
		{
			addingCard.gameObject.transform.SetParent(connectedNode.nodePlaneManager.gameObject.transform);
		}

		// consolidateTypeCards();
		alignCards();
	}

	public List<int> getAllCardIds()
	{
		List<int> ids = new List<int>();
		foreach (Card singleCard in cards)
		{
			ids.Add(singleCard.id);
		}
		return ids;
	}

	public List<Card> getCards(List<int> cardIds)
	{
		Dictionary<int, int> indexedCardIds = CardHelpers.indexCardIds(cardIds);
		List<Card> returnCards = new List<Card>();

		foreach (Card singleCard in cards)
		{
			if (indexedCardIds.ContainsKey(singleCard.id))
			{
				indexedCardIds[singleCard.id]--;
				if (indexedCardIds[singleCard.id] == 0)
				{
					indexedCardIds.Remove(singleCard.id);
				}
				returnCards.Add(singleCard);
			}
		}

		return returnCards;
	}

	public List<int> getAllCardIdsOfMinusIntervalModules()
	{
		List<int> modules = new List<int>();
		foreach (Card singleCard in cards)
		{
			if (
				CardDictionary.globalCardDictionary[singleCard.id].module != null
				&& CardDictionary.globalCardDictionary[singleCard.id].module.minusInterval != null
				&& CardDictionary.globalCardDictionary[singleCard.id].module.minusInterval.time != 0
			)
			{
				modules.Add(singleCard.id);
			}
		}
		return modules;
	}

	public List<int> getAllCardIdsOfUnityModules()
	{
		List<int> modules = new List<int>();
		foreach (Card singleCard in cards)
		{
			if (
				CardDictionary.globalCardDictionary[singleCard.id].module != null
				&& CardDictionary.globalCardDictionary[singleCard.id].module.unityCount != 0
			)
			{
				modules.Add(singleCard.id);
			}
		}
		return modules;
	}

	public List<int> getActiveCardIds()
	{
		List<int> ids = new List<int>();
		foreach (Card singleCard in cards)
		{
			if (!singleCard.isInteractiveDisabled)
			{
				ids.Add(singleCard.id);
			}
		}
		return ids;
	}

	public List<Card> getActiveCards()
	{
		List<Card> returnCards = new List<Card>();
		foreach (Card singleCard in cards)
		{
			if (!singleCard.isInteractiveDisabled)
			{
				returnCards.Add(singleCard);
			}
		}
		return returnCards;
	}

	public List<int> getNonTypeActiveCardIds()
	{
		List<int> ids = new List<int>();
		foreach (Card singleCard in cards)
		{
			if (
				CardHelpers.isNonValueTypeCard(CardDictionary.globalCardDictionary[singleCard.id].type)
				&& singleCard.isInteractiveDisabled == false
			)
			{
				ids.Add(singleCard.id);
			}
		}
		return ids;
	}

	public List<int> getTypeActiveCards(CardsTypes cardType)
	{
		List<int> ids = new List<int>();
		foreach (Card singleCard in cards)
		{
			if (CardDictionary.globalCardDictionary[singleCard.id].type == cardType && singleCard.isInteractiveDisabled == false)
			{
				ids.Add(singleCard.id);
			}
		}
		return ids;
	}

	public List<int> getMagnetizedCards()
	{
		List<int> magnetizedCards = new List<int>();
		foreach (Card card in cards)
		{
			if (
				CardDictionary.globalCardDictionary[card.id].module != null
				&& CardDictionary.globalCardDictionary[card.id].module.isMagnetizedCardIds != null
			)
			{
				magnetizedCards.AddRange(CardDictionary.globalCardDictionary[card.id].module.isMagnetizedCardIds);
			}
		}
		return magnetizedCards;
	}

	private float getPositionZ()
	{
		if (cardStackType == CardStackType.Cards)
		{
			return HelperData.baseZ;
		}
		return HelperData.nodeBoardZ - 1;
	}

	private Card getRootCard()
	{
		if (cards.Count > 0)
		{
			return cards[0];
		}
		return null;
	}
}
