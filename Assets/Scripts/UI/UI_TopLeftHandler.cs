using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_TopLeftHandler : MonoBehaviour
{
	// Expect 4 Count
	// title
	// type
	// cost
	// 4rd
	private int currentCardId;
	public List<TextMeshProUGUI> textFields;
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
		// currentCardId
		textFields[0].text = CardDictionary.globalCardDictionary[currentCardId].name;
		textFields[1].text = $"{CardDictionary.globalCardDictionary[currentCardId].type}";

		if (CardDictionary.globalCardDictionary[currentCardId].type == Core.CardsTypes.Idea)
		{
			// Todo find the process from process..
		}
		else
		{
			textFields[2].text =
				$"{CardDictionary.globalCardDictionary[currentCardId].resourceInventoryCount}/{CardDictionary.globalCardDictionary[currentCardId].infraInventoryCount}";

			if (CardDictionary.globalCardDictionary[currentCardId].isSellable)
			{
				textFields[3].text = $"{CardDictionary.globalCardDictionary[currentCardId].sellingPrice}";
			}
			else
			{
				textFields[3].text = "";
			}
		}
	}
}
