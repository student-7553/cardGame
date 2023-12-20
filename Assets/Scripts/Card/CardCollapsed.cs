using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Core;
using System.Linq;
using Helpers;

public class CardCollapsed : BaseCard, CardHolder, IClickable
{
	public SO_Interactable so_Interactable;
	public SO_PlayerRuntime playerRuntime;

	List<BaseCard> cards = new List<BaseCard>();

	public TextMeshPro titleTextMesh;

	public TextMeshPro collapsedCountTextMesh;

	// public SpriteRenderer mainBodySpriteRenderer;
	// public SpriteRenderer borderSpriteRenderer;

	public CardCollapsedPlaneHandler cardCollapsedPlaneHandler;

	public override CardCollapsed getCollapsedCard()
	{
		return this;
	}

	public override CoreInteractableType interactableType
	{
		get { return CoreInteractableType.CollapsedCards; }
	}

	// -------------------- START Clickable Members -------------------------
	public void OnClick()
	{
		cardCollapsedPlaneHandler.gameObject.SetActive(true);
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

	private void Awake()
	{
		// mainBodySpriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
		computeCorners();
	}

	private void FixedUpdate()
	{
		reflectScreen();
	}

	public override void stackOnThis(BaseCard draggingCard, Node _prevNode)
	{
		if (draggingCard.id == id)
		{
			addCardsToStack(new List<BaseCard>() { draggingCard });
			return;
		}

		if (isStacked())
		{
			joinedStack.addCardsToStack(new List<BaseCard>() { draggingCard });
			return;
		}

		List<BaseCard> newCardStackCards = new List<BaseCard> { this, draggingCard };
		CardStack newStack = new CardStack(null);
		newStack.addCardsToStack(newCardStackCards);
	}

	public override void destroyCard()
	{
		if (!gameObject)
		{
			return;
		}

		if (joinedStack != null)
		{
			joinedStack.removeCardsFromStack(new List<BaseCard>() { this });
			joinedStack = null;
		}

		Destroy(gameObject);
	}

	public void reflectScreen()
	{
		if (titleTextMesh == null)
		{
			return;
		}

		string cardTitle = CardDictionary.globalCardDictionary[id].name;

		// if (mainBodySpriteRenderer.color.a != 1f)
		// {
		// 	mainBodySpriteRenderer.color = new Color(
		// 		mainBodySpriteRenderer.color.r,
		// 		mainBodySpriteRenderer.color.g,
		// 		mainBodySpriteRenderer.color.b,
		// 		1f
		// 	);
		// }

		if (isInteractiveDisabled && cardDisable != null)
		{
			string disabledTitle = "[DISABLED] ";
			disabledTitle = disabledTitle + $"[{cardDisable}]";
			if (cardDisable == CardDisableType.AutoMoving)
			{
				// mainBodySpriteRenderer.color = new Color(
				// 	mainBodySpriteRenderer.color.r,
				// 	mainBodySpriteRenderer.color.g,
				// 	mainBodySpriteRenderer.color.b,
				// 	0.3f
				// );
			}
			cardTitle = disabledTitle + cardTitle;
		}

		titleTextMesh.text = cardTitle;
		collapsedCountTextMesh.text = $"{cards.Count}";
	}

	//---------------- START CardHolder ------------

	public void removeCardsFromStack(List<BaseCard> removingCards)
	{
		// Good to have some checks here tho
		// Will always be Cards
		bool changed = false;
		foreach (BaseCard singleCard in removingCards)
		{
			bool isIncluded = cards.Any((card) => singleCard.GetInstanceID() == card.GetInstanceID());
			if (!isIncluded)
			{
				continue;
			}

			changed = true;
			cards.Remove(singleCard);

			singleCard.gameObject.transform.SetParent(null);
			singleCard.gameObject.SetActive(true);
			singleCard.joinedStack = null;
		}
		if (changed)
		{
			deadCheck();
		}
	}

	private void deadCheck()
	{
		if (cards.Count > 1)
		{
			return;
		}

		bool isStackedCurrently = isStacked();
		CardHolder preJoinedStack = joinedStack;
		if (isStackedCurrently)
		{
			preJoinedStack.removeCardsFromStack(new List<BaseCard>() { this });
		}

		if (cards.Count == 1)
		{
			BaseCard lastCard = cards[0];

			lastCard.gameObject.transform.SetParent(null);
			lastCard.gameObject.SetActive(true);
			lastCard.joinedStack = null;

			if (isStackedCurrently)
			{
				preJoinedStack.addCardsToStack(new List<BaseCard>() { lastCard });
			}
			else
			{
				if (LeftClickHandler.current != null)
				{
					StartCoroutine(delayedDragFinish(lastCard));
				}
			}
		}

		Destroy(gameObject);
	}

	public IEnumerator delayedDragFinish(BaseCard card)
	{
		Vector3 basePosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, HelperData.draggingBaseZ);
		card.moveCard(basePosition);
		card.isInteractiveDisabled = false;
		yield return null;
		if (card != null)
		{
			LeftClickHandler.current.dragFinishHandler(new List<Interactable>() { card }, null);
		}
	}

	public void addCardsToStack(List<BaseCard> addingCards)
	{
		foreach (BaseCard baseCard in addingCards)
		{
			if (baseCard.id != id)
			{
				stackOnThis(baseCard, null);
				continue;
			}

			if (baseCard.interactableType == CoreInteractableType.Cards)
			{
				cards.Add(baseCard);
			}
			else if (baseCard.interactableType == CoreInteractableType.CollapsedCards)
			{
				CardCollapsed cardCollapsed = baseCard.getCollapsedCard();
				List<BaseCard> cardCollapsedCards = new List<BaseCard>(cardCollapsed.getCards());
				cardCollapsed.removeCardsFromStack(cardCollapsedCards);
				cards.AddRange(cardCollapsedCards);
				cardCollapsed.destroyCard();
			}
		}

		foreach (BaseCard singleCard in cards)
		{
			singleCard.gameObject.transform.SetParent(gameObject.transform);
			singleCard.gameObject.SetActive(false);
			singleCard.attachToCardHolder(this);
		}
	}

	public CardStackType getCardHolderType()
	{
		return CardStackType.CollapsedCards;
	}

	public BaseNode getNode()
	{
		return joinedStack?.getNode();
	}

	public List<BaseCard> getCards()
	{
		return cards;
	}

	public List<BaseCard> getActiveCards()
	{
		List<BaseCard> activeCards = cards.Where((card) => !card.isInteractiveDisabled).ToList();
		return activeCards;
	}
	//---------------- END CardHolder ------------
}
