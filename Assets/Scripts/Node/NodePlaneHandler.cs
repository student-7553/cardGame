using UnityEngine;
using Core;
using TMPro;

public class NodePlaneHandler : MonoBehaviour, IStackable
{
	private Node connectedNode;
	public TextMeshPro titleTextMesh;

	private void Awake()
	{
		Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));
		titleTextMesh = textMeshes[0] as TextMeshPro;
	}

	public void init(Node parentNode)
	{
		connectedNode = parentNode;
	}

	private void OnDisable()
	{
		if (connectedNode == null)
		{
			return;
		}
	}

	private void OnEnable()
	{
		if (connectedNode == null)
		{
			return;
		}

		GameManager.current.boardPlaneHandler.setActiveNodePlane(this);
		connectedNode.storageCardStack.alignCards();
	}

	public void stackOnThis(Card draggingCard, Node prevNode)
	{
		if (connectedNode == null)
		{
			return;
		}

		bool dropOnLeftSide = true;

		if (!connectedNode.isMarket() && prevNode == connectedNode && dropOnLeftSide)
		{
			connectedNode.addCardToProcessCardStack(draggingCard);
		}
		else
		{
			connectedNode.stackOnThis(draggingCard, prevNode);
		}
	}

	private void FixedUpdate()
	{
		if (connectedNode == null)
		{
			return;
		}
		reflectToScreen();
	}

	private void reflectToScreen()
	{
		if (connectedNode.nodeProcess.isProccessing)
		{
			titleTextMesh.text = $"{connectedNode.nodeProcess.proccessingLeft}";
		}
		else
		{
			titleTextMesh.text = "";
		}

		if (!connectedNode.isActive)
		{
			titleTextMesh.text = "[No Food] " + titleTextMesh.text;
		}
	}
}
