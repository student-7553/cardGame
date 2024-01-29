using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class TopLeftEntry : MonoBehaviour
{
	public abstract void show(int currentCardIdk);

	public abstract void hide();
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

		if (CardDictionary.globalCardDictionary[currentCardId].type == Core.CardsTypes.Idea)
		{
			entry1.show(currentCardId);
			entry2.hide();
		}
		else
		{
			entry2.show(currentCardId);
			entry1.hide();
		}
	}
}
