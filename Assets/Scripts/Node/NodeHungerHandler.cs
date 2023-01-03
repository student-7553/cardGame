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
		isInit = true;
		// connectedNode = gameObject.GetComponent(typeof(Node)) as Node;
		connectedNode = parentNode;
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
