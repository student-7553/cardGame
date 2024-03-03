using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager current;

	// private GameObject[] cards;
	// private GameObject[] nodes;
	// private GameObject[] enemyNodes;

	public GameObject floatingTextPrefab;
	public GameFoodManager gameFoodManager;
	public Descriptions descriptions;

	public SO_PlayerRuntime playerRuntime;
	public SO_Interactable so_Interactable;

	void Awake()
	{
		if (current != null)
		{
			Destroy(gameObject);
			return;
		}
		current = this;
		DontDestroyOnLoad(gameObject);

		CardDictionary.init(descriptions);
		gameSettings();
		// startGame();
	}

	public void startGame()
	{
		handleNewStart();
		// findTempLogic();
		AwakeGameLogic();
		gameFoodManager = new GameFoodManager { food = 0 };
	}

	public void SpawnFloatingText(string floatingText, Vector2 spawnLocation)
	{
		int heightMin = -2;
		int heightMax = 2;

		int widthMin = -2;
		int widthMax = 2;

		Vector3 newSpawnLocation =
			(Vector3)spawnLocation + new Vector3(Random.Range(widthMin, widthMax), Random.Range(heightMin, heightMax), -8);

		GameObject floatingTextObject = Instantiate(floatingTextPrefab, newSpawnLocation, Quaternion.identity);

		floatingTextObject.GetComponent<FloatingText>().Run(floatingText);
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

	private void handleNewStart()
	{
		so_Interactable.cards.Clear();
		so_Interactable.nodes.Clear();
		playerRuntime.gameTimeScale = 1f;
	}
}
