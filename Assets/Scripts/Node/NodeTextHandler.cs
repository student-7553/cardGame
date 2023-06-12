using UnityEngine;
using TMPro;

public class NodeTextHandler
{
	// -------------------- Unity Component -------------------------
	private TextMeshPro titleTextMesh;
	private TextMeshPro availableInventoryTextMesh;
	private TextMeshPro processTimerTextMesh;
	private Node connectedNode;

	public NodeTextHandler(Node _node)
	{
		connectedNode = _node;
		Component[] textMeshes = _node.gameObject.GetComponentsInChildren(typeof(TextMeshPro));
		titleTextMesh = textMeshes[0] as TextMeshPro;
		availableInventoryTextMesh = textMeshes[1] as TextMeshPro;
		processTimerTextMesh = textMeshes[2] as TextMeshPro;
	}

	public void reflectToScreen()
	{
		if (titleTextMesh.text == "")
		{
			if (CardDictionary.globalCardDictionary.ContainsKey(connectedNode.id))
			{
				titleTextMesh.text = CardDictionary.globalCardDictionary[connectedNode.id].name;
			}
		}

		availableInventoryTextMesh.text =
			$"Inven:{connectedNode.nodeStats.currentNodeStats.resourceInventoryUsed}/{connectedNode.nodeStats.currentNodeStats.resourceInventoryLimit} Infra:{connectedNode.nodeStats.currentNodeStats.infraInventoryUsed}/{connectedNode.nodeStats.currentNodeStats.infraInventoryLimit}";

		if (connectedNode.nodeProcess.isProccessing)
		{
			processTimerTextMesh.text = $"{Mathf.RoundToInt(connectedNode.nodeProcess.proccessingLeft)}";
		}
		else
		{
			processTimerTextMesh.text = "";
		}

		if (!connectedNode.isActive)
		{
			processTimerTextMesh.text = "[No Food] " + processTimerTextMesh.text;
		}
		else
		{
			// Active Node
			processTimerTextMesh.text = processTimerTextMesh.text + $"[{connectedNode.nodeHungerHandler.getHungerCountdown()}]";
		}
	}
}
