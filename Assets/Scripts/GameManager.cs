using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager current;

	private GameObject[] cards;
	private GameObject[] nodes;
	private GameObject[] enemyNodes;

	public InteractableManagerScriptableObject interactableManagerScriptableObject;

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
