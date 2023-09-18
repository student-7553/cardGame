using UnityEngine;
using System.Collections.Generic;
using Core;
using TMPro;

public class CardCollapsed : BaseCard, IStackable, SelfBaseCardInterface
{
	List<Card> collpasedCards = new List<Card>();

	private SpriteRenderer spriteRenderer;

	private TextMeshPro titleTextMesh;

	public InteractableManagerScriptableObject interactableManagerScriptableObject;

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
					// Todo
					// joinedStack.removeCardsFromStack(new List<Card>() { this });
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

	public new CardCollapsed getCardCollapsed()
	{
		return this;
	}

	public void stackOnThis(Card draggingCard, Node _prevNode)
	{
		// Check if same card
		if (collpasedCards.Count != 0 && draggingCard.id == collpasedCards[0].id)
		{
			addCardToCollapsed(draggingCard);
			return;
		}

		if (isStacked)
		{
			CardStack existingstack = joinedStack;
			existingstack.addCardToStack(draggingCard);
			return;
		}

		// Todo
		// List<Card> newCardStackCards = new List<Card> { this, draggingCard };
		// CardStack newStack = new CardStack(null);
		// newStack.addCardToStack(newCardStackCards);
	}

	public void destroyCard()
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

	private void addCardToCollapsed(Card newCard)
	{
		collpasedCards.Add(newCard);
		newCard.gameObject.SetActive(false);
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
