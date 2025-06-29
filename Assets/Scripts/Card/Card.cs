using UnityEngine;
using System.Collections.Generic;
using Core;
using System.Linq;

public class Card : BaseCard, IClickable
{
	public SO_PlayerRuntime playerRuntime;
	public SO_Interactable so_Interactable;
	public SO_Highlight soHighlight;
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

		bool dim = isDim();
		if (dim)
		{
			dimCard();
		}
		else
		{
			nonDimCard();
		}

		int fontSize = getFontSize(cardTitle);

		titleTextMesh.fontSize = fontSize;

		cardTitle = cardTitle.Replace("[Idea]", "<i><size=70%>[Idea]</size></i>\n");
		cardTitle = cardTitle.Replace("[Dorm]", "<i><size=70%>[Dorm]</size></i>\n");
		cardTitle = cardTitle.Replace("[Node]", "<i><size=70%>[Node]</size></i>\n");

		titleTextMesh.text = cardTitle;
	}

	private bool isDim()
	{
		if (isInteractiveDisabled && cardDisable != null)
		{
			return true;
		}

		if (soHighlight.isHighlightEnabled)
		{
			if (soHighlight.cardIds.Any((cardId) => cardId == id))
			{
				return false;
			}
			return true;
		}
		return false;
	}

	private int getFontSize(string title)
	{
		if (title.Length < 9)
		{
			return 18;
		}
		if (title.Length < 13)
		{
			return 14;
		}

		return 12;
	}
}
