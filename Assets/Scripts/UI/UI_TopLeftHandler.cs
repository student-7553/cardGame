using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
	}
}
