using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Core;
using System.Linq;
using Helpers;

public class Node : MonoBehaviour, BaseNode
{
	public bool isCardType()
	{
		return false;
	}

	// -------------------- Interactable Members -------------------------
	public bool isInteractiveDisabled { get; set; }
	public SpriteRenderer spriteRenderer { get; set; }
	public CoreInteractableType interactableType
	{
		get { return CoreInteractableType.Nodes; }
	}

	public Card getCard()
	{
		return null;
	}

	public BaseCard getBaseCard()
	{
		return null;
	}

	public CardCollapsed getCollapsedCard()
	{
		return null;
	}

	// -------------------- Custom Class -------------------------
	[System.NonSerialized]
	public NodeCardQue nodeCardQue;

	[System.NonSerialized]
	public NodeTextHandler nodeTextHandler;

	[System.NonSerialized]
	public NodeStats nodeStats;

	[System.NonSerialized]
	public NodeProcess nodeProcess;

	[System.NonSerialized]
	public NodeHungerHandler nodeHungerHandler;

	public NodePlaneHandler nodePlaneManager { get; set; }

	public NodeMagnetizeCircle nodeMagnetizeCircle;

	public InteractableManagerScriptableObject interactableManagerScriptableObject;

	public StaticVariables staticVariables;

	// -------------------- Node Stats -------------------------

	public CardStack processCardStack { get; set; }

	public int _id;

	public int id
	{
		get { return _id; }
		set
		{
			_id = value;
			if (nodeHungerHandler != null)
			{
				nodeHungerHandler.intervalTimer = 0;
			}
			nodeStats = new NodeStats(this);
		}
	}

	public bool isActive { get; set; }

	// ---------------------------------------------------------

	private void Awake()
	{
		processCardStack = new CardStack(this);
		processCardStack.originPointAdjustment = new Vector3(0f, 35f, 0);

		nodeTextHandler = new NodeTextHandler(this);

		isInteractiveDisabled = false;
		isActive = true;
	}

	private void FixedUpdate()
	{
		nodeStats.computeStats();
		nodeStats.handleLimits();
		nodeTextHandler.reflectToScreen();
	}

	public void init(NodePlaneHandler nodePlane, NodeMagnetizeCircle _nodeMagnetizeCircle)
	{
		nodePlaneManager = nodePlane;

		nodeMagnetizeCircle = _nodeMagnetizeCircle;

		nodeCardQue = gameObject.GetComponent(typeof(NodeCardQue)) as NodeCardQue;

		spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;

		nodeProcess = gameObject.GetComponent(typeof(NodeProcess)) as NodeProcess;
		nodeProcess.init(this);

		nodeHungerHandler = gameObject.GetComponent(typeof(NodeHungerHandler)) as NodeHungerHandler;
		nodeHungerHandler.init(this);
	}

	public void OnClick()
	{
		if (nodePlaneManager.gameObject.activeSelf == true)
		{
			nodePlaneManager.gameObject.SetActive(false);
		}
		else
		{
			nodePlaneManager.gameObject.SetActive(true);
		}
	}

	public void killNode()
	{
		List<BaseCard> allCards = new List<BaseCard>(processCardStack.cards);
		ejectCards(allCards);

		interactableManagerScriptableObject.removeNode(this);

		Destroy(nodePlaneManager.gameObject);
		Destroy(gameObject);
	}

	public void stackOnThis(BaseCard newCard, Node prevNode)
	{
		if (isMarket())
		{
			if (!CardDictionary.globalCardDictionary[newCard.id].isSellable)
			{
				return;
			}

			if (prevNode != this)
			{
				nodeCardQue.addCard(newCard);
			}
			processCardStack.addCardsToStack(new List<BaseCard>() { newCard });
			return;
		}
		if (
			CardDictionary.globalCardDictionary[newCard.id].resourceInventoryCount + nodeStats.currentNodeStats.resourceInventoryUsed
			> nodeStats.currentNodeStats.resourceInventoryLimit
		)
		{
			return;
		}

		if (
			CardDictionary.globalCardDictionary[newCard.id].infraInventoryCount + nodeStats.currentNodeStats.infraInventoryUsed
			> nodeStats.currentNodeStats.infraInventoryLimit
		)
		{
			return;
		}

		if (prevNode != this)
		{
			nodeCardQue.addCard(newCard);
		}
		processCardStack.addCardsToStack(new List<BaseCard>() { newCard });
	}

	public void ejectCards(List<BaseCard> cards)
	{
		Vector3 basePosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 15, HelperData.draggingBaseZ);

		processCardStack.removeCardsFromStack(cards);

		for (int index = 0; index < cards.Count; index++)
		{
			cards[index].moveCard(basePosition);
			cards[index].isInteractiveDisabled = false;
		}

		StartCoroutine(delayedDragFinish(cards));
	}

	public IEnumerator delayedDragFinish(List<BaseCard> cards)
	{
		if (LeftClickHandler.current != null)
		{
			for (int index = 0; index < cards.Count; index++)
			{
				yield return null;
				if (cards[index] != null)
				{
					LeftClickHandler.current.dragFinishHandler(new List<Interactable>() { cards[index] }, this);
				}
			}
		}
	}

	public bool isMarket()
	{
		if (id == 3003)
		{
			return true;
		}
		return false;
	}
}
