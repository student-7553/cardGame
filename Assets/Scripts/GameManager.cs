using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager current;

	private GameObject[] cards;
	private GameObject[] nodes;
	private GameObject[] enemyNodes;

	public GameObject floatingTextPrefab;
	public GameFoodManager gameFoodManager;

	void Awake()
	{
		if (current != null)
		{
			Destroy(gameObject);
			return;
		}
		current = this;
		DontDestroyOnLoad(gameObject);

		CardDictionary.init();

		gameSettings();
		findTempLogic();
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
		foreach (GameObject singleCard in cards)
		{
			StaticData cardData = singleCard.GetComponent(typeof(StaticData)) as StaticData;
			if (cardData != null)
			{
				CardHandler.current.createCard(cardData.id, singleCard, singleCard.transform.position);
			}
		}

		foreach (GameObject singleNode in nodes)
		{
			StaticData nodeData = singleNode.GetComponent(typeof(StaticData)) as StaticData;
			if (nodeData != null)
			{
				CardHandler.current.createNode(nodeData.id, singleNode);
			}
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

	public void findTempLogic()
	{
		if (cards == null)
		{
			cards = GameObject.FindGameObjectsWithTag("Cards");
		}

		if (nodes == null)
		{
			nodes = GameObject.FindGameObjectsWithTag("Nodes");
		}

		if (enemyNodes == null)
		{
			enemyNodes = GameObject.FindGameObjectsWithTag("EnemyNodes");
		}
	}

	public void handleGamePauseAction()
	{
		if (Time.timeScale == 0)
		{
			Time.timeScale = 1;
			Debug.Log("We are Resumed");
		}
		else
		{
			Time.timeScale = 0;
			Debug.Log("We are Paused");
		}
	}

	private void gameSettings()
	{
		Application.targetFrameRate = 60;
	}
}
