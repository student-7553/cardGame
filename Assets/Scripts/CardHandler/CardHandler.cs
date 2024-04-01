using UnityEngine;
using Core;
using System.Collections.Generic;
using Helpers;

public class CardHandler : MonoBehaviour
{
	public PlayerCardTrackerObject playerCardTracker;
	public static CardHandler current;

	// -----------------------PREFAB--------------------------
	public GameObject cardPrefab;
	public GameObject cardCollapsedPrefab;
	public GameObject nodePrefab;
	public GameObject enemyNodePrefab;
	public GameObject nodePlanePrefab;

	public GameObject collapsedCardPrefab;

	public GameObject nodeMagnetizeCirclePrefab;

	private Vector3 defaultCardPoint;
	private Vector3 defaultNodePoint;

	private EnemySpawer enemySpawner;

	public SO_Interactable so_Interactable;
	public StaticVariables staticVariables;
	public SO_PlayerRuntime playerRuntime;
	public SO_Highlight so_Highlight;

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

		enemySpawner = GetComponent(typeof(EnemySpawer)) as EnemySpawer;
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

		newCard.id = cardId;

		playerCardTracker.ensureCardIdTracked(cardId);
		so_Interactable.registerCard(newCard);

		roughCardHooks(cardId);

		return newCard;
	}

	public CardCollapsed createCardCollapsed(int cardId)
	{
		if (!CardDictionary.globalCardDictionary.ContainsKey(cardId))
		{
			return null;
		}

		GameObject cardCollapsedGameObject = Instantiate(cardCollapsedPrefab);

		cardCollapsedGameObject.name = CardDictionary.globalCardDictionary[cardId].name;
		cardCollapsedGameObject.tag = "Cards";
		cardCollapsedGameObject.layer = 6;

		CardCollapsed newCardCollapsed = ensureComponent<CardCollapsed>(cardCollapsedGameObject);

		newCardCollapsed.id = cardId;

		GameObject newCollapsedCardPlane = Instantiate(
			collapsedCardPrefab,
			new Vector3(0, 27, 0),
			Quaternion.identity,
			cardCollapsedGameObject.transform
		);

		newCollapsedCardPlane.SetActive(false);
		newCollapsedCardPlane.layer = 6;

		CardCollapsedPlaneHandler cardCollapsedPlane = ensureComponent<CardCollapsedPlaneHandler>(newCollapsedCardPlane);
		cardCollapsedPlane.init(newCardCollapsed);

		newCardCollapsed.cardCollapsedPlaneHandler = cardCollapsedPlane;

		return newCardCollapsed;
	}

	public Node createNode(int cardId, GameObject nodeGameObject)
	{
		nodeGameObject.name = CardDictionary.globalCardDictionary[cardId].name;
		nodeGameObject.tag = "Nodes";
		nodeGameObject.layer = 6;

		Node newNode = ensureComponent<Node>(nodeGameObject);
		newNode.id = cardId;

		ensureComponent<NodeCardQue>(nodeGameObject);
		NodeProcess nodeProcess = ensureComponent<NodeProcess>(nodeGameObject);
		nodeProcess.playerRuntime = playerRuntime;

		NodeHungerHandler nodeHungerHandler = ensureComponent<NodeHungerHandler>(nodeGameObject);
		nodeHungerHandler.playerRuntime = playerRuntime;

		GameObject newNodePlane = Instantiate(nodePlanePrefab);

		newNodePlane.layer = 6;
		newNodePlane.SetActive(false);

		NodePlaneHandler nodePlane = ensureComponent<NodePlaneHandler>(newNodePlane);
		nodePlane.init(newNode);

		GameObject nodeMagnetizeCircleGameObject = Instantiate(nodeMagnetizeCirclePrefab, nodeGameObject.transform);
		NodeMagnetizeCircle nodeMagnetizeCircle = nodeMagnetizeCircleGameObject.GetComponent<NodeMagnetizeCircle>();
		nodeMagnetizeCircle.init(newNode);

		newNode.init(nodePlane, nodeMagnetizeCircle);

		so_Interactable.registerNode(newNode);

		return newNode;
	}

	public List<Card> handleCreatingCards(List<int> cardIds)
	{
		if (cardIds.Count == 0)
		{
			return new List<Card>();
		}

		List<Card> addingCards = new List<Card>();
		foreach (int singleAddingCardId in cardIds)
		{
			if (CardDictionary.globalCardDictionary[singleAddingCardId].type == CardsTypes.Node)
			{
				createNode(singleAddingCardId);
			}
			else
			{
				Card createdCard = createCard(singleAddingCardId);
				addingCards.Add(createdCard);
			}
		}
		return addingCards;
	}

	public EnemyNode createEnemyNode(int cardId, GameObject nodeGameObject)
	{
		nodeGameObject.name = CardDictionary.globalCardDictionary[cardId].name;
		nodeGameObject.tag = "EnemyNodes";
		nodeGameObject.layer = 7;

		EnemyNode newEnemyNode = ensureComponent<EnemyNode>(nodeGameObject);
		newEnemyNode.id = cardId;
		newEnemyNode.so_Interactable = so_Interactable;

		ensureComponent<EnemyNodeProcess>(nodeGameObject);

		Vector3 nodePlanePositon = new Vector3(
			newEnemyNode.transform.position.x,
			newEnemyNode.transform.position.y + 15f,
			HelperData.nodeBoardZ
		);
		GameObject newNodePlane = Instantiate(nodePlanePrefab, nodePlanePositon, Quaternion.identity);
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

	public Node createNode(int cardId, Vector3 spawnPosition)
	{
		GameObject newNodeGameObject = Instantiate(nodePrefab);
		newNodeGameObject.transform.position = spawnPosition;
		return createNode(cardId, newNodeGameObject);
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

	private void roughCardHooks(int cardId)
	{
		// 2001 - [Idea][Node] Space dome
		if (cardId == 2001)
		{
			so_Highlight.isHighlightEnabled = true;
			so_Highlight.cardIds = new int[] { 2001 };
			so_Highlight.ideaId = 2001;
			so_Highlight.highlightText =
				"New cards :v). Right sidebar contains information about all new cards you can create \nClick \"Space dome\" side tab";
			so_Highlight.triggerRefresh();
			return;
		}
		// 1004 - Global expidition
		if (cardId == 1004 && disableEnemySpawner == false)
		{
			enemySpawner.Run(EnemySpawer.EnemySpawner_Tier.tier_1);
			return;
		}

		// 27 - Core pillar
		if (cardId == 27 && disableEnemySpawner == false)
		{
			enemySpawner.Run(EnemySpawer.EnemySpawner_Tier.tier_2);

			return;
		}

		if (cardId == staticVariables.endingCardId)
		{
			Debug.Log("GAME ENDED");
			//Todo
		}
	}
}
