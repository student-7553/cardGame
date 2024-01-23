using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using Core;
using Helpers;

public class EnemyNode : MonoBehaviour, BaseNode
{
	// -------------------- Custom Class ------------------------

	public Vector3 currentVelocity;

	public virtual Interactable[] getMouseHoldInteractables()
	{
		Interactable[] interactables = { this };
		return interactables;
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

	public bool isCardType()
	{
		return false;
	}

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

	public SO_Interactable so_Interactable;

	// -------------------- Node Stats -------------------------


	public CardStack processCardStack { get; set; }

	private void Awake()
	{
		processCardStack = new CardStack(this) { originPointAdjustment = new Vector3(0f, 14.5f, 0) };
		isActive = true;

		enemyNodeTextHandler = new EnemyNodeTextHandler(this);
	}

	private int _id;
	public int id
	{
		get { return _id; }
		set { _id = value; }
	}

	[SerializeField]
	private SpriteRenderer shadowSpriteRenderer;

	public void setSpriteHovering(bool isHovering, Interactable.SpriteInteractable targetSprite)
	{
		if (targetSprite == Interactable.SpriteInteractable.hover)
		{
			if (shadowSpriteRenderer == null)
			{
				return;
			}
			Vector3 newScale = isHovering
				? shadowSpriteRenderer.transform.localScale * 1.075f
				: shadowSpriteRenderer.transform.localScale / 1.075f;
			shadowSpriteRenderer.transform.localScale = newScale;
		}
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

	public void stackOnThis(BaseCard newCard, Node prevNode)
	{
		processCardStack.addCardsToStack(new List<BaseCard>() { newCard });
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
		float totalFighterValue = processCardStack
			.getAllCardIds()
			.Aggregate(
				0,
				(totalFighterValue, next) =>
				{
					return totalFighterValue + CardDictionary.globalCardDictionary[next].typeValue;
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
		foreach (Node node in so_Interactable.nodes)
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

	// ---------------------------------------------------------
}
