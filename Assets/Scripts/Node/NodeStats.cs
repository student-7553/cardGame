using System.Collections.Generic;
using Core;

public class BaseNodeStats
{
	public int infraInventoryLimit;
	public int resourceInventoryLimit;
	public int currentFoodCheck;
	public int goldGeneration;
	public int hungerSetIntervalTimer;

	public BaseNodeStats(
		int _infraInventoryLimit,
		int _resourceInventoryLimit,
		int _currentFoodCheck,
		int _goldGeneration,
		int _hungerSetIntervalTimer
	)
	{
		infraInventoryLimit = _infraInventoryLimit;
		resourceInventoryLimit = _resourceInventoryLimit;
		currentFoodCheck = _currentFoodCheck;
		goldGeneration = _goldGeneration;
		hungerSetIntervalTimer = _hungerSetIntervalTimer;
	}
}

public class CurrentNodeStats
{
	public int resourceInventoryLimit;

	public int resourceInventoryUsed;

	public int infraInventoryLimit;

	public int infraInventoryUsed;

	public int currentFoodCheck;

	public int currentElectricity;

	public int currentGold;

	public int goldGeneration;

	public int currentFood;
}

public class NodeStats
{
	public BaseNodeStats baseNodeStat;

	public CurrentNodeStats currentNodeStats;

	private Node connectedNode;

	public NodeStats(Node _connectedNode)
	{
		int[] baseNodeStats = NodeBaseStats.getBaseStats(_connectedNode.id);
		baseNodeStat = new BaseNodeStats(baseNodeStats[0], baseNodeStats[1], baseNodeStats[2], baseNodeStats[3], baseNodeStats[4]);

		currentNodeStats = new CurrentNodeStats();
		connectedNode = _connectedNode;
	}

	// private

	public void injectGold(int goldAmount)
	{
		currentNodeStats.currentGold = currentNodeStats.currentGold + goldAmount;
	}

	public void injectFood(int foodAmount)
	{
		currentNodeStats.currentFood = currentNodeStats.currentFood + foodAmount;
	}

	public void computeStats()
	{
		List<int> storageCardIds = connectedNode.storageCardStack.getActiveCardIds();
		int calcResourceInventoryUsed = 0;
		int calcResourceInventoryLimit = 0;
		int calcInfraInventoryUsed = 0;
		int calcInfraInventoryLimit = 0;
		int calcGoldGeneration = 0;
		int calcHungerCheck = 0;
		int calcElectricity = 0;
		int calcGold = 0;
		int calcFood = 0;

		foreach (int id in storageCardIds)
		{
			calcResourceInventoryUsed += CardDictionary.globalCardDictionary[id].resourceInventoryCount;
			calcInfraInventoryUsed += CardDictionary.globalCardDictionary[id].infraInventoryCount;
			calcHungerCheck += CardDictionary.globalCardDictionary[id].foodCost;
			switch (CardDictionary.globalCardDictionary[id].type)
			{
				case CardsTypes.Electricity:
					calcElectricity += CardDictionary.globalCardDictionary[id].typeValue;
					break;
				case CardsTypes.Gold:
					calcGold += CardDictionary.globalCardDictionary[id].typeValue;
					break;
				case CardsTypes.Food:
					calcFood += CardDictionary.globalCardDictionary[id].typeValue;
					break;
				case CardsTypes.Module:
					calcResourceInventoryLimit += CardDictionary.globalCardDictionary[id].module.resourceInventoryIncrease;
					calcInfraInventoryLimit += CardDictionary.globalCardDictionary[id].module.infraInventoryIncrease;
					calcGoldGeneration += CardDictionary.globalCardDictionary[id].module.increaseGoldGeneration;
					break;
				default:
					break;
			}
		}

		currentNodeStats.resourceInventoryUsed = calcResourceInventoryUsed;
		currentNodeStats.resourceInventoryLimit = baseNodeStat.resourceInventoryLimit + calcResourceInventoryLimit;
		currentNodeStats.infraInventoryUsed = calcInfraInventoryUsed;
		currentNodeStats.infraInventoryLimit = baseNodeStat.infraInventoryLimit + calcInfraInventoryLimit;
		currentNodeStats.goldGeneration = baseNodeStat.goldGeneration + calcGoldGeneration;
		currentNodeStats.currentFoodCheck = baseNodeStat.currentFoodCheck + calcHungerCheck;
		currentNodeStats.currentElectricity = calcElectricity;
		currentNodeStats.currentGold = calcGold;
		currentNodeStats.currentFood = calcFood;
	}

	public void handleLimits()
	{
		if (currentNodeStats.infraInventoryUsed > currentNodeStats.infraInventoryLimit)
		{
			int ejectingResourceValue = currentNodeStats.infraInventoryUsed - currentNodeStats.infraInventoryLimit;
			List<Card> ejectedCards = getEjectedCards(connectedNode.storageCardStack.cards, ejectingResourceValue, false);
			connectedNode.ejectCards(ejectedCards);
		}
		if (currentNodeStats.resourceInventoryUsed > currentNodeStats.resourceInventoryLimit)
		{
			int ejectingResourceValue = currentNodeStats.resourceInventoryUsed - currentNodeStats.resourceInventoryLimit;
			List<Card> ejectedCards = getEjectedCards(connectedNode.storageCardStack.cards, ejectingResourceValue, true);
			connectedNode.ejectCards(ejectedCards);
		}

		List<Card> getEjectedCards(List<Card> cards, int ejectingResourceValue, bool isResource)
		{
			List<Card> ejectedCards = new List<Card>();
			for (int index = cards.Count - 1; index >= 0; index--)
			{
				if (ejectingResourceValue > 0)
				{
					ejectingResourceValue =
						ejectingResourceValue
						- (
							isResource
								? CardDictionary.globalCardDictionary[cards[index].id].resourceInventoryCount
								: CardDictionary.globalCardDictionary[cards[index].id].infraInventoryCount
						);
					ejectedCards.Add(cards[index]);
				}
				else
				{
					break;
				}
			}
			return ejectedCards;
		}
	}
}
