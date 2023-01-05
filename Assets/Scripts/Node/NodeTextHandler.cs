using UnityEngine;

using TMPro;

public class NodeTextHandler : MonoBehaviour
{
	// -------------------- Unity Component -------------------------
	private TextMeshPro titleTextMesh;
	private TextMeshPro availableInventoryTextMesh;
	private TextMeshPro processTimerTextMesh;
	private Node connectedNode;

	public void Awake()
	{
		Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));
		titleTextMesh = textMeshes[0] as TextMeshPro;
		availableInventoryTextMesh = textMeshes[1] as TextMeshPro;
		processTimerTextMesh = textMeshes[2] as TextMeshPro;
		connectedNode = gameObject.GetComponent(typeof(Node)) as Node;
	}

	private void FixedUpdate()
	{
		reflectToScreen();
	}

	private void reflectToScreen()
	{
		availableInventoryTextMesh.text =
			$"{connectedNode.nodeStats.currentNodeStats.resourceInventoryUsed}/{connectedNode.nodeStats.currentNodeStats.resourceInventoryLimit}";

		if (connectedNode.nodeProcess.isProccessing)
		{
			processTimerTextMesh.text = $"{connectedNode.nodeProcess.proccessingLeft}";
		}
		else
		{
			processTimerTextMesh.text = "";
		}

		if (!connectedNode.isActive)
		{
			processTimerTextMesh.text = "[Inactive] " + processTimerTextMesh.text;
		}
	}
}
