
// using UnityEngine;
using System.Collections.Generic;
namespace Core
{
    public interface IStackable
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

    public enum NodeStateTypes
    {
        base_1, // low
        base_2, // medium
        base_3, // high
        market_1, // market low
    };
    public enum CardsTypes
    {
        Resource,
        Gold,
        Electricity,
        Food,
        Infrastructure,
        Module,
        Idea,
        Node,
        CombatUnit,
        Enemy
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
    public class AddingCardsObject
    {
        public float odds;
        public int[] addingCardIds;
        public int[] extraUnlockCardIds;
        public bool isOneTime;
        public int id;
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
        public float nodeTransferTimeCost;
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
        public float nodeTransferTimeCost;
        public int foodCost;
        public CardModuleObject module;
    }

    [System.Serializable]
    public class RawProcessObject
    {
        public int id;
        public int baseCardId;
        public int[] unlockCardIds;
        public int[] requiredIds;
        public int requiredGold;
        public int requiredElectricity;
        public float time;
        public AddingCardsObject[] addingCardObjects;
    }


}