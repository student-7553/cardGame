using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using DG.Tweening;

public class MagneticModuleManager : MonoBehaviour
{
	public InteractableManagerScriptableObject interactableManagerScriptableObject;
	public StaticVariables staticVariables;
	private float selfIntervelTimer = 0;
	private float magnetizeMoveTime = 2;

	private void FixedUpdate()
	{
		selfIntervelTimer = selfIntervelTimer + Time.fixedDeltaTime;
		if (selfIntervelTimer > staticVariables.magnetizedIntervel)
		{
			selfIntervelTimer = 0;
			run();
		}
	}

	private void run()
	{
		foreach (Node node in interactableManagerScriptableObject.nodes)
		{
			if (node.nodeStats.currentNodeStats.resourceInventoryLimit - node.nodeStats.currentNodeStats.resourceInventoryUsed < 4)
			{
				continue;
			}
			List<Card> availableCards = getAvailableCards();
			List<int> magnetizedCards = getMagnetizedCards(node.processCardStack.cards);
			if (magnetizedCards.Count == 0)
			{
				continue;
			}

			Card targetMagnetCard = getTargetMagnetCard(magnetizedCards, availableCards);
			if (targetMagnetCard == null)
			{
				continue;
			}

			handleMagnetizeCard(node, targetMagnetCard);
		}
	}

	private void handleMagnetizeCard(Node node, Card targetMagnetCard)
	{
		targetMagnetCard.isStacked = false;
		targetMagnetCard.disableInteractiveForATime(magnetizeMoveTime, CardDisableType.AutoMoving);
		Vector3 targetNodePosition = node.transform.position;
		targetMagnetCard.gameObject.transform
			.DOMove(targetNodePosition, magnetizeMoveTime)
			.OnComplete(() =>
			{
				node.stackOnThis(targetMagnetCard, null);
			});
	}

	private Card getTargetMagnetCard(List<int> magnetizedCards, List<Card> availableCards)
	{
		List<Card> possibleMagnetizingCards = new List<Card>();

		foreach (int magnetizedCard in magnetizedCards)
		{
			Card targetCard = availableCards.Find(card => card.id == magnetizedCard);
			if (targetCard == null)
			{
				continue;
			}
			possibleMagnetizingCards.Add(targetCard);
		}

		if (possibleMagnetizingCards.Count == 0)
		{
			return null;
		}

		return possibleMagnetizingCards[Random.Range(0, possibleMagnetizingCards.Count - 1)];
	}

	private List<Card> getAvailableCards()
	{
		List<Card> availableCards = new List<Card>();
		foreach (Card globalCard in interactableManagerScriptableObject.cards)
		{
			if (globalCard.isInteractiveDisabled)
			{
				continue;
			}
			if (globalCard.isStacked && globalCard.joinedStack.cardStackType != CardStackType.Cards)
			{
				continue;
			}
			availableCards.Add(globalCard);
		}
		return availableCards;
	}

	private List<int> getMagnetizedCards(List<Card> cards)
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
}
