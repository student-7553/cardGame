using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class TopLeftEntry : MonoBehaviour
{
	public virtual void Show(int currentCardIdk) { }

	public virtual void Hide() { }
}

public class UI_TopLeftHandler : MonoBehaviour
{
	private int currentCardId;

	public List<TextMeshProUGUI> textFields;

	public TopLeftEntry entry1;
	public TopLeftEntry entry2;

	public SO_PlayerRuntime playerRuntime;

	public SO_Highlight so_Highlight;

	private void Awake()
	{
		playerRuntime.registerActionToPlayerFocus(focusCardIdChanged);
	}

	private void Start()
	{
		if (playerRuntime.getPlayerFocusingCardId() != 0)
		{
			focusCardIdChanged();
		}
	}

	private void OnDestroy()
	{
		playerRuntime.unRegisterAction(focusCardIdChanged);
	}

	public void focusCardIdChanged()
	{
		currentCardId = playerRuntime.getPlayerFocusingCardId();
		handleTextChange();
	}

	public void handleTextChange()
	{
		textFields[0].text = $"{CardDictionary.globalCardDictionary[currentCardId].name}";
		textFields[1].text = $"{CardDictionary.globalCardDictionary[currentCardId].type}";
		textFields[2].text = $"{CardDictionary.globalCardDictionary[currentCardId].description}";

		if (CardDictionary.globalCardDictionary[currentCardId].type == Core.CardsTypes.Idea)
		{
			entry1?.Show(currentCardId);
			entry2?.Hide();
		}
		else
		{
			entry2?.Show(currentCardId);
			entry1?.Hide();
		}

		if (so_Highlight.isHighlightEnabled && currentCardId == 2001)
		{
			so_Highlight.isHighlightEnabled = true;
			so_Highlight.cardIds = new int[] { 3, 2, 12, 3000 };
			so_Highlight.ideaId = -1;
			so_Highlight.topLeftHighlighted = true;
			so_Highlight.highlightText =
				"By clicking on a [Idea] card or pressing Tab, you can see more information about new cards, Lets create the Space dome card. Add the correct cards into \"Small Base\" and create it";

			so_Highlight.highlightMainText = "Create the \"[Idea][Node] Space dome\" card";
			so_Highlight.objectiveText = "Create the \"[Idea][Node] Space dome\" card";

			so_Highlight.triggerRefresh();
		}
	}

	// public IEnumerator stopDim()
	// {
	// 	yield return new WaitForSeconds(10);
	// 	so_Highlight.isHighlightEnabled = false;
	// 	so_Highlight.cardIds = new int[] { };
	// 	so_Highlight.ideaId = -1;
	// 	so_Highlight.topLeftHighlighted = false;
	// 	so_Highlight.highlightText = "Lets create this card. Add the correct cards into \"Small Base\" and create it";
	// 	so_Highlight.highlightMainText = "Create the \"[Idea][Node] Space dome\" card";


	// 	so_Highlight.objectiveText = "Create the \"[Idea][Node] Space dome\" card";
	// 	so_Highlight.triggerRefresh();
	// }
}
