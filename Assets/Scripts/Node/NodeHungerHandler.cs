using UnityEngine;
using Core;

public class NodeHungerHandler : MonoBehaviour
{
	private Node connectedNode;

	[System.NonSerialized]
	public float intervalTimer; // ********* Loop timer *********

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

	public int getHungerCountdown()
	{
		return Mathf.RoundToInt(connectedNode.nodeStats.baseNodeStat.hungerSetIntervalTimer - intervalTimer);
	}

	private void FixedUpdate()
	{
		if (!isInit || !connectedNode.isActive)
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
			connectedNode.isActive = false;
		}
		else
		{
			StartCoroutine(connectedNode.nodeProcess.queUpTypeDeletion(CardsTypes.Food, foodMinus, 2f, null));
		}
	}
}
