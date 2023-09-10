using UnityEngine;
using System.Collections.Generic;

public class NodeMagnetizeCircle : MonoBehaviour
{
	private Node node;
	private SpriteRenderer spriteRenderer;
	public Vector3 scale;

	void Start()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
		transform.localScale = scale;
		spriteRenderer.enabled = false;
	}

	public void init(Node _node)
	{
		node = _node;
	}

	void FixedUpdate()
	{
		List<int> magnetizedCards = node.processCardStack.getMagnetizedCards();
		if (magnetizedCards.Count == 0)
		{
			spriteRenderer.enabled = false;
			return;
		}
		if (!spriteRenderer.enabled)
		{
			spriteRenderer.enabled = true;
		}
	}
}
