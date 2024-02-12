using UnityEngine;
using Core;
using System.Collections.Generic;
using TMPro;

public abstract class BaseCard : MonoBehaviour, Interactable, IStackable, PositionRestricted
{
	public static float baseCardX = 5;
	public static float baseCardY = 8;
	public Vector3 currentVelocity;

	// -------------------- Interactable Members -------------------------

	public CardDisableType? cardDisable;

	public StaticVariables staticVariables;

	[SerializeField]
	private bool _isInteractiveDisabled = false;
	public bool isInteractiveDisabled
	{
		get { return _isInteractiveDisabled; }
		set
		{
			if (value == false)
			{
				cardDisable = null;
			}
			_isInteractiveDisabled = value;
		}
	}

	[SerializeField]
	private SpriteRenderer shadowSpriteRenderer;

	[SerializeField]
	private SpriteRenderer borderSpriteRenderer;

	[SerializeField]
	private SpriteRenderer backgroundSpriteRenderer;

	public TextMeshPro titleTextMesh;

	public void setSpriteHovering(bool isHovering, Interactable.SpriteInteractable targetSprite)
	{
		if (targetSprite == Interactable.SpriteInteractable.hover)
		{
			if (shadowSpriteRenderer == null)
			{
				return;
			}
			Vector3 newScale = isHovering
				? shadowSpriteRenderer.transform.localScale * 1.075f
				: shadowSpriteRenderer.transform.localScale / 1.075f;
			shadowSpriteRenderer.transform.localScale = newScale;
		}
	}

	public virtual CoreInteractableType interactableType
	{
		get { return CoreInteractableType.Cards; }
	}

	public bool isCardType()
	{
		return true;
	}

	public virtual Card getCard()
	{
		return null;
	}

	public virtual CardCollapsed getCollapsedCard()
	{
		return null;
	}

	public ref Vector3 getCurrentVelocity()
	{
		return ref currentVelocity;
	}

	public Vector3 getFinalPosition(Vector3 newPostion)
	{
		float padding = 40;
		CornerPoints cameraCornerPoints = new CornerPoints()
		{
			up = 140,
			down = -140,
			left = -200,
			right = 200
		};
		CornerPoints restrictedCornerPoints = new CornerPoints()
		{
			up = cameraCornerPoints.up - padding,
			down = cameraCornerPoints.down + padding,
			left = cameraCornerPoints.left + padding,
			right = cameraCornerPoints.right - padding
		};

		Vector3 adjustedNewPosition = newPostion;
		if (newPostion.x > restrictedCornerPoints.right)
		{
			adjustedNewPosition.x = restrictedCornerPoints.right;
		}
		else if (newPostion.x < restrictedCornerPoints.left)
		{
			adjustedNewPosition.x = restrictedCornerPoints.left;
		}

		if (newPostion.y > restrictedCornerPoints.up)
		{
			adjustedNewPosition.y = restrictedCornerPoints.up;
		}
		else if (newPostion.y < restrictedCornerPoints.down)
		{
			adjustedNewPosition.y = restrictedCornerPoints.down;
		}

		return adjustedNewPosition;
	}

	public BaseCard getBaseCard()
	{
		return this;
	}

	public virtual void destroyCard() { }

	public virtual Interactable[] getMouseHoldInteractables()
	{
		Interactable[] interactables = { this };
		return interactables;
	}

	// -------------------- Interactable Members End -------------------------


	public int _id;
	public int id
	{
		get => _id;
		set
		{
			_id = value;

			Color typeColor = staticVariables.cardColors
				.Find(
					(cardColor) =>
					{
						return cardColor.cardType == CardDictionary.globalCardDictionary[_id].type;
					}
				)
				.color;

			Color typeTextColor = staticVariables.cardTextColors
				.Find(
					(cardColor) =>
					{
						return cardColor.cardType == CardDictionary.globalCardDictionary[_id].type;
					}
				)
				.color;

			Color typeBackgroundColor = staticVariables.cardBackgroundColors
				.Find(
					(cardColor) =>
					{
						return cardColor.cardType == CardDictionary.globalCardDictionary[_id].type;
					}
				)
				.color;

			typeColor.a = 1;
			typeBackgroundColor.a = 1;
			typeTextColor.a = 1;

			borderSpriteRenderer.color = typeColor;
			backgroundSpriteRenderer.color = typeBackgroundColor;
			titleTextMesh.color = typeTextColor;
		}
	}

	public CardCorners corners;

	public virtual CardHolder joinedStack { get; set; }

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
