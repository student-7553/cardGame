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

	private void Start()
	{
		if (playerRuntime.getPlayerFocusingCardId() != 0)
		{
			focusCardIdChanged();
		}
		playerRuntime.registerActionToPlayerFocus(focusCardIdChanged);
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
