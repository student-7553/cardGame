using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

[CreateAssetMenu(
	fileName = "InteractableManagerScriptableObject",
	menuName = "ScriptableObjects/InteractableManagerScriptableObject",
	order = 1
)]
public class InteractableManagerScriptableObject : ScriptableObject
{
	public List<Card> cards;
	public List<Node> nodes;
	public NodePlaneHandler currentNodePlaneHandler;

	public void registerCard(Card newCard)
	{
		cards.Add(newCard);
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

	public void setActiveNodePlane(NodePlaneHandler newNodePlaneHandler)
	{
		if (currentNodePlaneHandler != null && currentNodePlaneHandler != newNodePlaneHandler)
		{
			currentNodePlaneHandler.gameObject.SetActive(false);
		}

		currentNodePlaneHandler = newNodePlaneHandler;
	}

	public void OnDisable()
	{
		cards.Clear();
		nodes.Clear();
	}

	private void OnEnable()
	{
		cards.Clear();
		nodes.Clear();
	}
}
