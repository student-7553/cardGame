using UnityEngine;
using TMPro;

public class NodeTextHandler
{
	// -------------------- Unity Component -------------------------
	private TextMeshPro titleTextMesh;

	private TextMeshPro nodeProcessCountdown;
	private TextMeshPro hungerCountPerIntervel;

	private Node connectedNode;

	public NodeTextHandler(Node _node)
	{
		connectedNode = _node;
		Component[] textMeshes = _node.gameObject.GetComponentsInChildren(typeof(TextMeshPro));
		titleTextMesh = textMeshes[0] as TextMeshPro;
		nodeProcessCountdown = textMeshes[1] as TextMeshPro;
		hungerCountPerIntervel = textMeshes[2] as TextMeshPro;
	}

	private int getFontSize(string title)
	{
		if (title.Length > 12)
		{
			return 13;
		}
		return 18;
	}

	public void reflectToScreen()
	{
		if (!CardDictionary.globalCardDictionary.ContainsKey(connectedNode.id))
		{
			return;
		}

		string cardTitle = CardDictionary.globalCardDictionary[connectedNode.id].name;

		int fontSize = getFontSize(cardTitle);
		titleTextMesh.fontSize = fontSize;
		titleTextMesh.text = cardTitle;

		nodeProcessCountdown.text = connectedNode.nodeProcess.isProccessing
			? $"{Mathf.RoundToInt(connectedNode.nodeProcess.proccessingLeft)}"
			: "";

		hungerCountPerIntervel.text = $"{connectedNode.nodeStats.currentNodeStats.currentFoodCheck}";
	}
}
