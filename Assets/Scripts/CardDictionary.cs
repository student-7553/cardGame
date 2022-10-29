using System.Collections.Generic;
using UnityEngine;


public enum CardsTypes
{
    Resource,
    Gold,
    Electricity,
    Infrastructure,
    Module
}
[System.Serializable]
public class ModuleMinusInterval
{
    public int time;
    public int[] processIds;
}


[System.Serializable]
public class CardModuleObject
{
    public int resourceInventory;
    public float minusMovement;
    public ModuleMinusInterval minusInterval;
}


[System.Serializable]
public class RawCardObject
{
    public int id;
    public string name;
    public string type;
    public int resourceInventoryCount;
    public int infraInventoryCount;
    public bool isSellable;
    public int sellingPrice;
    public int typeValue;
    public float timeCost;
    public int foodCost;
    public CardModuleObject module;
}

public class CardObject
{
    public int id;
    public string name;
    public CardsTypes type;
    public int resourceInventoryCount;
    public int infraInventoryCount;
    public bool isSellable;
    public int sellingPrice;
    public int typeValue;
    public float timeCost;
    public int foodCost;
    public CardModuleObject module;
}

[System.Serializable]
public class RawProcessObject
{
    public int processId;
    public int baseCardId;
    public int[] requiredIds;
    public int[] processedIds;
    public int requiredGold;
    public int requiredElectricity;
    public float time;
}

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
        foreach (RawProcessObject singleProcess in listOfProcess)
        {
            if (globalProcessDictionary.ContainsKey(singleProcess.baseCardId))
            {
                globalProcessDictionary[singleProcess.baseCardId].Add(singleProcess);
            }
            else
            {
                List<RawProcessObject> newProcesses = new List<RawProcessObject>();
                newProcesses.Add(singleProcess);
                globalProcessDictionary.Add(singleProcess.baseCardId, newProcesses);
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
        newEntry.timeCost = rawCardObject.timeCost;
        newEntry.foodCost = rawCardObject.foodCost;


        switch (rawCardObject.type)
        {
            case "Resource":
                newEntry.type = CardsTypes.Resource;
                break;
            case "Infrastructure":
                newEntry.type = CardsTypes.Infrastructure;
                break;
            case "Gold":
                newEntry.type = CardsTypes.Gold;
                break;
            case "Electricity":
                newEntry.type = CardsTypes.Electricity;
                break;
            case "Module":
                newEntry.type = CardsTypes.Module;
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

}