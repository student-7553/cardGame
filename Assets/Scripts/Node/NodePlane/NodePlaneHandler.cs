using UnityEngine;
using TMPro;
using Core;

public class NodePlaneHandler : MonoBehaviour, IStackable
{
	private BaseNode connectedNode;
	public TextMeshPro titleTextMesh;
	public InteractableManagerScriptableObject interactableManagerScriptableObject;

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

		interactableManagerScriptableObject.setActiveNodePlane(this);
	}

	public void stackOnThis(Card draggingCard, Node prevNode)
	{
		connectedNode.stackOnThis(draggingCard, prevNode);
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
		titleTextMesh.text = "";
		if (!connectedNode.isActive)
		{
			titleTextMesh.text = "[No Food] " + titleTextMesh.text;
		}
	}
}
