using System.Collections.Generic;
using UnityEngine;
using Core;
using TMPro;
using System.Linq;

public class UI_TopLeftHandler : MonoBehaviour
{
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

	private string getRequiredStringValueFromProcess(RawProcessObject process)
	{
		List<string> addingString = new List<string>();

		if (process.requiredGold != 0)
		{
			addingString.Add($"{process.requiredGold} gold");
		}
		if (process.requiredWill != 0)
		{
			addingString.Add($"{process.requiredWill} will");
		}
		List<int> requiredIds = new List<int>(process.requiredIds) { process.baseRequiredId };

		var groupedIds = requiredIds.GroupBy(id => id, (Key, ids) => new { Key, Count = ids.Count() });
		foreach (var ids in groupedIds)
		{
			addingString.Add($"{ids.Count} {CardDictionary.globalCardDictionary[ids.Key].name}");
		}
		string text2 = "Required: " + string.Join(",", addingString);

		return text2;
	}

	public void handleTextChange()
	{
		textFields[0].text = $"{CardDictionary.globalCardDictionary[currentCardId].name}";
		textFields[1].text = $"{CardDictionary.globalCardDictionary[currentCardId].type}";

		if (CardDictionary.globalCardDictionary[currentCardId].type == Core.CardsTypes.Idea)
		{
			RawProcessObject process = getTracedProcess();
			if (process != null)
			{
				string text2 = getRequiredStringValueFromProcess(process);
				textFields[2].text = text2;
				textFields[3].text = $"{process.time} sec";
			}
		}
		else
		{
			// Todo add another textField here
			textFields[2].text =
				$"{CardDictionary.globalCardDictionary[currentCardId].resourceInventoryCount}/{CardDictionary.globalCardDictionary[currentCardId].infraInventoryCount}";

			if (CardDictionary.globalCardDictionary[currentCardId].isSellable)
			{
				textFields[3].text = $"{CardDictionary.globalCardDictionary[currentCardId].sellingPrice}$";
			}
			else
			{
				textFields[3].text = "";
			}
		}
	}
}
