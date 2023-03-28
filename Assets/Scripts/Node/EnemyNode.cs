using UnityEngine;
using System.Collections.Generic;
using Core;
using System.Linq;
using Helpers;

public class EnemyNode : MonoBehaviour, BaseNode
{
	// -------------------- Custom Class ------------------------

	public NodePlaneHandler nodePlaneManager { get; set; }

	[System.NonSerialized]
	public EnemyNodeTextHandler enemyNodeTextHandler;

	// -------------------- Node Stats -------------------------


	public CardStack processCardStack { get; set; }

	private int _id;
	public int id
	{
		get { return _id; }
		set { _id = value; }
	}

	public bool isActive { get; set; }

	public void stackOnThis(Card newCard, Node prevNode)
	{
		processCardStack.addCardToStack(newCard);
	}

	public void init(NodePlaneHandler nodePlane)
	{
		nodePlaneManager = nodePlane;
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

	private void Awake()
	{
		processCardStack = new CardStack(this);
		processCardStack.originPointAdjustment = new Vector3(0f, 35f, 0);

		isActive = true;

		enemyNodeTextHandler = new EnemyNodeTextHandler(this);
	}

	// ---------------------------------------------------------
}
