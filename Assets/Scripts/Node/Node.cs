using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Core;
using System.Linq;
using Helpers;
using DG.Tweening;
using System;

public class Node : MonoBehaviour, BaseNode
{
	public Vector3 currentVelocity;

	public bool isCardType()
	{
		return false;
	}

	public virtual Interactable[] getMouseHoldInteractables()
	{
		Interactable[] interactables = { this };
		return interactables;
	}

	// -------------------- Interactable Members -------------------------
	public bool isInteractiveDisabled { get; set; }

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

	public ref Vector3 getCurrentVelocity()
	{
		return ref currentVelocity;
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

	public SO_Interactable so_Interactable;

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
		processCardStack = new CardStack(this) { originPointAdjustment = new Vector3(0f, 35f, 0) };

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

		so_Interactable.removeNode(this);

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

			processCardStack.addCardsToStack(new List<BaseCard>() { newCard });
			return;
		}

		bool isAllowed = isAllowedToStack(newCard);

		if (!isAllowed)
		{
			return;
		}

		processCardStack.addCardsToStack(new List<BaseCard>() { newCard });
	}

	public void ejectCards(List<BaseCard> cards)
	{
		processCardStack.removeCardsFromStack(cards);
		if (LeftClickHandler.current == null)
		{
			return;
		}
		// StartCoroutine(delayedDragFinish(cards));
		delayedDragFinish(cards);
	}

	// public IEnumerator delayedDragFinish(List<BaseCard> cards)
	public void delayedDragFinish(List<BaseCard> cards)
	{
		Vector3 basePosition = new Vector3(
			gameObject.transform.position.x,
			gameObject.transform.position.y - staticVariables.nodeEjectDistance,
			HelperData.draggingBaseZ
		);
		List<BaseCard> clonedCards = new List<BaseCard>(cards);
		BaseCard subjectCard = clonedCards[0];
		subjectCard.cardDisable = null;
		subjectCard.moveCard(basePosition);

		Vector3 smoothBasePsosition = new Vector3(basePosition.x, basePosition.y - staticVariables.nodeEjectSlideDistance, basePosition.z);

		subjectCard.gameObject.transform
			.DOMove(smoothBasePsosition, staticVariables.nodeEjectSlideTime)
			.OnComplete(() =>
			{
				if (subjectCard != null)
				{
					foreach (Interactable interactable in clonedCards)
					{
						interactable.gameObject.transform.position = subjectCard.transform.position;
						interactable.isInteractiveDisabled = false;
					}

					LeftClickHandler.current.dragFinishHandler(new List<Interactable>(clonedCards), null);
				}
			});
	}

	public bool isMarket()
	{
		if (id == 3003)
		{
			return true;
		}
		return false;
	}

	private bool isAllowedToStack(BaseCard newCard)
	{
		int resourceInventoryCount = 0;
		int infraInventoryCount = 0;
		if (newCard.interactableType == CoreInteractableType.CollapsedCards)
		{
			List<int> cardIds = newCard.getCollapsedCard().getCards().Select((card) => card.id).ToList();
			foreach (int cardId in cardIds)
			{
				resourceInventoryCount = resourceInventoryCount + CardDictionary.globalCardDictionary[newCard.id].resourceInventoryCount;
				infraInventoryCount = infraInventoryCount + CardDictionary.globalCardDictionary[newCard.id].infraInventoryCount;
			}
		}
		else
		{
			resourceInventoryCount = CardDictionary.globalCardDictionary[newCard.id].resourceInventoryCount;
			infraInventoryCount = CardDictionary.globalCardDictionary[newCard.id].infraInventoryCount;
		}

		if (resourceInventoryCount + nodeStats.currentNodeStats.resourceInventoryUsed > nodeStats.currentNodeStats.resourceInventoryLimit)
		{
			return false;
		}

		if (infraInventoryCount + nodeStats.currentNodeStats.infraInventoryUsed > nodeStats.currentNodeStats.infraInventoryLimit)
		{
			return false;
		}

		return true;
	}
}
