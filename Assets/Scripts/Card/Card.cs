using UnityEngine;
using System.Collections.Generic;
using Core;

public class Card : BaseCard, IClickable
{
	public SO_PlayerRuntime playerRuntime;
	public SO_Interactable so_Interactable;
	public SO_Audio soAudio;

	public GameObject dimObject;

	public override Card getCard()
	{
		return this;
	}

	// -------------------- START Clickable Members -------------------------
	public void OnClick()
	{
		soAudio.cardClickAudioAction?.Invoke();
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

	// public TextMeshPro titleTextMesh;

	private void Awake()
	{
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
			CardStack newStack = new CardStack(null, 0, Vector3.zero);
			newStack.addCardsToStack(newCardStackCards);
		}
	}

	public void dimCard()
	{
		dimObject.SetActive(true);
	}

	public void nonDimCard()
	{
		dimObject.SetActive(false);
	}

	public void reflectScreen()
	{
		if (titleTextMesh == null)
		{
			return;
		}

		string cardTitle = "";
		cardTitle = cardTitle + CardDictionary.globalCardDictionary[id].name;

		if (isInteractiveDisabled && cardDisable != null)
		{
			string disabledTitle = "[DISABLED] ";

			disabledTitle = disabledTitle + $"[{cardDisable}]";
			if (cardDisable == CardDisableType.AutoMoving) { }
			cardTitle = disabledTitle + cardTitle;
		}

		int fontSize = getFontSize(cardTitle);

		titleTextMesh.fontSize = fontSize;
		titleTextMesh.text = cardTitle;
	}

	private int getFontSize(string title)
	{
		if (title.Length > 10)
		{
			return 13;
		}
		return 18;
	}
}
