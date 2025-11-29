using System.Collections.Generic;
using UnityEngine;
using Core;
using Helpers;
using System.Linq;
using DG.Tweening;

public class CardStack : CardHolder
{
	private static float stackDistance = 5;
	private static float zDistancePerCards = 0.01f;

	private BaseNode connectedNode;

	private CardStackType cardStackType;

	private List<BaseCard> _cards;

	public List<BaseCard> cards
	{
		get { return _cards; }
		set { _cards = value; }
	}

	private int topVisibleCards;
	private Vector3 originPointAdjustment;

	public CardStack(BaseNode _connectedNode, int topVisibleCards, Vector3 originPointAdjustment)
	{
		this.originPointAdjustment = originPointAdjustment;
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
		this.topVisibleCards = topVisibleCards;
	}

	public void alignCards(Vector3 originPoint)
	{
		if (cards.Count == 0)
		{
			return;
		}

		Vector3 adjustedOriginPoint =
			cardStackType == CardStackType.Nodes
				? new Vector3(
					connectedNode.nodePlaneManager.gameObject.transform.position.x,
					connectedNode.nodePlaneManager.gameObject.transform.position.y,
					getPositionZ()
				)
				: originPoint;

		adjustedOriginPoint = adjustedOriginPoint + originPointAdjustment;

		float paddingCounter = 0;
		for (int index = 0; index < cards.Count; index++)
		{
			BaseCard singleCard = cards[index];
			if (singleCard == null)
			{
				continue;
			}

			Vector3 newPostionForCardInSubject = new Vector3(adjustedOriginPoint.x, adjustedOriginPoint.y, getPositionZ());
			newPostionForCardInSubject.y = newPostionForCardInSubject.y - (paddingCounter * stackDistance);
			newPostionForCardInSubject.z = newPostionForCardInSubject.z - (paddingCounter * zDistancePerCards);

			paddingCounter++;

			moveBaseCard(singleCard, newPostionForCardInSubject);
			singleCard.computeCorners();

			if (topVisibleCards != 0)
			{
				if (index >= topVisibleCards)
				{
					singleCard.gameObject.SetActive(false);
				}
				else
				{
					singleCard.gameObject.SetActive(true);
				}
			}
		}
	}

	private void moveBaseCard(BaseCard card, Vector3 newPosition)
	{
		card.gameObject.transform.DOMove(newPosition, HelperData.cardReachSmoothTime).SetId(card.id);
	}

	private void collapseCardStack()
	{
		BaseCard targetBaseCard = findCollapsableBaseCard();

		while (targetBaseCard != null)
		{
			if (targetBaseCard.interactableType == CoreInteractableType.CollapsedCards)
			{
				List<BaseCard> subjectCards = cards
					.Where((card) => card.isCardType() && card.id == targetBaseCard.id && card != targetBaseCard)
					.ToList();

				CardCollapsed cardCollapsed = targetBaseCard.getCollapsedCard();
				removeCardsFromStack(subjectCards);
				cardCollapsed.addCardsToStack(subjectCards);
			}
			else if (targetBaseCard.interactableType == CoreInteractableType.Cards)
			{
				List<BaseCard> subjectCards = cards
					.Where((card) => card.interactableType == CoreInteractableType.Cards && card.id == targetBaseCard.id)
					.ToList();

				CardCollapsed cardCollapsed = CardHandler.current.createCardCollapsed(targetBaseCard.id);
				cardCollapsed.transform.position = targetBaseCard.transform.position;
				removeCardsFromStack(subjectCards);
				cardCollapsed.addCardsToStack(subjectCards);
				handleAddCardsToStack(new List<BaseCard>() { cardCollapsed });
			}
			else
			{
				Debug.LogError("collapseCardStack should not happen here");
				break;
			}
			break;
		}
	}

	public List<int> getAllCardIds()
	{
		return getIdsOfBaseCards(cards);
	}

