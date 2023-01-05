using UnityEngine;
using System.Collections.Generic;
using Core;
using System.Linq;

namespace Helpers
{
	public static class DragAndDropHelper
	{
		public static bool getPositionAngle(Vector3 initalPostion, Vector3 currentPosition)
		{
			// get the angle of attack on the postions
			Vector2 initalPostion2d = new Vector2(initalPostion.x, initalPostion.y);
			Vector2 currentPosition2d = new Vector2(currentPosition.x, currentPosition.y);
			float angle = Vector2.Angle(currentPosition2d - initalPostion2d, Vector2.up);
			float directionalAngle = Vector3.Angle((currentPosition2d - initalPostion2d), Vector2.right);
			if (directionalAngle > 90)
			{
				angle = 360 - angle;
			}
			if (angle > 150 && angle < 210)
			{
				return true;
			}
			return false;
		}

		public static Card getCardFromGameObject(GameObject cardObject)
		{
			Card hitCard = cardObject.GetComponent(typeof(Card)) as Card;
			if (hitCard == null)
			{
				return null;
			}
			return hitCard;
		}

		public static bool isObjectDraggable(GameObject cardObject)
		{
			Card hitCard = cardObject.GetComponent(typeof(Card)) as Card;
			if (hitCard == null)
			{
				return false;
			}
			return true;
		}
	}

	public static class CardHelpers
	{
		private static CardsTypes[] allowedCardTypes = { CardsTypes.Electricity, CardsTypes.Gold, CardsTypes.Food };

		public static List<int> getAscTypeValueCardIds(CardsTypes cardType, List<int> cardIds)
		{
			List<int> ascTypeCardIds = new List<int>();
			if (isNotAllowedCardType(cardType))
			{
				return ascTypeCardIds;
			}

			List<CardObject> cardObjects = new List<CardObject>();

			foreach (int cardId in cardIds)
			{
				if (CardDictionary.globalCardDictionary.ContainsKey(cardId) && CardDictionary.globalCardDictionary[cardId].type == cardType)
				{
					cardObjects.Add(CardDictionary.globalCardDictionary[cardId]);
				}
			}

			List<CardObject> descSortedList = cardObjects.OrderBy(o => o.typeValue).ToList();
			foreach (CardObject singleCardObject in descSortedList)
			{
				ascTypeCardIds.Add(singleCardObject.id);
			}

			return ascTypeCardIds;
		}

		public static List<int> generateTypeValueCards(CardsTypes cardType, int value)
		{
			List<int> ascTypeCardIds = new List<int>();
			if (isNotAllowedCardType(cardType))
			{
				return ascTypeCardIds;
			}

			List<CardObject> cardObjects = CardDictionary.getAllCardTypeCards(cardType);
			List<CardObject> descCardObjects = cardObjects.OrderByDescending(o => o.typeValue).ToList();
			int remainingValue = value;
			foreach (CardObject singleCardObject in descCardObjects)
			{
				void handleLoop()
				{
					// return true;
					if (singleCardObject.typeValue <= remainingValue)
					{
						ascTypeCardIds.Add(singleCardObject.id);
						remainingValue = remainingValue - singleCardObject.typeValue;
					}
					if (singleCardObject.typeValue <= remainingValue)
					{
						handleLoop();
					}
				}

				handleLoop();

				if (remainingValue == 0)
				{
					break;
				}
			}

			return ascTypeCardIds;
		}

		private static bool isNotAllowedCardType(CardsTypes cardType)
		{
			List<int> ascTypeCardIds = new List<int>();
			List<CardsTypes> listAllowedCardTypes = new List<CardsTypes>(allowedCardTypes);
			if (!listAllowedCardTypes.Contains(cardType))
			{
				return true;
			}
			return false;
		}
	}

	public interface Interactable
	{
		public bool isInteractiveDisabled { get; set; }
		public SpriteRenderer spriteRenderer { get; set; }
		public CoreInteractableType interactableType { get; set; }
		GameObject gameObject { get; }
		public Card getCard();
	}

	public static class HelperData
	{
		public static readonly float baseZ = 0f;

		public static readonly float nodeBoardZ = -2f;

		public static readonly float draggingBaseZ = -4f;
	}
}
