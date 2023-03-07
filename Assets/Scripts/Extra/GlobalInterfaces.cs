using UnityEngine;

namespace Core
{
	public interface IStackable
	{
		void stackOnThis(Card draggingCard, Node prevNode);
		GameObject gameObject { get; }
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

	public static class NodeBaseStats
	{
		// public static BaseNodeStats base1Stats = new BaseNodeStats(10, 20, 1, 1, 120);
		// public static BaseNodeStats base2Stats = new BaseNodeStats(10, 20, 1, 1, 120);
		// public static BaseNodeStats base3Stats = new BaseNodeStats(10, 20, 1, 1, 120);

		public static BaseNodeStats base1Stats = new BaseNodeStats(10, 20, 1, 1, 10000);
		public static BaseNodeStats base2Stats = new BaseNodeStats(10, 30, 1, 1, 45);
		public static BaseNodeStats base3Stats = new BaseNodeStats(10, 30, 1, 1, 45);

		public static BaseNodeStats getBaseStats(int nodeId)
		{
			switch (nodeId)
			{
				case 3000:
					return base1Stats;
				case 3001:
					return base2Stats;
				case 3002:
					return base3Stats;
				default:
					return base1Stats;
			}
		}
	}

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
		public int updateCurrentNode;
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
		public int baseRequiredId;
		public int[] unlockCardIds;
		public int inNodeId;
		public int[] removingIds;
		public int[] requiredIds;
		public int requiredGold;
		public int requiredElectricity;
		public int time;
		public AddingCardsObject[] addingCardObjects;
	}
}