	public List<BaseCard> getBaseCardsFromIds(List<int> cardIds)
	{
		Dictionary<int, int> indexedCardIds = CardHelpers.indexCardIds(cardIds);
		List<BaseCard> returnCards = new List<BaseCard>();

		foreach (BaseCard singleCard in cards)
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

	public List<Card> getCardsFromIds(List<int> cardIds)
	{
		Dictionary<int, int> indexedCardIds = CardHelpers.indexCardIds(cardIds);
		List<Card> returnCards = new List<Card>();
		List<Card> realCards = CardHelpers.baseCardsToCards(cards);

		foreach (Card singleCard in realCards)
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

	public List<int> getAllCardIdsOfUnityModules()
	{
		List<BaseCard> unityBaseCards = cards
			.Where(
				(singleCard) =>
					CardDictionary.globalCardDictionary[singleCard.id].module != null
					&& CardDictionary.globalCardDictionary[singleCard.id].module.unityCount != 0
			)
			.ToList();

		List<int> modules = getIdsOfBaseCards(unityBaseCards);

		return modules;
	}

	public List<int> getAllActiveCardIds()
	{
		List<BaseCard> activeBaseCards = cards
			.Where(
				(card) =>
				{
					return !card.isInteractiveDisabled;
				}
			)
			.ToList();

		return getIdsOfBaseCards(activeBaseCards);
	}

	public List<BaseCard> getActiveBaseCards()
	{
		List<BaseCard> returnCards = new List<BaseCard>();
		foreach (BaseCard singleCard in cards)
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
		List<BaseCard> activeBaseCards = cards
			.Where(
				(singleCard) =>
				{
					return CardHelpers.isNonValueTypeCard(CardDictionary.globalCardDictionary[singleCard.id].type)
						&& singleCard.isInteractiveDisabled == false;
				}
			)
			.ToList();

		return getIdsOfBaseCards(activeBaseCards);
	}

	public List<int> getTypeActiveCards(CardsTypes cardType)
	{
		List<BaseCard> activeBaseCards = cards
			.Where(
				(singleCard) =>
				{
					return CardDictionary.globalCardDictionary[singleCard.id].type == cardType && singleCard.isInteractiveDisabled == false;
				}
			)
			.ToList();
		return getIdsOfBaseCards(activeBaseCards);
	}

	public List<int> getMagnetizedCards()
	{
		List<BaseCard> activeBaseCards = cards
			.Where(
				(singleCard) =>
				{
					return CardDictionary.globalCardDictionary[singleCard.id].module != null
						&& CardDictionary.globalCardDictionary[singleCard.id].module.isMagnetizedCardIds != null;
				}
			)
			.ToList();
		List<int> magnetizeCardIds = getIdsOfBaseCards(activeBaseCards);

		List<int> returnVar = magnetizeCardIds.Aggregate(
			new List<int>(),
			(total, next) =>
			{
				// maybe wrong :p
				total.AddRange(CardDictionary.globalCardDictionary[next].module.isMagnetizedCardIds);
				return total;
			}
		);

		return returnVar;
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
		if (cards.Count > 0 && cards[0] != null)
		{
			return cards[0].transform.position;
		}
		return Vector3.zero;
	}

	private List<int> getIdsOfBaseCards(List<BaseCard> baseCards)
	{
		List<int> ids = new List<int>();
		foreach (BaseCard singleCard in baseCards)
		{
			if (singleCard.interactableType == CoreInteractableType.CollapsedCards)
			{
				CardCollapsed cardCollapsed = singleCard.getCollapsedCard();
				ids.AddRange(cardCollapsed.getCards().Select((card) => card.id));
				continue;
			}
			ids.Add(singleCard.id);
		}
		return ids;
	}

	private BaseCard findCollapsableBaseCard()
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
					.Where((card) => card.isCardType() && singleCard != card && card.id == singleCard.id)
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

	public CardStackType getCardHolderType()
	{
		return cardStackType;
	}

	public BaseNode getNode()
	{
		return connectedNode;
	}

	public List<BaseCard> getCards()
	{
		return cards;
	}

	public void removeCardsFromStack(List<BaseCard> removingCards)
	{
		bool changed = false;
		bool isRootCardRemoved = false;
		Vector3 rootCardPosition = Vector3.zero;

		foreach (BaseCard singleCard in removingCards)
		{
			if (cards.Count > 0 && singleCard.GetInstanceID() == cards[0].GetInstanceID())
			{
				isRootCardRemoved = true;
				rootCardPosition = singleCard.transform.position;
			}

			changed = true;
			cards.Remove(singleCard);
			DOTween.Kill(singleCard.id);

			singleCard.gameObject.transform.SetParent(null);
			singleCard.gameObject.SetActive(true);
			singleCard.joinedStack = null;
		}
		if (changed)
		{
			Vector3 rootPosition = isRootCardRemoved ? rootCardPosition : getRootPosition();
			alignCards(rootPosition);
		}
	}

	public void addCardsToStack(List<BaseCard> addingCards)
	{
		handleAddCardsToStack(addingCards);
		collapseCardStack();
	}

	public void dimCard() { }

	public void nonDimCard() { }

	private void handleAddCardsToStack(List<BaseCard> addingCards)
	{
		cards.AddRange(addingCards);
		foreach (BaseCard singleCard in addingCards)
		{
			singleCard.attachToCardHolder(this);
			if (cardStackType == CardStackType.Nodes || cardStackType == CardStackType.CollapsedCards)
			{
				singleCard.gameObject.transform.SetParent(connectedNode.nodePlaneManager.gameObject.transform);
			}
		}
		alignCards(getRootPosition());
	}

	//---------------- END CardHolder------------
}
