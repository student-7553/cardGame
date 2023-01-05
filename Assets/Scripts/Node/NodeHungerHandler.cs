using UnityEngine;
using Core;

public class NodeHungerHandler : MonoBehaviour
{
	private Node connectedNode;

	private float intervalTimer; // ********* Loop timer *********

	private bool isInit = false;

	public void Awake()
	{
		intervalTimer = 0;
	}

	public void init(Node parentNode)
	{
		connectedNode = parentNode;
		if (parentNode.isMarket())
		{
			return;
		}

		isInit = true;
	}

	private void FixedUpdate()
	{
		if (!isInit)
		{
			return;
		}

		intervalTimer = intervalTimer + Time.deltaTime;
		if (intervalTimer > connectedNode.nodeStats.baseNodeStat.hungerSetIntervalTimer)
		{
			intervalTimer = 0;
			this.handleHungerInterval();
		}
	}

	private void handleHungerInterval()
	{
		Debug.Log("Deleting food from this handleHungerInterval");
		int foodMinus = connectedNode.nodeStats.currentNodeStats.currentFoodCheck;
		if (connectedNode.nodeStats.currentNodeStats.currentFood - foodMinus <= 0)
		{
			StartCoroutine(
				connectedNode.handleCardTypeDeletion(CardsTypes.Food, connectedNode.nodeStats.currentNodeStats.currentFood, 3f, null)
			);
			connectedNode.isActive = false;
		}
		else
		{
			Debug.Log("Deleting food from this handleHungerInterval/" + foodMinus);
			StartCoroutine(connectedNode.handleCardTypeDeletion(CardsTypes.Food, foodMinus, 3f, null));
		}
	}
}
