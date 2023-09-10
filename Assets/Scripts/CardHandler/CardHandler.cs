using UnityEngine;
using Core;
using Helpers;

// [DefaultExecutionOrder(-100)]
public class CardHandler : MonoBehaviour
{
	public PlayerCardTrackerObject playerCardTracker;
	public static CardHandler current;

	// -----------------------PREFAB--------------------------
	public GameObject cardPrefab;
	public GameObject nodePrefab;
	public GameObject enemyNodePrefab;
	public GameObject nodePlanePrefab;
	public GameObject nodeMagnetizeCirclePrefab;

	public Sprite[] cardSprites;
	public Sprite[] nodeSprites;

	private Vector3 defaultCardPoint;
	private Vector3 defaultNodePoint;

	public Vector3 defaultNodePlanePositon;

	// public Vector2Int enemySpawnInterval;

	private EnemySpawer enemySpawner;

	public InteractableManagerScriptableObject interactableManagerScriptableObject;
	public StaticVariables staticVariables;

	public bool disableEnemySpawner = false;

	void Awake()
	{
		if (current != null)
		{
			Destroy(gameObject);
			return;
		}
		current = this;

		defaultCardPoint = new Vector3(0, 0, HelperData.baseZ);
		defaultNodePoint = new Vector3(0, 0, HelperData.baseZ);
		playerCardTracker = new PlayerCardTrackerObject();
		defaultNodePlanePositon = new Vector3(
			staticVariables.defaultNodePlanePositon.x,
			staticVariables.defaultNodePlanePositon.y,
			HelperData.nodeBoardZ
		);

		enemySpawner = GetComponent(typeof(EnemySpawer)) as EnemySpawer;

		// test enemySpawn  Comment out the bellow later
		// enemySpawner.start(enemySpawnInterval.x, enemySpawnInterval.y);
	}

	public Card createCard(int cardId, GameObject cardGameObject, Vector3 cardOriginPoint)
	{
		if (!CardDictionary.globalCardDictionary.ContainsKey(cardId))
		{
			return null;
		}
		cardGameObject.name = CardDictionary.globalCardDictionary[cardId].name;
		cardGameObject.tag = "Cards";
		cardGameObject.layer = 6;
		cardGameObject.transform.position = cardOriginPoint;

		Card newCard = ensureComponent<Card>(cardGameObject);

		newCard.interactableManagerScriptableObject = interactableManagerScriptableObject;
		newCard.id = cardId;

		SpriteRenderer cardSpriteRenderer = ensureComponent<SpriteRenderer>(cardGameObject);
		cardSpriteRenderer.sprite = cardSprites[Random.Range(0, cardSprites.Length)];

		playerCardTracker.ensureCardIdTracked(cardId);
		interactableManagerScriptableObject.registerCard(newCard);

		tempCardHooks(cardId);

		return newCard;
	}

	public Node createNode(int cardId, GameObject nodeGameObject)
	{
		nodeGameObject.name = CardDictionary.globalCardDictionary[cardId].name;
		nodeGameObject.tag = "Nodes";
		nodeGameObject.layer = 6;

		Node newNode = ensureComponent<Node>(nodeGameObject);
		newNode.id = cardId;
		newNode.interactableManagerScriptableObject = interactableManagerScriptableObject;
		newNode.staticVariables = staticVariables;

		ensureComponent<NodeCardQue>(nodeGameObject);
		ensureComponent<NodeProcess>(nodeGameObject);
		NodeHungerHandler nodeHungerHandler = ensureComponent<NodeHungerHandler>(nodeGameObject);

		GameObject newNodePlane = Instantiate(nodePlanePrefab, defaultNodePlanePositon, Quaternion.identity);
		newNodePlane.SetActive(false);

		NodePlaneHandler nodePlane = newNodePlane.GetComponent(typeof(NodePlaneHandler)) as NodePlaneHandler;
		nodePlane.init(newNode);

		GameObject nodeMagnetizeCircleGameObject = Instantiate(nodeMagnetizeCirclePrefab, nodeGameObject.transform);
		NodeMagnetizeCircle nodeMagnetizeCircle = nodeMagnetizeCircleGameObject.GetComponent<NodeMagnetizeCircle>();
		nodeMagnetizeCircle.init(newNode);

		newNode.init(nodePlane, nodeMagnetizeCircle);

		interactableManagerScriptableObject.registerNode(newNode);

		return newNode;
	}

	public EnemyNode createEnemyNode(int cardId, GameObject nodeGameObject)
	{
		nodeGameObject.name = CardDictionary.globalCardDictionary[cardId].name;
		nodeGameObject.tag = "EnemyNodes";
		nodeGameObject.layer = 7;

		EnemyNode newEnemyNode = ensureComponent<EnemyNode>(nodeGameObject);
		newEnemyNode.id = cardId;
		newEnemyNode.interactableManagerScriptableObject = interactableManagerScriptableObject;

		ensureComponent<EnemyNodeProcess>(nodeGameObject);

		GameObject newNodePlane = Instantiate(nodePlanePrefab, defaultNodePlanePositon, Quaternion.identity);
		NodePlaneHandler nodePlane = newNodePlane.GetComponent(typeof(NodePlaneHandler)) as NodePlaneHandler;
		nodePlane.init(newEnemyNode);
		newNodePlane.SetActive(false);

		newEnemyNode.init(nodePlane, CardDictionary.globalCardDictionary[cardId].nodeTransferTimeCost);

		return newEnemyNode;
	}

	public Card createCard(int cardId, Vector3 cardOriginPoint)
	{
		GameObject newNodePlane = Instantiate(cardPrefab);
		newNodePlane.transform.position = cardOriginPoint;
		newNodePlane.SetActive(true);

		return createCard(cardId, newNodePlane, cardOriginPoint);
	}

	public Card createCard(int cardId)
	{
		return createCard(cardId, defaultCardPoint);
	}

	public Node createNode(int cardId)
	{
		GameObject newNodeGameObject = Instantiate(nodePrefab, defaultNodePoint, Quaternion.identity);
		return createNode(cardId, newNodeGameObject);
	}

	public EnemyNode createEnemyNode(int cardId)
	{
		GameObject newNodeGameObject = Instantiate(enemyNodePrefab);
		return createEnemyNode(cardId, newNodeGameObject);
	}

	private T ensureComponent<T>(GameObject gameObject) where T : Component
	{
		var cardSpriteRenderer = gameObject.GetComponent(typeof(T)) as T;
		if (cardSpriteRenderer == null)
		{
			cardSpriteRenderer = gameObject.AddComponent<T>();
		}
		;
		return cardSpriteRenderer;
	}

	private void tempCardHooks(int cardId)
	{
		// Global expidition
		if (cardId == 1004 && disableEnemySpawner == false)
		{
			enemySpawner.Run(EnemySpawer.EnemySpawner_Tier.tier_1);
		}

		// Zenith idea
		if (cardId == 2017 && disableEnemySpawner == false)
		{
			enemySpawner.Run(EnemySpawer.EnemySpawner_Tier.tier_2);
		}

		if (cardId == staticVariables.endingCardId)
		{
			Debug.Log("GAME ENDED");
		}
	}
}
