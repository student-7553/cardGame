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

	public GameObject containerDimObject;
	public GameObject containerDim2Object;
	public GameObject containerDimDesriptionObject;

	private void Awake()
	{
		playerRuntime.registerActionToPlayerFocus(focusCardIdChanged);
		so_Highlight.triggerAction.Add(triggerDimRefresh);
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
		so_Highlight.triggerAction.Remove(triggerDimRefresh);
		playerRuntime.unRegisterAction(focusCardIdChanged);
	}

	private void triggerDimRefresh()
	{
		if (so_Highlight.isHighlightEnabled)
		{
			containerDimObject.SetActive(true);
			containerDimDesriptionObject.SetActive(true);
			if (!so_Highlight.topLeftHighlighted)
			{
				containerDim2Object.SetActive(true);
			}
			else
			{
				containerDim2Object.SetActive(false);
			}
		}
		else
		{
			containerDimObject.SetActive(false);
			containerDim2Object.SetActive(false);
			containerDimDesriptionObject.SetActive(false);
		}
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
			so_Highlight.cardIds = new int[] { };
			so_Highlight.ideaId = -1;
			so_Highlight.topLeftHighlighted = true;
			so_Highlight.highlightText =
				"By clicking on a card or its associated right sidebar, you can see detailed information about the card on the left side";

			so_Highlight.highlightMainText = "Specifically the cards required to make this card";
			so_Highlight.objectiveText = "";
			so_Highlight.triggerRefresh();
			StartCoroutine(stopDim());
		}
	}

	public IEnumerator stopDim()
	{
		yield return new WaitForSeconds(5);
		so_Highlight.isHighlightEnabled = false;
		so_Highlight.cardIds = new int[] { };
		so_Highlight.ideaId = -1;
		so_Highlight.topLeftHighlighted = false;
		so_Highlight.highlightText = "Lets create this card. Add the correct cards into \"Small Base\" and create it";
		so_Highlight.highlightMainText = "Create the \"Space dome\" card";
		so_Highlight.objectiveText = "Create the \"Space dome\" card";
		so_Highlight.triggerRefresh();
	}
}
