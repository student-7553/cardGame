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
		GameManager.current.boardPlaneHandler.clearActiveNodePlane();
	}

	private void OnEnable()
	{
		connectedNode.cardStack.changeActiveStateOfAllCards(true);
		GameManager.current.boardPlaneHandler.setActiveNodePlane(this);
		this.alignCardStackToPlane();
	}

	public void stackOnThis(Card draggingCard)
	{
		connectedNode.stackOnThis(draggingCard);
	}

	private void alignCardStackToPlane()
	{
		// connectedNode.cardStack.moveRootCardToPosition(gameObject.transform.position.x, gameObject.transform.position.y);
	}
}
