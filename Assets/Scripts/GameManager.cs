using System.Collections;
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

		gameFoodManager = new GameFoodManager(handleFoodIconFlash) { food = 5, isEnabled = isFoodDecreasedEnabledForce };
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

			so_Highlight.cardIds = new int[] {  };
			so_Highlight.highlightText = "You can move the screen by pressing WASD";
			so_Highlight.highlightMainText = "Move screen";
			so_Highlight.objectiveText = "Move screen";
			so_Highlight.triggerRefresh();
		}
	}

	public IEnumerator SpawnFloatingTexts(List<string> floatingTexts, Vector2 spawnLocation)
	{
		int baseHeightMin = -2;
		int baseheightMax = 2;

		int widthMin = -2;
		int widthMax = 2;

		Vector3 newSpawnBaseLocation =
			(Vector3)spawnLocation + new Vector3(Random.Range(widthMin, widthMax), Random.Range(baseHeightMin, baseheightMax), -8);

		Vector3 positionCounter = Vector3.zero;

		for (int i = 0; i < floatingTexts.Count; i++)
		{
			positionCounter = new Vector3(Random.Range(widthMin, widthMax), positionCounter.y - 1.25f, 0);

			Vector3 spawnPosition = newSpawnBaseLocation + positionCounter;
			GameObject floatingTextObject = Instantiate(floatingTextPrefab, spawnPosition, Quaternion.identity);
			floatingTextObject.GetComponent<FloatingText>().Run(floatingTexts[i]);
			yield return new WaitForSeconds(0.05f);
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

	private void handleFoodIconFlash()
	{
		StartCoroutine(handleFoodIconFlashAsync());
	}

	private IEnumerator handleFoodIconFlashAsync()
	{
		so_Highlight.isFoodFlashing = true;
		so_Highlight.triggerRefresh();
		yield return new WaitForSeconds(2f);
		so_Highlight.isFoodFlashing = false;
		so_Highlight.triggerRefresh();
	}
}
