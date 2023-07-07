using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class InteractableManager : MonoBehaviour
{
	public List<Card> cards;
	public List<Node> nodes;

	void Start() { }

	public void registerCard(Card newCard)
	{
		cards.Add(newCard);
	}

	public void registerNode(Node nodeCard)
	{
		nodes.Add(nodeCard);
	}
}
