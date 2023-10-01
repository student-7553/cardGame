using UnityEngine;
using System.Collections.Generic;
using Core;
using TMPro;
using Helpers;

public class Card : BaseCard, SelfBaseCardInterface
{
	public override Card getCard()
	{
		return this;
	}

	// -------------------- CardInterface Members -------------------------


	private CardHolder _joinedStack;

	public override CardHolder joinedStack
	{
		get { return _joinedStack; }
		set
		{
			if (value == null && _joinedStack != null)
			{
				_joinedStack.removeCardsFromStack(new List<BaseCard>() { this });

				gameObject.SetActive(true);
				gameObject.transform.SetParent(null);
			}
			_joinedStack = value;
		}
	}

	// -------------------- CardInterface Members end -------------------------

	private TextMeshPro titleTextMesh;
	private SpriteRenderer spriteRenderer;
	public InteractableManagerScriptableObject interactableManagerScriptableObject;

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

	public override void destroyCard()
	{
		if (!gameObject)
		{
			return;
		}
		joinedStack = null;
		interactableManagerScriptableObject.removeCard(this);
		Destroy(gameObject);
	}

	public override void stackOnThis(BaseCard draggingCard, Node _prevNode)
	{
		if (isStacked())
		{
			joinedStack.addCardsToStack(new List<BaseCard>() { draggingCard });
		}
		else
		{
			List<BaseCard> newCardStackCards = new List<BaseCard> { this, draggingCard };
			CardStack newStack = new CardStack(null);
			newStack.addCardsToStack(newCardStackCards);
		}
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
