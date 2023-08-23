using UnityEngine;
using Core;
using Helpers;
using System.Linq;
using System.Collections.Generic;

public class NodeHungerHandler : MonoBehaviour
{
	private Node connectedNode;

	[System.NonSerialized]
	public float intervalTimer; // ********* Loop timer *********

	private bool isInit = false;

	public void Awake()
	{
		intervalTimer = 0;
	}

	public void init(Node parentNode)
	{
		connectedNode = parentNode;
		if (parentNode.isMarket())
		{
			return;
		}

		isInit = true;
	}

	public int getHungerCountdown()
	{
		return Mathf.RoundToInt(connectedNode.nodeStats.baseNodeStat.hungerSetIntervalTimer - intervalTimer);
	}

	private void FixedUpdate()
	{
		if (!isInit || !connectedNode.isActive)
		{
			return;
		}

		intervalTimer = intervalTimer + Time.deltaTime;
		if (intervalTimer > connectedNode.nodeStats.baseNodeStat.hungerSetIntervalTimer)
		{
			intervalTimer = 0;
			handleHungerInterval();
		}
	}

	private void handleHungerInterval()
	{
		handleHunger(connectedNode.nodeStats.currentNodeStats.currentFoodCheck);
	}

	private void handleHunger(int foodValue)
	{
		List<Card> foodCards = new List<Card>();
		List<Card> allFoodCards = connectedNode.interactableManagerScriptableObject.cards
			.Where(
				(card) =>
				{
					return connectedNode.staticVariables.foodCardIds.Exists((foodCardId) => card.id == foodCardId);
				}
			)
			.ToList();

		int allFoodValue = allFoodCards.Aggregate(0, (total, card) => total + CardDictionary.globalCardDictionary[card.id].typeValue);
		if (allFoodValue < foodValue)
		{
			connectedNode.isActive = false;
			return;
		}

		List<Card> removingCards = new List<Card>();
		List<Card> creatingCards = new List<Card>();

		TypeAdjustingData foodAdjData = CardHelpers.handleTypeAdjusting(allFoodCards, CardsTypes.Food, foodValue);

		foreach (Card card in foodAdjData.removingCards)
		{
			card.destroyCard();
		}
		List<Card> addingCards = connectedNode.processCardStack.handleCreatingCards(foodAdjData.addingCardIds);
		connectedNode.ejectCards(addingCards);

		return;
	}
}
