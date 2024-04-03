using UnityEngine;
using Core;
using System.Collections.Generic;

public class UI_TopRightHandler : MonoBehaviour
{
	private float ideaTabHeight = 30;
	private float ideaTabYPadding = 5;

	public GameObject contentContainer;
	public static Dictionary<int, UI_IdeaSingleBarContainer> currentShowingIdeaTabs = new Dictionary<int, UI_IdeaSingleBarContainer>();
	public GameObject prefabIdeaBar;

	public GameObject containerDimObject;
	public SO_Interactable so_Interactable;
	public SO_Highlight so_Highlight;

	private void Awake()
	{
		so_Interactable.addActionToCardEvent(addSingleIdeaBar);
		so_Highlight.triggerAction.Add(triggerDimRefresh);
	}

	private void Start()
	{
		handleStart();
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
		containerDimObject.SetActive(true);

		foreach (KeyValuePair<int, UI_IdeaSingleBarContainer> entry in currentShowingIdeaTabs)
		{
			if (entry.Value.cardId != so_Highlight.ideaId)
			{
				entry.Value.handleDim();
			}
			else
			{
				entry.Value.handleNonDim();
			}
		}
	}

	private void disableDim()
	{
		containerDimObject.SetActive(false);
		foreach (KeyValuePair<int, UI_IdeaSingleBarContainer> entry in currentShowingIdeaTabs)
		{
			entry.Value.handleNonDim();
		}
	}

	public void handleStart()
	{
		foreach (Card card in so_Interactable.cards)
		{
			addSingleIdeaBar(card.id);
		}
	}

	public void addSingleIdeaBar(int cardId)
	{
		if (CardDictionary.globalCardDictionary[cardId].type != CardsTypes.Idea || currentShowingIdeaTabs.ContainsKey(cardId))
		{
			return;
		}
		GameObject ideaBarGameObject = Instantiate(prefabIdeaBar, contentContainer.transform);
		UI_IdeaSingleBarContainer ideaBar = ideaBarGameObject.GetComponent<UI_IdeaSingleBarContainer>();
		ideaBar.setCardId(cardId);

		handleNewIdeaBarPosition(ideaBar);
		currentShowingIdeaTabs.Add(cardId, ideaBar);
	}

	private void handleNewIdeaBarPosition(UI_IdeaSingleBarContainer newBar)
	{
		int currentCount = currentShowingIdeaTabs.Count;

		float topPadding = (ideaTabYPadding * (currentCount + 1)) + (ideaTabHeight * currentCount) + (ideaTabHeight / 2);

		RectTransform ideaBarRectTransform = newBar.gameObject.GetComponent<RectTransform>();
		ideaBarRectTransform.localPosition = new Vector3(
			ideaBarRectTransform.localPosition.x,
			-topPadding,
			ideaBarRectTransform.localPosition.z
		);
	}
}
