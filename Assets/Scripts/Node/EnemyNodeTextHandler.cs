using UnityEngine;
using TMPro;

public class EnemyNodeTextHandler
{
	// -------------------- Unity Component -------------------------
	private TextMeshPro titleTextMesh;
	private TextMeshPro availableInventoryTextMesh;
	private TextMeshPro processTimerTextMesh;
	private EnemyNode connectedNode;

	public EnemyNodeTextHandler(EnemyNode _node)
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

		availableInventoryTextMesh.text = $"{connectedNode.powerValue}";

		processTimerTextMesh.text = $"{Mathf.RoundToInt(connectedNode.proccessingLeft)}";
	}
}
