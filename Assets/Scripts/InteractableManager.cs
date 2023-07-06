using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class InteractableManager : MonoBehaviour
{
	public List<Card> cards;
	public List<Node> nodes;

	void Start()
	{
		// foreach (int cardId in pushedCardIds)
		// {
		// 	CardHandler.current.playerCardTracker.ensureCardIdTracked(cardId);
		// }
	}

	void registerCard(Card newCard)
	{
		cards.Add(newCard);
	}

	void registerNode(Node nodeCard)
	{
		nodes.Add(nodeCard);
	}
}
