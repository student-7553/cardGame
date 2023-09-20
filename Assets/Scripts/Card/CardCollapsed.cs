using UnityEngine;
using System.Collections.Generic;
using TMPro;

// public class CardCollapsed : BaseCard, IStackable, SelfBaseCardInterface
public class CardCollapsed : BaseCard, SelfBaseCardInterface
{
	List<BaseCard> collpasedCards = new List<BaseCard>();

	private SpriteRenderer spriteRenderer;

	private TextMeshPro titleTextMesh;

	public InteractableManagerScriptableObject interactableManagerScriptableObject;

	public override CardCollapsed getCollapsedCard()
	{
		return this;
	}

	// -------------------- CardInterface Members -------------------------
	public new bool isStacked
	{
		get { return _isStacked; }
		set
		{
			bool preValue = _isStacked;
			_isStacked = value;

			if (value == false && preValue == true)
			{
				if (joinedStack != null)
				{
					joinedStack.removeCardsFromStack(new List<BaseCard>() { this });
					joinedStack = null;
				}
				gameObject.SetActive(true);
				gameObject.transform.SetParent(null);
			}
		}
	}

	// -------------------- CardInterface Members end -------------------------

	private void Awake()
	{
		Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));
		if (textMeshes.Length > 0)
		{
			titleTextMesh = textMeshes[0] as TextMeshPro;
		}

		spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
		computeCorners();
	}

	private void FixedUpdate()
	{
		reflectScreen();
	}

	public void addToCollapsedCards(List<BaseCard> baseCards)
	{
		foreach (BaseCard baseCard in baseCards)
		{
			addToCollapsedCards(baseCard);
		}
	}

	public void addToCollapsedCards(BaseCard newCard)
	{
		collpasedCards.Add(newCard);
		newCard.gameObject.SetActive(false);
	}

	public override void stackOnThis(BaseCard draggingCard, Node _prevNode)
	{
		// Check if same card
		if (collpasedCards.Count != 0 && draggingCard.id == collpasedCards[0].id)
		{
			addToCollapsedCards(draggingCard);
			return;
		}

		if (isStacked)
		{
			CardStack existingstack = joinedStack;
			existingstack.addCardToStack(draggingCard);
			return;
		}

		List<BaseCard> newCardStackCards = new List<BaseCard> { this, draggingCard };
		CardStack newStack = new CardStack(null);
		newStack.addCardToStack(newCardStackCards);
	}

	public override void destroyCard()
	{
		if (!gameObject)
		{
			return;
		}
		if (isStacked)
		{
			isStacked = false;
		}

		foreach (Card singleCard in collpasedCards)
		{
			interactableManagerScriptableObject.removeCard(singleCard);
		}
		Destroy(gameObject);
	}

	public void reflectScreen()
	{
		string cardTitle = "";
		if (CardDictionary.globalCardDictionary.ContainsKey(id))
		{
			if (titleTextMesh != null)
			{
				cardTitle = cardTitle + CardDictionary.globalCardDictionary[id].name;
			}
		}

		if (spriteRenderer.color.a != 1f)
		{
			spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
		}

		if (isInteractiveDisabled)
		{
			string disabledTitle = "[DISABLED] ";

			if (cardDisable != null)
			{
				disabledTitle = disabledTitle + $"[{cardDisable}]";
				if (cardDisable == CardDisableType.AutoMoving)
				{
					spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.3f);
				}
			}
			cardTitle = disabledTitle + cardTitle;
		}

		if (titleTextMesh != null)
		{
			titleTextMesh.text = cardTitle;
		}
	}
}
