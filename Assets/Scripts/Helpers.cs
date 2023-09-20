using UnityEngine;
using System.Collections.Generic;
using Core;
using System.Linq;

namespace Helpers
{
	public struct TypeAdjustingData
	{
		public List<Card> removingCards;
		public List<int> addingCardIds;

		public void init()
		{
			removingCards = new List<Card>();
			addingCardIds = new List<int>();
		}
	}

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

		public static Interactable getInteractableFromGameObject(GameObject cardObject)
		{
			Interactable interactable = cardObject.GetComponent(typeof(Interactable)) as Interactable;
			return interactable;
		}

		public static bool getDraggingCardsAngle(Vector3 initalPostion, Vector3 currentPosition)
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
		public static CardsTypes[] valueCardTypes = { CardsTypes.Electricity, CardsTypes.Will, CardsTypes.Gold, CardsTypes.Food };

		public static List<int> getAscTypeValueCardIds(CardsTypes cardType, List<int> cardIds)
		{
			List<int> ascTypeCardIds = new List<int>();
			if (isNonValueTypeCard(cardType))
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

		public static List<Card> getAscTypeValueCards(CardsTypes cardType, List<Card> cards)
		{
			List<Card> ascTypeCards = new List<Card>();
			if (isNonValueTypeCard(cardType))
			{
				return ascTypeCards;
			}

			var populatedCards = cards
				.Where(
					(card) =>
					{
						return CardDictionary.globalCardDictionary.ContainsKey(card.id)
							&& CardDictionary.globalCardDictionary[card.id].type == cardType;
					}
				)
				.Select((card) => new { cardObject = CardDictionary.globalCardDictionary[card.id], card = card })
				.OrderBy(o => o.cardObject.typeValue)
				.ToList();

			List<Card> returnCards = populatedCards.Select((populatedCard) => populatedCard.card).ToList();

			return returnCards;
		}

		public static List<int> generateTypeValueCards(CardsTypes cardType, int value)
		{
			List<int> ascTypeCardIds = new List<int>();
			if (isNonValueTypeCard(cardType))
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

		public static int getTypeValueFromCardIds(CardsTypes cardType, List<int> cardIds)
		{
			if (isNonValueTypeCard(cardType))
			{
				return 0;
			}

			int typeValue = 0;
			foreach (int cardId in cardIds)
			{
				if (CardDictionary.globalCardDictionary.ContainsKey(cardId) && CardDictionary.globalCardDictionary[cardId].type == cardType)
				{
					typeValue = typeValue + CardDictionary.globalCardDictionary[cardId].typeValue;
				}
			}

			return typeValue;
		}

		public static bool isNonValueTypeCard(CardsTypes cardType)
		{
			if (!valueCardTypes.Contains(cardType))
			{
				return true;
			}
			return false;
		}

		public static TypeAdjustingData handleTypeAdjusting(List<Card> availableCards, CardsTypes cardType, int requiredTypeValue)
		{
			TypeAdjustingData returnData = new TypeAdjustingData { addingCardIds = new List<int>(), removingCards = new List<Card>() };
			int totalSum = 0;
			List<Card> ascTypeCards = CardHelpers.getAscTypeValueCards(cardType, availableCards);
			foreach (Card singleCard in ascTypeCards)
			{
				returnData.removingCards.Add(singleCard);
				totalSum = totalSum + CardDictionary.globalCardDictionary[singleCard.id].typeValue;
				if (totalSum == requiredTypeValue)
				{
					break;
				}
				if (totalSum > requiredTypeValue)
				{
					int addingTypeValue = totalSum - requiredTypeValue;
					List<int> newCardIds = CardHelpers.generateTypeValueCards(cardType, addingTypeValue);
					returnData.addingCardIds.AddRange(newCardIds);
					break;
				}
			}
			return returnData;
		}

		public static Dictionary<int, int> indexCardIds(List<int> requiredIds)
		{
			Dictionary<int, int> indexedRequiredIds = new Dictionary<int, int>();
			foreach (int baseRequiredId in requiredIds)
			{
				if (indexedRequiredIds.ContainsKey(baseRequiredId))
				{
					indexedRequiredIds[baseRequiredId] = indexedRequiredIds[baseRequiredId] + 1;
				}
				else
				{
					indexedRequiredIds.Add(baseRequiredId, 1);
				}
			}
			return indexedRequiredIds;
		}
	}

	public interface Interactable
	{
		public bool isInteractiveDisabled { get; set; }
		public CoreInteractableType interactableType { get; }

		public BaseCard getBaseCard();
		public Card getCard();
		public CardCollapsed getCollapsedCard();
		GameObject gameObject { get; }
	}

	public static class HelperData
	{
		public static readonly float baseZ = 0f;

		public static readonly float nodeBoardZ = -2f;

		public static readonly float draggingBaseZ = -4f;

		public static readonly float enemyNodeBaseZ = -3f;
	}
}
