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
		connectedNode.activeStack.changeActiveStateOfAllCards(false);
		GameManager.current.boardPlaneHandler.clearActiveNodePlane();
	}

	private void OnEnable()
	{
		connectedNode.activeStack.changeActiveStateOfAllCards(true);
		GameManager.current.boardPlaneHandler.setActiveNodePlane(this);
		this.alignCardStackToPlane();
	}

	public void stackOnThis(List<Card> draggingCards)
	{
		connectedNode.stackOnThis(draggingCards);
	}

	private void alignCardStackToPlane()
	{
		connectedNode.activeStack.moveRootCardToPosition(gameObject.transform.position.x, gameObject.transform.position.y);
	}
}
