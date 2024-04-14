using UnityEngine;
using TMPro;

public class NodeHungerHandler : MonoBehaviour
{
	private Node connectedNode;

	[System.NonSerialized]
	public float intervalTimer; // ********* Loop timer *********

	private bool isInit = false;

	private float internalTimer = 0f;

	public TextMeshPro nodeHungerTimer;

	public GameObject nodeBottomPart;

	[System.NonSerialized]
	public SO_PlayerRuntime playerRuntime;

	public void Awake()
	{
		intervalTimer = 0;
	}

	public void init(Node parentNode)
	{
		connectedNode = parentNode;
		if (parentNode.isMarket())
		{
			nodeBottomPart.SetActive(false);
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
		internalTimer = internalTimer + Time.fixedDeltaTime;
		if (internalTimer >= 1f)
		{
			handleSecondTick();
			internalTimer = 0;
		}
	}

	private void handleSecondTick()
	{
		if (!isInit || !connectedNode.isActive || !GameManager.current.gameFoodManager.isEnabled)
		{
			return;
		}

		intervalTimer = intervalTimer + playerRuntime.gameTimeScale;
		if (intervalTimer > connectedNode.nodeStats.baseNodeStat.hungerSetIntervalTimer)
		{
			intervalTimer = 0;
			handleHungerInterval();
		}
		nodeHungerTimer.text = $"{getHungerCountdown()}";
	}

	private void handleHungerInterval()
	{
		handleHunger(connectedNode.nodeStats.currentNodeStats.currentFoodCheck);
	}

	private void handleHunger(int foodValue)
	{
		int currentFoodValue = GameManager.current.gameFoodManager.food;
		if (currentFoodValue == 0)
		{
			GameManager.current.gameLost();
			return;
		}

		GameManager.current.gameFoodManager.decreaseFood(foodValue);
	}
}
