using System.Collections.Generic;
using UnityEngine;
using Core;
using Helpers;

public class CardStack
{
	private static float stackDistance = 5;
	private static float distancePerCards = 0.01f;

	public Node connectedNode;

	public CardStackType cardStackType;

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

	public CardStack(Node spawningNode)
	{
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
		Card rootCard = this.getRootCard();
		rootCard.transform.position = new Vector3(originPoint.x, originPoint.y, this.getPositionZ());

		// we are not looping through first card because it's the origin point
		for (int i = 1; i < cards.Count; i++)
		{
			Card cardInSubject = cards[i];
			Vector3 newPostionForCardInSubject = new Vector3(
				rootCard.transform.position.x,
				rootCard.transform.position.y - (stackDistance * i),
				rootCard.transform.position.z - (i * distancePerCards)
			);
			cardInSubject.transform.position = newPostionForCardInSubject;
			cardInSubject.computeCorners();
		}
	}

	public void alignCards()
	{
		if (cards.Count == 0)
		{
			return;
		}

		if (cardStackType == CardStackType.Nodes)
		{
			Vector3 rootPosition = new Vector3(
				connectedNode.nodePlaneManager.gameObject.transform.position.x,
				connectedNode.nodePlaneManager.gameObject.transform.position.y,
				this.getPositionZ()
			);
			this.alignCards(rootPosition);
		}
		else
		{
			Card rootCard = this.getRootCard();
			this.alignCards(rootCard.transform.position);
		}
	}

	public void changeActiveStateOfAllCards(bool isActive)
	{
		foreach (Card singleCard in cards)
		{
			if (singleCard != null && singleCard.gameObject != null)
			{
				singleCard.gameObject.SetActive(isActive);
			}
		}
	}

	public void removeCardsFromStack(List<Card> removingCards)
	{
		foreach (Card singleCard in removingCards)
		{
			cards.Remove(singleCard);
			singleCard.removeFromCardStack();
		}
		this.alignCards();
	}

	public void addCardToStack(List<Card> addingCards)
	{
		cards.AddRange(addingCards);
		this.alignCards();
		foreach (Card singleCard in addingCards)
		{
			singleCard.addToCardStack(this);
		}

		if (cardStackType == CardStackType.Nodes && connectedNode != null)
		{
			if (connectedNode.nodePlaneManager.gameObject.activeSelf == false)
			{
				changeActiveStateOfAllCards(false);
			}
		}
	}

	public void addCardToStack(Card addingCard)
	{
		cards.Add(addingCard);
		this.alignCards();

		addingCard.addToCardStack(this);

		if (cardStackType == CardStackType.Nodes && connectedNode != null)
		{
			if (connectedNode.nodePlaneManager.gameObject.activeSelf == false)
			{
				changeActiveStateOfAllCards(false);
			}
		}
	}

	private void logCards()
	{
		foreach (Card singleCard in cards)
		{
			Debug.Log(singleCard);
		}
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

	public List<int> getNonTypeActiveCardIds()
	{
		List<int> ids = new List<int>();
		foreach (Card singleCard in cards)
		{
			if (
				CardDictionary.globalCardDictionary[singleCard.id].type != CardsTypes.Gold
				&& CardDictionary.globalCardDictionary[singleCard.id].type != CardsTypes.Electricity
				&& CardDictionary.globalCardDictionary[singleCard.id].type != CardsTypes.Food
				&& singleCard.isInteractiveDisabled == false
			)
			{
				ids.Add(singleCard.id);
			}
		}
		return ids;
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
