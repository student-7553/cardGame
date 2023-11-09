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
			List<int> magnetizedCards = node.processCardStack.getMagnetizedCards();
			if (magnetizedCards.Count == 0)
			{
				continue;
			}
			Card targetMagnetCard = getTargetMagnetCard(node.transform.position, magnetizedCards, staticVariables.magnetizeMaxRange);
			if (targetMagnetCard == null)
			{
				continue;
			}
			handleMagnetizeCard(node, targetMagnetCard);
		}
	}

	private void handleMagnetizeCard(Node node, Card targetMagnetCard)
	{
		if (targetMagnetCard.joinedStack != null)
		{
			targetMagnetCard.joinedStack.removeCardsFromStack(new List<BaseCard>() { targetMagnetCard });
		}

		Vector3 targetNodePosition = node.transform.position;
		targetMagnetCard.disableInteractiveForATime(magnetizeMoveTime, CardDisableType.AutoMoving);
		targetMagnetCard.gameObject.transform
			.DOMove(targetNodePosition, magnetizeMoveTime)
			.OnKill(() =>
			{
				targetMagnetCard.isInteractiveDisabled = false;
				node.stackOnThis(targetMagnetCard, null);
			});
	}

	private Card getTargetMagnetCard(Vector3 nodePosition, List<int> magnetizedCards, float maxRange)
	{
		List<Card> availableCards = getAvailableCards();
		List<Card> possibleMagnetizingCards = new List<Card>();

		foreach (int magnetizedCard in magnetizedCards)
		{
			Card targetCard = availableCards.Find(
				card => card != null && card.id == magnetizedCard && isCardInRange(nodePosition, card.transform.position, maxRange)
			);

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

	private bool isCardInRange(Vector2 nodePosition, Vector2 cardPosition, float maxRange)
	{
		return Vector2.Distance(nodePosition, cardPosition) <= maxRange;
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
			if (globalCard.isStacked() && globalCard.joinedStack.getCardHolderType() == CardStackType.Nodes)
			{
				continue;
			}
			availableCards.Add(globalCard);
		}
		return availableCards;
	}
}
