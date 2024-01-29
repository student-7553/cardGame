using System.Collections.Generic;
using UnityEngine;
using Core;
using TMPro;
using System.Linq;

public class UI_TopLeftEntry_1 : TopLeftEntry
{
	public TextMeshProUGUI textField;
	public TextMeshProUGUI textField2;

	public override void hide()
	{
		gameObject.SetActive(false);
	}

	public override void show(int currentCardId)
	{
		RawProcessObject process = getTracedProcess(currentCardId);
		if (process == null)
		{
			return;
		}

		string text2 = getRequiredStringValueFromProcess(process);
		textField.text = text2;
		textField2.text = $"{process.time}";

		gameObject.SetActive(true);
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
		string text2 = string.Join("\n", addingString);

		return text2;
	}

	public RawProcessObject getTracedProcess(int currentCardId)
	{
		foreach (KeyValuePair<int, List<RawProcessObject>> entry in CardDictionary.globalProcessDictionary)
		{
			foreach (RawProcessObject process in entry.Value)
			{
				if (process.ideaCard == currentCardId)
				{
					return process;
				}
			}
		}
		return null;
	}
}
