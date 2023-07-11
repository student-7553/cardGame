using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using Core;

public class EnemyNode : MonoBehaviour, BaseNode
{
	// -------------------- Custom Class ------------------------

	public bool isInteractiveDisabled { get; set; }
	public SpriteRenderer spriteRenderer { get; set; }
	public CoreInteractableType interactableType
	{
		get { return CoreInteractableType.Nodes; }
	}

	public NodePlaneHandler nodePlaneManager { get; set; }

	[System.NonSerialized]
	public EnemyNodeTextHandler enemyNodeTextHandler;

	private float detonationTime;

	public float proccessingLeft;

	public int powerValue;

	private LayerMask interactableLayerMask;

	public InteractableManagerScriptableObject interactableManagerScriptableObject;

	// -------------------- Node Stats -------------------------


	public CardStack processCardStack { get; set; }

	private void Awake()
	{
		processCardStack = new CardStack(this);
		processCardStack.originPointAdjustment = new Vector3(0f, 35f, 0);

		isActive = true;
		interactableLayerMask = LayerMask.GetMask("Interactable");

		enemyNodeTextHandler = new EnemyNodeTextHandler(this);
	}

	private int _id;
	public int id
	{
		get { return _id; }
		set { _id = value; }
	}

	private bool _isActive;
	public bool isActive
	{
		get { return _isActive; }
		set
		{
			if (!value && _isActive)
			{
				Destroy(nodePlaneManager.gameObject);
				Destroy(this.gameObject);
			}
			_isActive = value;
		}
	}

	public void stackOnThis(Card newCard, Node prevNode)
	{
		processCardStack.addCardToStack(newCard);
		this.checkIfDead();
	}

	private void checkIfDead()
	{
		float currentTotalFighterValue = this.getCurrentFigherValue();
		if (currentTotalFighterValue >= CardDictionary.globalCardDictionary[id].typeValue)
		{
			this.killNode();
			// dead
		}
	}

	private float getCurrentFigherValue()
	{
		float totalFighterValue = processCardStack.cards.Aggregate(
			0,
			(totalFighterValue, next) =>
			{
				return totalFighterValue + CardDictionary.globalCardDictionary[next.id].typeValue;
			}
		);
		return totalFighterValue;
	}

	public void init(NodePlaneHandler nodePlane, float detonationTime)
	{
		nodePlaneManager = nodePlane;
		this.proccessingLeft = detonationTime;
		this.powerValue = CardDictionary.globalCardDictionary[id].typeValue;

		StartCoroutine(enemyNodeDetonation());
	}

	public IEnumerator enemyNodeDetonation()
	{
		while (this.proccessingLeft > 0)
		{
			yield return new WaitForSeconds(1);
			this.proccessingLeft = this.proccessingLeft - 1f;
		}

		if (isActive)
		{
			Node nearestNode = this.getNearestNode();
			if (nearestNode)
			{
				nearestNode.killNode();
			}

			isActive = false;
		}
	}

	private Node getNearestNode()
	{
		Node closestNode = null;
		Vector2 currentNodePosition = gameObject.transform.position;
		float currentNodeDistance = 0;
		foreach (Node node in interactableManagerScriptableObject.nodes)
		{
			if (closestNode == null)
			{
				closestNode = node;
				currentNodeDistance = this.getDistanceBetweenTwoPoints(node.transform.position, currentNodePosition);
				continue;
			}

			float distance = this.getDistanceBetweenTwoPoints(node.transform.position, currentNodePosition);
			if (distance < currentNodeDistance)
			{
				closestNode = node;
				currentNodeDistance = distance;
			}
		}
		return closestNode;
	}

	private float getDistanceBetweenTwoPoints(Vector2 pos1, Vector2 pos2)
	{
		float a = pos1.x - pos2.x;
		float b = pos1.y - pos2.y;
		float distance = Mathf.Sqrt(a * a + b * b);
		return distance;
	}

	private void FixedUpdate()
	{
		enemyNodeTextHandler.reflectToScreen();
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
		// List<Card> allCards = new List<Card>(processCardStack.cards);
		// this.ejectCards(allCards);

		Destroy(nodePlaneManager.gameObject);
		Destroy(this.gameObject);
	}

	public Card getCard()
	{
		return null;
	}

	// ---------------------------------------------------------
}
