using UnityEngine;
using Core;
using System.Collections.Generic;
using TMPro;

public class NodePlaneHandler : MonoBehaviour, IStackable
{
	private Node connectedNode;
	public TextMeshPro textMesh;

	private void Awake()
	{
		connectedNode = GetComponentInParent(typeof(Node)) as Node;
		Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));
		textMesh = textMeshes[0] as TextMeshPro;
	}

	private void OnDisable()
	{
		connectedNode.cardStack.changeActiveStateOfAllCards(false);
	}

	private void OnEnable()
	{
		connectedNode.cardStack.changeActiveStateOfAllCards(true);
		GameManager.current.boardPlaneHandler.setActiveNodePlane(this);
		connectedNode.cardStack.alignCards();
	}

	public void stackOnThis(Card draggingCard, Node prevNode)
	{
		connectedNode.stackOnThis(draggingCard, prevNode);
	}
}
