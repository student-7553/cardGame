using UnityEngine;

public class GameManager : MonoBehaviour
{
	// ---------------------- REFERENCES ------------------------------
	public GameObject cardHandlerGameObject;
	public GameObject nodePlane;

	public static GameManager current;

	private GameObject[] cards;
	private GameObject[] nodes;

	[System.NonSerialized]
	public BoardPlaneHandler boardPlaneHandler;

	void Start()
	{
		if (current != null)
		{
			Destroy(gameObject);
			return;
		}
		current = this;

		CardDictionary.init();

		DontDestroyOnLoad(gameObject);
		gameSettings();
		findTempLogic();
		awakeGameLogic();
	}

	private void awakeGameLogic()
	{
		this.boardPlaneHandler = GameObject.FindObjectOfType<BoardPlaneHandler>();

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
	}

	private void gameSettings()
	{
		Application.targetFrameRate = 60;
	}
}
