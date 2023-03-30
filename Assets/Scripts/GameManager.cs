using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager current;

	private GameObject[] cards;
	private GameObject[] nodes;
	private GameObject[] enemyNodes;

	[System.NonSerialized]
	public BoardPlaneHandler boardPlaneHandler;

	[System.NonSerialized]
	public EnemySpawer enemySpawner;

	void Start()
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
		awakeGameLogic();
	}

	private void awakeGameLogic()
	{
		this.boardPlaneHandler = GetComponent(typeof(BoardPlaneHandler)) as BoardPlaneHandler;
		this.enemySpawner = GetComponent(typeof(EnemySpawer)) as EnemySpawer;
		// this.enemySpawner.init(5);

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

	private void gameSettings()
	{
		Application.targetFrameRate = 60;
	}
}
