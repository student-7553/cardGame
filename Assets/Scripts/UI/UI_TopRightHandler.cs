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
	public SO_Interactable so_Interactable;

	void Start()
	{
		handleStart();
		so_Interactable.addActionToCardEvent(addSingleIdeaBar);
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
