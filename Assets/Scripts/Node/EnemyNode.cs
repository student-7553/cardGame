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

	[NonSerialized]
	public EnemyNodeTextHandler enemyNodeTextHandler;

	public float proccessingLeft;

	public int powerValue;

	public InteractableManagerScriptableObject interactableManagerScriptableObject;

	// -------------------- Node Stats -------------------------


	public CardStack processCardStack { get; set; }

	private void Awake()
	{
		processCardStack = new CardStack(this);
		processCardStack.originPointAdjustment = new Vector3(0f, 35f, 0);

		isActive = true;

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
				Destroy(gameObject);
			}
			_isActive = value;
		}
	}

	public void stackOnThis(Card newCard, Node prevNode)
	{
		processCardStack.addCardToStack(newCard);
		checkIfDead();
	}

	private void checkIfDead()
	{
		float currentTotalFighterValue = getCurrentFigherValue();
		if (currentTotalFighterValue >= CardDictionary.globalCardDictionary[id].typeValue)
		{
			killNode();
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
		proccessingLeft = detonationTime;
		powerValue = CardDictionary.globalCardDictionary[id].typeValue;

		StartCoroutine(enemyNodeDetonation());
	}

	public IEnumerator enemyNodeDetonation()
	{
		while (proccessingLeft > 0)
		{
			yield return new WaitForSeconds(1);
			proccessingLeft = proccessingLeft - 1f;
		}

		if (isActive)
		{
			Node nearestNode = getNearestNode();
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
			if (closestNode == null && !node.isMarket())
			{
				closestNode = node;
				currentNodeDistance = getDistanceBetweenTwoPoints(node.transform.position, currentNodePosition);
				continue;
			}

			float distance = getDistanceBetweenTwoPoints(node.transform.position, currentNodePosition);
			if (distance < currentNodeDistance && !node.isMarket())
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
		Destroy(nodePlaneManager.gameObject);
		Destroy(gameObject);
	}

	public Card getCard()
	{
		return null;
	}

	// ---------------------------------------------------------
}
