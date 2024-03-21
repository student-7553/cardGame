using UnityEngine;
using Core;
using System.Collections.Generic;
using Helpers;

public class DimBoardHandler : MonoBehaviour
{
	public GameObject dimBoardGameObject;
	public SO_Highlight so_Highlight;
	public SO_Interactable so_Interactable;

	private bool isHighlightEnabled = false;

	private void FixedUpdate()
	{
		if (so_Highlight.isHighlightEnabled != isHighlightEnabled)
		{
			if (so_Highlight.isHighlightEnabled)
			{
				enableDim();
				isHighlightEnabled = true;
			}
			else
			{
				disableDim();
				isHighlightEnabled = false;
			}
		}
	}

	private void enableDim()
	{
		dimBoardGameObject.SetActive(true);

		foreach (Card card in so_Interactable.cards)
		{
			card.dimCard();
		}

		foreach (Node node in so_Interactable.nodes)
		{
			node.dimCard();
		}
	}

	private void disableDim()
	{
		dimBoardGameObject.SetActive(false);

		foreach (Card card in so_Interactable.cards)
		{
			card.nonDimCard();
		}

		foreach (Node node in so_Interactable.nodes)
		{
			node.nonDimCard();
		}
	}
}
