using UnityEngine;
using System.Collections;

public class EnemyNode : MonoBehaviour, BaseNode
{
	// -------------------- Custom Class ------------------------

	public NodePlaneHandler nodePlaneManager { get; set; }

	[System.NonSerialized]
	public EnemyNodeTextHandler enemyNodeTextHandler;

	private readonly float detonationTime = 5f;

	public float proccessingLeft;

	public int powerValue;

	private LayerMask interactableLayerMask;

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
	}

	public void init(NodePlaneHandler nodePlane)
	{
		nodePlaneManager = nodePlane;
		this.proccessingLeft = detonationTime;
		this.powerValue = CardDictionary.globalCardDictionary[id].typeValue;

		StartCoroutine(enemyNodeDetonation());

		// process
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
		// god help this logic
		// Prob need to change to read from a global variable

		Collider2D[] hits = Physics2D.OverlapCircleAll(this.transform.position, 35, interactableLayerMask, 0f, 0f);
		if (hits == null || hits.Length == 0)
		{
			return null;
		}

		foreach (Collider2D singleHit in hits)
		{
			Node interactableObject = singleHit.GetComponent(typeof(Node)) as Node;
			if (interactableObject != null)
			{
				return interactableObject;
			}
		}
		return null;
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

	// ---------------------------------------------------------
}
