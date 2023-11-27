using UnityEngine;
using System.Collections.Generic;
using Core;
using TMPro;
using Helpers;

public class Card : BaseCard, IClickable
{
	public SO_PlayerRuntime playerRuntime;

	public override Card getCard()
	{
		return this;
	}

	// -------------------- START Clickable Members -------------------------
	public void OnClick()
	{
		playerRuntime.changePlayerFocusingCardId(id);
	}

	// -------------------- END Clickable Members -------------------------

	// -------------------- CardInterface Members -------------------------


	private CardHolder _joinedStack;

	public override CardHolder joinedStack
	{
		get { return _joinedStack; }
		set { _joinedStack = value; }
	}

	// -------------------- CardInterface Members end -------------------------

	private TextMeshPro titleTextMesh;
	private TextMeshPro typeTextMesh;
	private SpriteRenderer spriteRenderer;
	public SO_Interactable so_Interactable;

	private void Awake()
	{
		TextMeshPro[] textMeshes = gameObject.GetComponentsInChildren<TextMeshPro>();

		titleTextMesh = textMeshes[0];
		typeTextMesh = textMeshes[1];

		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		computeCorners();
	}

	private void FixedUpdate()
	{
		reflectScreen();
	}

	public override void destroyCard()
	{
		if (gameObject == null)
		{
			return;
		}
		if (joinedStack != null)
		{
			joinedStack.removeCardsFromStack(new List<BaseCard>() { this });
			joinedStack = null;
		}

		so_Interactable.removeCard(this);
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
		if (titleTextMesh == null)
		{
			return;
		}

		string cardTitle = "";
		if (CardDictionary.globalCardDictionary.ContainsKey(id))
		{
			cardTitle = cardTitle + CardDictionary.globalCardDictionary[id].name;
		}

		if (spriteRenderer.color.a != 1f)
		{
			spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
		}

		if (isInteractiveDisabled && cardDisable != null)
		{
			string disabledTitle = "[DISABLED] ";

			disabledTitle = disabledTitle + $"[{cardDisable}]";
			if (cardDisable == CardDisableType.AutoMoving)
			{
				spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.3f);
			}
			cardTitle = disabledTitle + cardTitle;
		}

		titleTextMesh.text = cardTitle;

		typeTextMesh.text = CardDictionary.globalCardDictionary[id].type.ToString();
	}
}
