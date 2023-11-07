using Helpers;
using UnityEngine;

namespace Core
{
	public interface IStackable
	{
		void stackOnThis(BaseCard draggingCard, Node prevNode);
		GameObject gameObject { get; }
	}

	public interface IClickable
	{
		void OnClick();
	}

	public interface IMouseHoldable
	{
		public Interactable[] getMouseHoldInteractables();
	}

	public interface Interactable : IMouseHoldable
	{
		public bool isInteractiveDisabled { get; set; }
		public CoreInteractableType interactableType { get; }
		public bool isCardType();
		public BaseCard getBaseCard();
		public Card getCard();
		public CardCollapsed getCollapsedCard();
		public ref Vector3 getCurrentVelocity();
		GameObject gameObject { get; }
	}

	public enum CoreInteractableType
	{
		Cards,
		CollapsedCards,
		Nodes
	}

	public enum CardStackType
	{
		Cards,
		CollapsedCards,
		Nodes
	}

	public static class NodeBaseStats
	{
		public static BaseNodeStats base1Stats = new(0, 10, 1, 1, 150);
		public static BaseNodeStats base2Stats = new(2, 15, 1, 1, 130);
		public static BaseNodeStats base3Stats = new(4, 20, 1, 1, 130);

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
		Will,
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
		public int[] isMagnetizedCardIds;
		public int unityCount;
	}

	[System.Serializable]
	public class AddingCardsObject
	{
		public int id;
		public float odds;
		public int[] addingCardIds;
		public int updateCurrentNode;
		public int addingFood;
		public int[] extraUnlockCardIds;
		public bool isOneTime;
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
	public class NodeRequirement
	{
		public int[] mustBeNodeIds;
	}

	[System.Serializable]
	public class RawProcessObject
	{
		public int id;
		public int baseRequiredId;
		public int[] unlockCardIds;

		public NodeRequirement nodeRequirement;
		public int[] removingIds;
		public int[] requiredIds;
		public int requiredGold;
		public int requiredElectricity;
		public int requiredWill;
		public int time;
		public float priority;
		public AddingCardsObject[] addingCardObjects;
	}
}
