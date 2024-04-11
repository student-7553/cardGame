using UnityEngine;
using System.Linq;

public class DimBoardHandler : MonoBehaviour
{
	public GameObject dimBoardGameObject;
	public SO_Highlight so_Highlight;
	public SO_Interactable so_Interactable;

	private void Awake()
	{
		so_Highlight.triggerAction.Add(triggerDimRefresh);
	}

	private void OnDestroy()
	{
		so_Highlight.triggerAction.Remove(triggerDimRefresh);
	}

	private void triggerDimRefresh()
	{
		if (so_Highlight.isHighlightEnabled)
		{
			enableDim();
		}
		else
		{
			disableDim();
		}
	}

	private void enableDim()
	{
		dimBoardGameObject.SetActive(true);

		foreach (Card card in so_Interactable.cards)
		{
			if (so_Highlight.cardIds.Any((cardId) => cardId == card.id))
			{
				continue;
			}
			card.dimCard();
		}

		foreach (Node node in so_Interactable.nodes)
		{
			if (so_Highlight.cardIds.Any((cardId) => cardId == node.id))
			{
				node.nonDimCard();
				continue;
			}
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
