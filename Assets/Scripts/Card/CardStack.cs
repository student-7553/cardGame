using System.Collections.Generic;
using UnityEngine;
using Core;
using Helpers;

public class NodeCardStack
{
	public Node connectedNode;
}

public class CardStack
{
	private static float stackDistance = 5;
	private static float zDistancePerCards = 0.01f;

	public Node connectedNode;

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

	public CardStack(Node spawningNode)
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
		Card rootCard = this.getRootCard();
		float paddingCounter = 0;
		foreach (Card singleCard in cards)
		{
			Vector3 newPostionForCardInSubject = new Vector3(originPoint.x, originPoint.y, this.getPositionZ());
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
		Vector3 originPoint = new Vector3();
		if (cardStackType == CardStackType.Nodes)
		{
			originPoint = new Vector3(
				connectedNode.nodePlaneManager.gameObject.transform.position.x,
				connectedNode.nodePlaneManager.gameObject.transform.position.y,
				this.getPositionZ()
			);
		}
		else
		{
			Card rootCard = this.getRootCard();
			originPoint = rootCard.transform.position;
		}
		originPoint = originPoint + originPointAdjustment;
		this.alignCards(originPoint);
	}

	public void removeCardsFromStack(List<Card> removingCards)
	{
		foreach (Card singleCard in removingCards)
		{
			cards.Remove(singleCard);
			singleCard.isStacked = false;
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

			if (cardStackType == CardStackType.Nodes)
			{
				singleCard.gameObject.transform.SetParent(connectedNode.nodePlaneManager.gameObject.transform);
			}
		}
	}

	public void addCardToStack(Card addingCard)
	{
		cards.Add(addingCard);
		this.alignCards();

		addingCard.addToCardStack(this);

		if (cardStackType == CardStackType.Nodes)
		{
			addingCard.gameObject.transform.SetParent(connectedNode.nodePlaneManager.gameObject.transform);
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
