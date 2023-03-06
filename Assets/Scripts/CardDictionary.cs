using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Core;
using System.Linq;

public static class CardDictionary
{
	public static Dictionary<int, CardObject> globalCardDictionary;
	public static Dictionary<int, List<RawProcessObject>> globalProcessDictionary;

	public static void init()
	{
		initCardDictionary();
		initProcessDictionary();
	}

	private static void initCardDictionary()
	{
		globalCardDictionary = new Dictionary<int, CardObject>();
		var jsonTextFile = Resources.Load<TextAsset>("Dictionary/card");
		RawCardObject[] listOfCards = JsonHelper.FromJson<RawCardObject>(jsonTextFile.text);
		foreach (RawCardObject singleCard in listOfCards)
		{
			CardObject newObject = processRawCardObject(singleCard);
			globalCardDictionary.Add(singleCard.id, newObject);
		}
	}

	private static void initProcessDictionary()
	{
		globalProcessDictionary = new Dictionary<int, List<RawProcessObject>>();
		var jsonTextFile = Resources.Load<TextAsset>("Dictionary/process");
		RawProcessObject[] listOfProcess = JsonHelper.FromJson<RawProcessObject>(jsonTextFile.text);

		foreach (RawProcessObject singleProcess in listOfProcess.Reverse())
		{
			if (globalProcessDictionary.ContainsKey(singleProcess.baseRequiredId))
			{
				globalProcessDictionary[singleProcess.baseRequiredId].Add(singleProcess);
			}
			else
			{
				List<RawProcessObject> newProcesses = new List<RawProcessObject>();
				newProcesses.Add(singleProcess);
				globalProcessDictionary.Add(singleProcess.baseRequiredId, newProcesses);
			}
		}
	}

	private static CardObject processRawCardObject(RawCardObject rawCardObject)
	{
		CardObject newEntry = new CardObject();
		newEntry.id = rawCardObject.id;
		newEntry.name = rawCardObject.name;
		newEntry.resourceInventoryCount = rawCardObject.resourceInventoryCount;
		newEntry.infraInventoryCount = rawCardObject.infraInventoryCount;
		newEntry.isSellable = rawCardObject.isSellable;
		newEntry.sellingPrice = rawCardObject.sellingPrice;
		newEntry.typeValue = rawCardObject.typeValue;
		newEntry.nodeTransferTimeCost = rawCardObject.nodeTransferTimeCost;
		newEntry.foodCost = rawCardObject.foodCost;

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
