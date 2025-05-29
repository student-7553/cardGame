using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager current;

	public GameObject floatingTextPrefab;
	public GameFoodManager gameFoodManager;
	public Descriptions descriptions;

	public SO_PlayerRuntime playerRuntime;
	public SO_Interactable so_Interactable;
	public SO_CardImage so_CardImage;
	public SO_Highlight so_Highlight;

	public bool isStartHighlightActive;

	public bool isFoodDecreasedEnabledForce;

	void Awake()
	{
		if (current != null)
		{
			Destroy(gameObject);
			return;
		}
		current = this;
		DontDestroyOnLoad(gameObject);

		gameFoodManager = new GameFoodManager { food = 5, isEnabled = isFoodDecreasedEnabledForce };
		CardDictionary.init(descriptions, so_CardImage);
		gameSettings();
	}

	public void startGame()
	{
		handleNewStart();
		AwakeGameLogic();

		if (isStartHighlightActive)
		{
			so_Highlight.isHighlightEnabled = true;
			so_Highlight.cardIds = new int[] { 12 };
			so_Highlight.highlightText = "You can move cards by dragging them, try it out :D";
			so_Highlight.highlightMainText = "Move \"Rock deposit\" card around";
			so_Highlight.objectiveText = "Move \"Rock deposit\" card around";
			so_Highlight.triggerRefresh();
		}
	}

	public void SpawnFloatingTexts(List<string> floatingTexts, Vector2 spawnLocation)
	{
		int baseHeightMin = -3;
		int baseheightMax = 3;

		int widthMin = -6;
		int widthMax = 6;

		Vector3 newSpawnBaseLocation =
			(Vector3)spawnLocation + new Vector3(Random.Range(widthMin, widthMax), Random.Range(baseHeightMin, baseheightMax), -8);

		Vector3 positionCounter = Vector3.zero;

		for (int i = 0; i < floatingTexts.Count; i++)
		{
			if (i % 2 == 0)
			{
				positionCounter = new Vector3(Random.Range(widthMin, widthMax), -positionCounter.y, 0);
			}
			else
			{
				positionCounter = new Vector3(Random.Range(widthMin, widthMax), (-positionCounter.y) + 3, 0);
			}
			Vector3 spawnPosition = newSpawnBaseLocation + positionCounter;

			GameObject floatingTextObject = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
			floatingTextObject.GetComponent<FloatingText>().Run(floatingTexts[i]);
		}
	}

	private void AwakeGameLogic()
	{
		GameObject[] cards = GameObject.FindGameObjectsWithTag("Cards");

		GameObject[] nodes = GameObject.FindGameObjectsWithTag("Nodes");

		GameObject[] enemyNodes = GameObject.FindGameObjectsWithTag("EnemyNodes");

		foreach (GameObject singleCard in cards)
		{
			StaticData cardData = singleCard.GetComponent(typeof(StaticData)) as StaticData;

			if (cardData != null)
			{
				CardHandler.current.createCard(cardData.id, singleCard.transform.position);
			}
			Destroy(singleCard);
		}

		foreach (GameObject singleNode in nodes)
		{
			StaticData nodeData = singleNode.GetComponent(typeof(StaticData)) as StaticData;
			if (nodeData != null)
			{
				CardHandler.current.createNode(nodeData.id, singleNode.transform.position);
			}
			Destroy(singleNode);
		}

		foreach (GameObject singleEnemyNode in enemyNodes)
		{
			StaticData nodeData = singleEnemyNode.GetComponent(typeof(StaticData)) as StaticData;
			if (nodeData != null)
			{
				CardHandler.current.createEnemyNode(nodeData.id, singleEnemyNode);
			}
		}
	}

	public void handleGamePauseAction()
	{
		if (playerRuntime.gameTimeScale > 0)
		{
			playerRuntime.gameTimeScale = 0;
		}
		else
		{
			playerRuntime.gameTimeScale = 1;
		}
	}

	public void handleFastTime()
	{
		playerRuntime.gameTimeScale = 2f;
	}

	public void handleNormalTime()
	{
		playerRuntime.gameTimeScale = 1f;
		return;
	}

	private void gameSettings()
	{
		Application.targetFrameRate = 60;
	}

	public void gameLost()
	{
		playerRuntime.gameTimeScale = 0;
		playerRuntime.isGameFailed = true;
	}

	private void handleNewStart()
	{
		so_Interactable.cards.Clear();
		so_Interactable.nodes.Clear();
		playerRuntime.gameTimeScale = 1f;
	}
}
