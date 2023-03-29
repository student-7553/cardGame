using UnityEngine;
using Core;
using Helpers;

[DefaultExecutionOrder(-100)]
public class CardHandler : MonoBehaviour
{
	public PlayerCardTrackerObject playerCardTracker;
	public static CardHandler current;

	public GameObject cardPrefab;
	public GameObject nodePrefab;
	public GameObject nodePlanePrefab;

	public Sprite[] cardSprites;
	public Sprite[] nodeSprites;

	private Vector3 defaultCardPoint;
	private Vector3 defaultNodePlanePositon = new Vector3(103, 0, HelperData.nodeBoardZ);
	private Vector3 defaultNodePositon = new Vector3(225, 0, HelperData.baseZ);

	void Start()
	{
		if (current != null)
		{
			Destroy(gameObject);
			return;
		}
		current = this;

		defaultCardPoint = new Vector3(0, 0, HelperData.baseZ);
		playerCardTracker = new PlayerCardTrackerObject();
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

		Card cardObject = ensureComponent<Card>(cardGameObject);

		cardObject.id = cardId;
		SpriteRenderer cardSpriteRenderer = ensureComponent<SpriteRenderer>(cardGameObject);
		cardSpriteRenderer.sprite = cardSprites[Random.Range(0, cardSprites.Length)];

		cardObject.init();
		playerCardTracker.ensureCardIdTracked(cardId);
		return cardObject;
	}

	public Node createNode(int cardId, GameObject nodeGameObject)
	{
		nodeGameObject.name = CardDictionary.globalCardDictionary[cardId].name;
		nodeGameObject.tag = "Nodes";
		nodeGameObject.layer = 6;

		// nodeGameObject.transform.position = this.defaultNodePositon;

		Node newNode = ensureComponent<Node>(nodeGameObject);
		newNode.id = cardId;

		ensureComponent<NodeCardQue>(nodeGameObject);

		ensureComponent<NodeProcess>(nodeGameObject);

		ensureComponent<NodeHungerHandler>(nodeGameObject);

		GameObject newNodePlane = Instantiate(nodePlanePrefab, this.defaultNodePlanePositon, Quaternion.identity);

		NodePlaneHandler nodePlane = newNodePlane.GetComponent(typeof(NodePlaneHandler)) as NodePlaneHandler;
		nodePlane.init(newNode);
		newNodePlane.SetActive(false);

		newNode.init(nodePlane);

		return newNode;
	}

	public EnemyNode createEnemyNode(int cardId, GameObject nodeGameObject)
	{
		nodeGameObject.name = CardDictionary.globalCardDictionary[cardId].name;
		nodeGameObject.tag = "EnemyNodes";
		nodeGameObject.layer = 7;

		EnemyNode newEnemyNode = ensureComponent<EnemyNode>(nodeGameObject);
		newEnemyNode.id = cardId;

		ensureComponent<EnemyNodeProcess>(nodeGameObject);

		GameObject newNodePlane = Instantiate(nodePlanePrefab, this.defaultNodePlanePositon, Quaternion.identity);
		NodePlaneHandler nodePlane = newNodePlane.GetComponent(typeof(NodePlaneHandler)) as NodePlaneHandler;
		nodePlane.init(newEnemyNode);
		newNodePlane.SetActive(false);

		newEnemyNode.init(nodePlane);

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
		GameObject newNodeGameObject = Instantiate(nodePrefab);
		return this.createNode(cardId, newNodeGameObject);
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
}
