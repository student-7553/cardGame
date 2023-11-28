using System.Collections.Generic;
using UnityEngine;
using Core;
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

	public RawProcessObject getTracedProcess()
	{
		foreach (KeyValuePair<int, List<RawProcessObject>> entry in CardDictionary.globalProcessDictionary)
		{
			foreach (RawProcessObject process in entry.Value)
			{
				//
				if (process.ideaCard == currentCardId)
				{
					return process;
				}
			}
		}
		return null;
	}

	public void handleTextChange()
	{
		textFields[0].text = CardDictionary.globalCardDictionary[currentCardId].name;
		textFields[1].text = $"{CardDictionary.globalCardDictionary[currentCardId].type}";

		if (CardDictionary.globalCardDictionary[currentCardId].type == Core.CardsTypes.Idea)
		{
			RawProcessObject process = getTracedProcess();
			if (process != null)
			{
				// Todo this field
				// textFields[2].text =
				// 	$"{CardDictionary.globalCardDictionary[currentCardId].resourceInventoryCount}/{CardDictionary.globalCardDictionary[currentCardId].infraInventoryCount}";

				// textFields[3].text = $"{CardDictionary.globalCardDictionary[currentCardId].sellingPrice}";
			}
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
