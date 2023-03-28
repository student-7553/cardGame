using UnityEngine;
using TMPro;

public class NodePlaneHandler : MonoBehaviour
{
	private BaseNode connectedNode;
	public TextMeshPro titleTextMesh;

	private void Awake()
	{
		Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));
		titleTextMesh = textMeshes[0] as TextMeshPro;
	}

	public void init(BaseNode parentNode)
	{
		connectedNode = parentNode;
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

		connectedNode.stackOnThis(card, prevNode);
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
		// if (connectedNode.nodeProcess.isProccessing)
		// {
		// 	//
		// 	titleTextMesh.text = $"{Mathf.RoundToInt(connectedNode.nodeProcess.proccessingLeft)}";
		// }
		// else
		// {
		// 	titleTextMesh.text = "";
		// }

		if (!connectedNode.isActive)
		{
			titleTextMesh.text = "[No Food] " + titleTextMesh.text;
		}
	}
}
