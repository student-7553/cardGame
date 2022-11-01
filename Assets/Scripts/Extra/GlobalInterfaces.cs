
using UnityEngine;
using System.Collections.Generic;
namespace Core
{
    public interface Stackable
    {
        void stackOnThis(List<Card> draggingCards);
    }

    public interface IClickable
    {
        void OnClick();
    }
    public enum CoreInteractableType
    {
        Cards,
        Nodes
    }
    public enum CardStackType
    {
        Cards,
        Nodes
    }

    enum NodeStateTypes
    {
        low,
        medium,
    };
    public enum CardsTypes
    {
        Resource,
        Gold,
        Electricity,
        Food,
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
        public int resourceInventoryIncrease;
        public int infraInventoryIncrease;
        public int increaseGoldGeneration;
        public float minusMovementTime;
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


}