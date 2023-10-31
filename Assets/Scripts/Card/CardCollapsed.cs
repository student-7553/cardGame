using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Core;
using System.Linq;
using Helpers;
using Unity.VisualScripting;

public class CardCollapsed : BaseCard, CardHolder, IClickable
{
	List<BaseCard> cards = new List<BaseCard>();

	private SpriteRenderer spriteRenderer;

	private TextMeshPro titleTextMesh;

	public InteractableManagerScriptableObject interactableManagerScriptableObject;

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

		string cardTitle = "";
		if (CardDictionary.globalCardDictionary.ContainsKey(id))
		{
			cardTitle = cardTitle + CardDictionary.globalCardDictionary[id].name + $"[Collapsed {cards.Count}]";
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

		titleTextMesh.text = cardTitle;
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

		Debug.Log("CardCollapsed is dieing");
		if (cards.Count == 1)
		{
			BaseCard lastCard = cards[0];

			lastCard.gameObject.transform.SetParent(null);
			lastCard.gameObject.SetActive(true);
			lastCard.joinedStack = null;

			// cards.Clear();

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
