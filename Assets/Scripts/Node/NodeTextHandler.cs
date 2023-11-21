using UnityEngine;
using TMPro;

public class NodeTextHandler
{
	// -------------------- Unity Component -------------------------
	private TextMeshPro titleTextMesh;
	private TextMeshPro availableInventoryTextMesh;
	private TextMeshPro availableInfraTextMesh;
	private TextMeshPro nodeProcessCountdown;
	private TextMeshPro hungerCountPerIntervel;
	private TextMeshPro hungerCountdown;

	private Node connectedNode;

	public NodeTextHandler(Node _node)
	{
		connectedNode = _node;
		Component[] textMeshes = _node.gameObject.GetComponentsInChildren(typeof(TextMeshPro));
		titleTextMesh = textMeshes[0] as TextMeshPro;
		availableInventoryTextMesh = textMeshes[1] as TextMeshPro;
		availableInfraTextMesh = textMeshes[2] as TextMeshPro;
		nodeProcessCountdown = textMeshes[3] as TextMeshPro;
		hungerCountPerIntervel = textMeshes[4] as TextMeshPro;
		hungerCountdown = textMeshes[5] as TextMeshPro;
	}

	public void reflectToScreen()
	{
		if (!CardDictionary.globalCardDictionary.ContainsKey(connectedNode.id))
		{
			return;
		}

		titleTextMesh.text = CardDictionary.globalCardDictionary[connectedNode.id].name;

		availableInventoryTextMesh.text =
			$"{connectedNode.nodeStats.currentNodeStats.resourceInventoryUsed}/{connectedNode.nodeStats.currentNodeStats.resourceInventoryLimit}";

		availableInfraTextMesh.text =
			$"{connectedNode.nodeStats.currentNodeStats.infraInventoryUsed}/{connectedNode.nodeStats.currentNodeStats.infraInventoryLimit}";

		nodeProcessCountdown.text = connectedNode.nodeProcess.isProccessing
			? $"{Mathf.RoundToInt(connectedNode.nodeProcess.proccessingLeft)}"
			: "";

		hungerCountPerIntervel.text = $"{connectedNode.nodeStats.currentNodeStats.currentFoodCheck}";

		if (!connectedNode.isActive)
		{
			hungerCountdown.text = "[No food] ";
		}
		else
		{
			// Active Node
			hungerCountdown.text = $"{connectedNode.nodeHungerHandler.getHungerCountdown()}";
		}
	}
}
