using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Interactable", menuName = "ScriptableObjects/SO_Interactable", order = 1)]
public class SO_Interactable : ScriptableObject
{
	public List<Card> cards = new List<Card>();
	public List<Node> nodes = new List<Node>();

	public NodePlaneHandler currentNodePlaneHandler;

	public List<Action<int>> newCardAction = new List<Action<int>>();

	public void registerCard(Card newCard)
	{
		cards.Add(newCard);
		//Todo call the newCardActionss
	}

	public void clearCards()
	{
		cards.Clear();
	}

	public void registerNode(Node nodeCard)
	{
		nodes.Add(nodeCard);
	}

	public void removeCard(Card newCard)
	{
		cards.Remove(newCard);
	}

	public void removeNode(Node nodeCard)
	{
		nodes.Remove(nodeCard);
	}

	public void addActionToCardEvent(Action<int> newAction)
	{
		newCardAction.Add(newAction);
	}

	public void setActiveNodePlane(NodePlaneHandler newNodePlaneHandler)
	{
		if (currentNodePlaneHandler != null && currentNodePlaneHandler != newNodePlaneHandler)
		{
			currentNodePlaneHandler.gameObject.SetActive(false);
		}

		currentNodePlaneHandler = newNodePlaneHandler;
	}
}
