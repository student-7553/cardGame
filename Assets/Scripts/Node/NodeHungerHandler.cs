using UnityEngine;
using Core;

public class NodeHungerHandler : MonoBehaviour
{
	private Node connectedNode;

	private float intervalTimer; // ********* Loop timer *********

	public void Awake()
	{
		connectedNode = gameObject.GetComponent(typeof(Node)) as Node;
	}

	private void FixedUpdate()
	{
		intervalTimer = intervalTimer + Time.deltaTime;
		int hungerSetIntervalTimer = connectedNode.nodeStats.baseNodeStat.hungerSetIntervalTimer;
		if (intervalTimer > hungerSetIntervalTimer)
		{
			this.handleHungerInterval();
			intervalTimer = 0;
		}
	}

	private void handleHungerInterval()
	{
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
			StartCoroutine(connectedNode.handleCardTypeDeletion(CardsTypes.Food, foodMinus, 3f, null));
		}
	}
}
