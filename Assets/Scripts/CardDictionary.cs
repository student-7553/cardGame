using System.Collections.Generic;
using UnityEngine;


public enum CardsTypes
{
    Resource,
    Gold,
    Electricity,
    Infrastructure
}


[System.Serializable]
public class RawCardObject
{
    public int id;
    public string name;
    public string type;
    public int inventoryCount;
    public bool isSellable;
    public int sellingPrice;
    public int typeValue;
    public float timeCost;
    public int foodCost;
}

[System.Serializable]
public class RawProcessObject
{
    public int baseId;
    public int[] requiredIds;
    public int[] processedIds;
    public int requiredGold;
    public int requiredElectricity;
    public float time;
}

public static class CardDictionary
{
    public static Dictionary<int, RawCardObject> globalCardDictionary;
    public static Dictionary<int, List<RawProcessObject>> globalProcessDictionary;

    public static void init()
    {
        initCardDictionary();
        initProcessDictionary();
    }

    private static void initCardDictionary()
    {
        globalCardDictionary = new Dictionary<int, RawCardObject>();
        var jsonTextFile = Resources.Load<TextAsset>("Dictionary/card");
        RawCardObject[] listOfCards = JsonHelper.FromJson<RawCardObject>(jsonTextFile.text);
        foreach (RawCardObject singleCard in listOfCards)
        {
            globalCardDictionary.Add(singleCard.id, singleCard);
        }
    }

    private static void initProcessDictionary()
    {
        globalProcessDictionary = new Dictionary<int, List<RawProcessObject>>();
        var jsonTextFile = Resources.Load<TextAsset>("Dictionary/process");
        RawProcessObject[] listOfProcess = JsonHelper.FromJson<RawProcessObject>(jsonTextFile.text);
        foreach (RawProcessObject singleProcess in listOfProcess)
        {
            if (globalProcessDictionary.ContainsKey(singleProcess.baseId))
            {
                globalProcessDictionary[singleProcess.baseId].Add(singleProcess);
            }
            else
            {
                List<RawProcessObject> newProcesses = new List<RawProcessObject>();
                newProcesses.Add(singleProcess);
                globalProcessDictionary.Add(singleProcess.baseId, newProcesses);
            }
        }
    }

}