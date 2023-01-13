using UnityEngine;
using TMPro;

public class NodePlaneHandler : MonoBehaviour
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
	}

	public void cardIsStacking(object[] package)
	{
		Card card = package[0] as Card;
		string direction = package[1] as string;
		Node prevNode = package[2] as Node;

		bool dropOnLeftSide = direction == "left" ? true : false;

		if (!connectedNode.isMarket() && prevNode == connectedNode && dropOnLeftSide)
		{
			connectedNode.storageCardStack.addCardToStack(card);
		}
		else
		{
			connectedNode.stackOnThis(card, prevNode);
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
