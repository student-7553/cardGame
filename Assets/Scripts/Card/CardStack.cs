using System.Collections.Generic;
using UnityEngine;
using Core;
using Helpers;
using System.Linq;
using UnityEditor.PackageManager;
using System;

public class CardStack
{
	private static float stackDistance = 5;
	private static float zDistancePerCards = 0.01f;

	public BaseNode connectedNode;

	public CardStackType cardStackType;

	public Vector3 originPointAdjustment;

	private List<BaseCard> _cards;

	public List<BaseCard> cards
	{
		get { return _cards; }
		set { _cards = value; }
	}

	public CardStack(BaseNode _connectedNode)
	{
		originPointAdjustment = new Vector3();
		cards = new List<BaseCard>();
		if (_connectedNode == null)
		{
			cardStackType = CardStackType.Cards;
		}
		else
		{
			cardStackType = CardStackType.Nodes;
			connectedNode = _connectedNode;
		}
	}

	public void alignCards(Vector3 originPoint)
	{
		if (cards.Count == 0)
		{
			return;
		}

		float paddingCounter = 0;
		foreach (BaseCard singleCard in cards)
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
				: getRootPosition();

		originPoint = originPoint + originPointAdjustment;

		alignCards(originPoint);
	}

	public void removeCardsFromStack(List<BaseCard> removingCards)
	{
		bool changed = false;

		foreach (BaseCard singleCard in removingCards)
		{
			if (!cards.Contains(singleCard))
			{
				continue;
			}
			changed = true;
			singleCard.joinedStack = null;
			singleCard.isStacked = false;

			cards.Remove(singleCard);
		}
		if (changed)
		{
			alignCards();
		}
	}

	public void addCardToStack(List<BaseCard> addingCards)
	{
		addCardsToStack(addingCards);
		collapseCardStack();
	}

	private void addCardsToStack(List<BaseCard> addingCards)
	{
		cards.AddRange(addingCards);
		foreach (BaseCard singleCard in cards)
		{
			if (singleCard.isStacked)
			{
				continue;
			}

			singleCard.attachToCardStack(this);
			if (cardStackType == CardStackType.Nodes)
			{
				singleCard.gameObject.transform.SetParent(connectedNode.nodePlaneManager.gameObject.transform);
			}
		}
		alignCards();
	}

	private void collapseCardStack()
	{
		BaseCard targetBaseCard = findCollapsableCard();

		while (targetBaseCard != null)
		{
			if (targetBaseCard.interactableType == CoreInteractableType.CollapsedCards)
			{
				List<BaseCard> subjectCards = cards
					.Where((card) => card.interactableType == CoreInteractableType.Cards && card.id == targetBaseCard.id)
					.ToList();
				CardCollapsed cardCollapsed = targetBaseCard.getCollapsedCard();
				cardCollapsed.addToCollapsedCards(subjectCards);
				removeCardsFromStack(subjectCards);
			}
			else if (targetBaseCard.interactableType == CoreInteractableType.Cards)
			{
				Debug.Log("cardsCount /" + cards.Count);

				List<BaseCard> subjectCards = cards
					.Where((card) => card.interactableType == CoreInteractableType.Cards && card.id == targetBaseCard.id)
					.ToList();

				Debug.Log("subjectCards count /" + subjectCards.Count);

				CardCollapsed cardCollapsed = CardHandler.current.createCardCollapsed(targetBaseCard.id);

				Debug.Log("Adding to collapsed/" + subjectCards.Count);

				cardCollapsed.addToCollapsedCards(subjectCards);

				addCardsToStack(new List<BaseCard>() { cardCollapsed });
				removeCardsFromStack(subjectCards);
			}
			else
			{
				Debug.LogError("collapseCardStack should not happen here");
				break;
			}
			break;
			// targetBaseCard = findCollapsableCard();
		}
	}

	private BaseCard findCollapsableCard()
	{
		foreach (BaseCard singleCard in cards)
		{
			if (singleCard.interactableType == CoreInteractableType.Nodes)
			{
				continue;
			}
			if (singleCard.interactableType == CoreInteractableType.CollapsedCards)
			{
				List<BaseCard> subjectCards = cards
					.Where((card) => card.interactableType == CoreInteractableType.Cards && singleCard != card && card.id == singleCard.id)
					.ToList();

				if (subjectCards.Count == 0)
				{
					continue;
				}
				return singleCard;
			}
			else if (singleCard.interactableType == CoreInteractableType.Cards)
			{
				List<BaseCard> subjectCards = cards
					.Where((card) => card.interactableType == CoreInteractableType.Cards && singleCard != card && card.id == singleCard.id)
					.ToList();
				if (subjectCards.Count == 0)
				{
					continue;
				}
				return singleCard;
			}
		}
		return null;
	}

	public void addCardToStack(BaseCard addingCard)
	{
		addCardToStack(new List<BaseCard>() { addingCard });
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

	private Vector3 getRootPosition()
	{
		if (cards.Count > 0)
		{
			return cards[0].transform.position;
		}
		return Vector3.zero;
	}
}
