using UnityEngine;
using Core;
using Helpers;
using System.Collections.Generic;

public abstract class BaseCard : MonoBehaviour, Interactable, IStackable
{
	public static float baseCardX = 5;
	public static float baseCardY = 8;

	// -------------------- Interactable Members -------------------------
	private bool _isInteractiveDisabled = false;
	public bool isInteractiveDisabled
	{
		get { return _isInteractiveDisabled; }
		set { _isInteractiveDisabled = value; }
	}

	public virtual CoreInteractableType interactableType
	{
		get { return CoreInteractableType.Cards; }
	}

	public virtual Card getCard()
	{
		return null;
	}

	public virtual CardCollapsed getCollapsedCard()
	{
		return null;
	}

	public BaseCard getBaseCard()
	{
		return this;
	}

	public virtual void destroyCard() { }

	// -------------------- Interactable Members End -------------------------



	public int id;

	public CardCorners corners;

	public virtual CardHolder joinedStack { get; set; }

	public CardDisableType? cardDisable;

	public void computeCorners()
	{
		corners = generateTheCorners();
	}

	public bool isStacked()
	{
		return joinedStack != null;
	}

	public void disableInteractiveForATime(float timer, CardDisableType disableType)
	{
		cardDisable = disableType;
		isInteractiveDisabled = true;
	}

	public void moveCard(Vector3 newPosition)
	{
		gameObject.transform.position = newPosition;
		computeCorners();
	}

	public void attachToCardHolder(CardHolder newCardHolder)
	{
		joinedStack = newCardHolder;
	}

	private CardCorners generateTheCorners()
	{
		if (gameObject == null)
		{
			return null;
		}
		Vector3 leftTopCornerPoint = new Vector3(
			gameObject.transform.position.x - (baseCardX / 2),
			gameObject.transform.position.y + (baseCardY / 2),
			gameObject.transform.position.z
		);

		Vector3 rightTopCornerPoint = new Vector3(
			gameObject.transform.position.x + (baseCardX / 2),
			gameObject.transform.position.y + (baseCardY / 2),
			gameObject.transform.position.z
		);

		Vector3 leftBottomCornerPoint = new Vector3(
			gameObject.transform.position.x - (baseCardX / 2),
			gameObject.transform.position.y - (baseCardY / 2),
			gameObject.transform.position.z
		);

		Vector3 rightBottomCornerPoint = new Vector3(
			gameObject.transform.position.x + (baseCardX / 2),
			gameObject.transform.position.y - (baseCardY / 2),
			gameObject.transform.position.z
		);

		CardCorners newCorners = new CardCorners(leftTopCornerPoint, rightTopCornerPoint, leftBottomCornerPoint, rightBottomCornerPoint);
		return newCorners;
	}

	public abstract void stackOnThis(BaseCard draggingCard, Node _prevNode);
}

public interface SelfBaseCardInterface
{
	public void reflectScreen() { }
}

public interface CardHolder
{
	public void removeCardsFromStack(List<BaseCard> removingCards);
	public void addCardsToStack(List<BaseCard> addingCard);
	public CardStackType getCardHolderType();
	public List<BaseCard> getCards();
	public BaseNode getNode();
}

public class CardCorners
{
	public Vector3 leftTopCorner;
	public Vector3 rightTopCorner;
	public Vector3 leftBottomCorner;
	public Vector3 rightBottomCorner;

	public CardCorners(Vector3 leftTopCorner_, Vector3 rightTopCorner_, Vector3 leftBottomCorner_, Vector3 rightBottomCorner_)
	{
		leftTopCorner = leftTopCorner_;
		rightTopCorner = rightTopCorner_;
		leftBottomCorner = leftBottomCorner_;
		rightBottomCorner = rightBottomCorner_;
	}
}

public enum CardDisableType
{
	Que,
	Process,
	Dead,
	AutoMoving,
}
