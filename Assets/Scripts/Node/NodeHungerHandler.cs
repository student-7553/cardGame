using UnityEngine;
using Core;
using Helpers;
using System.Linq;
using System.Collections.Generic;

// using System.Collections;

public class NodeHungerHandler : MonoBehaviour
{
	private Node connectedNode;

	[System.NonSerialized]
	public float intervalTimer; // ********* Loop timer *********

	private bool isInit = false;

	public InteractableManagerScriptableObject interactableManagerScriptableObject;
	public StaticVariables staticVariables;

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
			this.handleHungerInterval();
		}
	}

	private void handleHungerInterval()
	{
		int foodMinus = connectedNode.nodeStats.currentNodeStats.currentFoodCheck;

		if (connectedNode.nodeStats.currentNodeStats.currentFood - foodMinus <= 0)
		{
			connectedNode.isActive = false;
		}
		else
		{
			StartCoroutine(connectedNode.nodeProcess.queUpTypeDeletion(CardsTypes.Food, foodMinus, 2f, null));
		}
	}

	// private List<Card> getCloseFoods(int foodValue)
	private void handleHunger(int foodValue)
	{
		List<Card> foodCards = new List<Card>();
		List<Card> allFoodCards = interactableManagerScriptableObject.cards
			.Where(
				(card) =>
				{
					return staticVariables.foodCardIds.Exists((foodCardId) => card.id == foodCardId);
				}
			)
			.ToList();

		List<int> allFoodCardIds = allFoodCards.Select((card) => card.id).ToList();

		int allFoodValue = allFoodCards.Aggregate(0, (total, card) => total + CardDictionary.globalCardDictionary[card.id].typeValue);
		if (allFoodValue < foodValue)
		{
			connectedNode.isActive = false;
			return;
		}

		List<Card> removingCards = new List<Card>();
		List<Card> creatingCards = new List<Card>();

		TypeAdjustingData foodAdjData = CardHelpers.handleTypeAdjusting(allFoodCardIds, CardsTypes.Food, foodValue);
		// foodAdjData.removingCardIds.for
		// data.addingCardIds.AddRange(goldData.addingCardIds);
		// data.removingCardIds.AddRange(goldData.removingCardIds);

		return;
	}
}
