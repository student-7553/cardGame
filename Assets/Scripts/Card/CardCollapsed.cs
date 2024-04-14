using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Core;
using System.Linq;
using Helpers;
using DG.Tweening;

public class CardCollapsed : BaseCard, CardHolder, IClickable
{
	private static float stackDistance = 5;
	private static float zDistancePerCards = 0.01f;

	private int topVisibleCards;

	public SO_Interactable so_Interactable;
	public SO_PlayerRuntime playerRuntime;

	List<BaseCard> cards = new List<BaseCard>();

	public TextMeshPro collapsedCountTextMesh;

	public SO_Audio soAudio;

	public CardCollapsedPlaneHandler cardCollapsedPlaneHandler;

	public GameObject dimObject;

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
		soAudio.cardClickAudioAction?.Invoke();

		if (cardCollapsedPlaneHandler.gameObject.activeSelf == true)
		{
			cardCollapsedPlaneHandler.gameObject.SetActive(false);
		}
		else
		{
			cardCollapsedPlaneHandler.gameObject.SetActive(true);
		}

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
		CardStack newStack = new CardStack(null, 0, Vector3.zero);
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

		if (isInteractiveDisabled && cardDisable != null)
		{
			string disabledTitle = "[Disabled] ";
			disabledTitle = disabledTitle + $"[{cardDisable}]";
			cardTitle = disabledTitle + cardTitle;
		}

		int fontSize = getFontSize(cardTitle);

		titleTextMesh.fontSize = fontSize;
		titleTextMesh.text = cardTitle;

		collapsedCountTextMesh.text = $"{cards.Count}";
	}

	private int getFontSize(string title)
	{
		if (title.Length > 12)
		{
			return 13;
		}
		return 18;
	}

	public void dimCard()
	{
		dimObject.SetActive(true);
	}

	public void nonDimCard()
	{
		dimObject.SetActive(false);
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
		alignCards();
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

	private IEnumerator delayedDragFinish(BaseCard card)
	{
		Vector3 basePosition = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, HelperData.draggingBaseZ);
		card.moveCard(basePosition);
		card.isInteractiveDisabled = false;
		yield return null;
		if (card != null)
		{
			LeftClickHandler.current.handleCardDrop(new List<Interactable>() { card }, null);
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
			// singleCard.gameObject.transform.SetParent(gameObject.transform);
			// singleCard.gameObject.SetActive(false);
			// singleCard.attachToCardHolder(this);

			singleCard.gameObject.transform.SetParent(cardCollapsedPlaneHandler.gameObject.transform);
			// singleCard.gameObject.SetActive(true);
			singleCard.attachToCardHolder(this);
		}
		alignCards();
	}

	public void alignCards()
	{
		if (cards.Count == 0)
		{
			return;
		}

		Vector3 adjustedOriginPoint = new Vector3(
			cardCollapsedPlaneHandler.gameObject.transform.position.x,
			cardCollapsedPlaneHandler.gameObject.transform.position.y,
			HelperData.nodeBoardZ - 1
		);

		adjustedOriginPoint = adjustedOriginPoint + new Vector3(0, 14.5f, 0);

		float paddingCounter = 0;
		for (int index = 0; index < cards.Count; index++)
		{
			BaseCard singleCard = cards[index];
			if (singleCard == null)
			{
				continue;
			}

			Vector3 newPostionForCardInSubject = new Vector3(adjustedOriginPoint.x, adjustedOriginPoint.y, HelperData.nodeBoardZ - 1);
			newPostionForCardInSubject.y = newPostionForCardInSubject.y - (paddingCounter * stackDistance);
			newPostionForCardInSubject.z = newPostionForCardInSubject.z - (paddingCounter * zDistancePerCards);

			paddingCounter++;

			moveBaseCard(singleCard, newPostionForCardInSubject);
			singleCard.computeCorners();

			if (topVisibleCards != 0)
			{
				if (index >= topVisibleCards)
				{
					singleCard.gameObject.SetActive(false);
				}
				else
				{
					singleCard.gameObject.SetActive(true);
				}
			}
		}
	}

	private void moveBaseCard(BaseCard card, Vector3 newPosition)
	{
		card.gameObject.transform.DOMove(newPosition, HelperData.cardReachSmoothTime).SetId(card.id);
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
