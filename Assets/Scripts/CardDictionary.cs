using System.Collections.Generic;
using UnityEngine;
using Core;
using System.Linq;

public static class CardDictionary
{
	public static Dictionary<int, CardObject> globalCardDictionary = new Dictionary<int, CardObject>();
	public static Dictionary<int, List<RawProcessObject>> globalProcessDictionary = new Dictionary<int, List<RawProcessObject>>();

	public static void init(Descriptions descriptions)
	{
		globalCardDictionary = getNewCardDictionary(descriptions);
		globalProcessDictionary = getNewProcessDictionary();
	}

	private static Dictionary<int, CardObject> getNewCardDictionary(Descriptions descriptions)
	{
		Dictionary<int, CardObject> newCardDictionary = new Dictionary<int, CardObject>();
		var jsonTextFile = Resources.Load<TextAsset>("Dictionary/card");

		RawCardObject[] listOfCards = JsonHelper.FromJson<RawCardObject>(jsonTextFile.text);
		List<RawCardObject> reversedListOfCards = listOfCards.Reverse().ToList();

		foreach (RawCardObject singleCard in reversedListOfCards)
		{
			string cardDescription = descriptions.foodCardIds.Find((single) => single.cardIds.Contains(singleCard.id)).description;
			CardObject newObject = processRawCardObject(singleCard, cardDescription);
			newCardDictionary.Add(singleCard.id, newObject);
		}
		return newCardDictionary;
	}

	private static Dictionary<int, List<RawProcessObject>> getNewProcessDictionary()
	{
		Dictionary<int, List<RawProcessObject>> newProcessDictionary = new Dictionary<int, List<RawProcessObject>>();
		var jsonTextFile = Resources.Load<TextAsset>("Dictionary/process");
		RawProcessObject[] listOfProcess = JsonHelper.FromJson<RawProcessObject>(jsonTextFile.text);

		foreach (RawProcessObject singleProcess in listOfProcess.Reverse())
		{
			if (newProcessDictionary.ContainsKey(singleProcess.baseRequiredId))
			{
				newProcessDictionary[singleProcess.baseRequiredId].Add(singleProcess);
			}
			else
			{
				List<RawProcessObject> newProcesses = new List<RawProcessObject>() { singleProcess };
				newProcessDictionary.Add(singleProcess.baseRequiredId, newProcesses);
			}
		}
		return newProcessDictionary;
	}

	private static CardObject processRawCardObject(RawCardObject rawCardObject, string description)
	{
		CardObject newEntry = new CardObject
		{
			id = rawCardObject.id,
			name = rawCardObject.name,
			resourceInventoryCount = rawCardObject.resourceInventoryCount,
			infraInventoryCount = rawCardObject.infraInventoryCount,
			isSellable = rawCardObject.isSellable,
			sellingPrice = rawCardObject.sellingPrice,
			typeValue = rawCardObject.typeValue,
			nodeTransferTimeCost = rawCardObject.nodeTransferTimeCost,
			foodCost = rawCardObject.foodCost,
			description = description,
		};

		switch (rawCardObject.type)
		{
			case "Resource":
				newEntry.type = CardsTypes.Resource;
				break;
			case "Infrastructure":
				newEntry.type = CardsTypes.Infrastructure;
				break;
			case "CombatUnit":
				newEntry.type = CardsTypes.CombatUnit;
				break;
			case "Enemy":
				newEntry.type = CardsTypes.Enemy;
				break;
			case "Gold":
				newEntry.type = CardsTypes.Gold;
				break;
			case "Electricity":
				newEntry.type = CardsTypes.Electricity;
				break;
			case "Will":
				newEntry.type = CardsTypes.Will;
				break;
			case "Food":
				newEntry.type = CardsTypes.Food;
				break;
			case "Module":
				newEntry.type = CardsTypes.Module;
				break;
			case "Idea":
				newEntry.type = CardsTypes.Idea;
				break;
			case "Node":
				newEntry.type = CardsTypes.Node;
				break;
			default:
				newEntry.type = CardsTypes.Resource;
				break;
		}

		if (newEntry.type == CardsTypes.Module)
		{
			newEntry.module = rawCardObject.module;
		}
		return newEntry;
	}

	public static List<CardObject> getAllCardTypeCards(CardsTypes cardType)
	{
		List<CardObject> cardIds = new List<CardObject>();
		foreach (int singleKey in globalCardDictionary.Keys)
		{
			if (globalCardDictionary[singleKey].type == cardType)
			{
				cardIds.Add(globalCardDictionary[singleKey]);
			}
		}
		return cardIds;
	}
}
