
using UnityEngine;
using CardGlobal;
// using System.Linq;
using System.Collections.Generic;
namespace Helpers
{
    public static class DragAndDropHelper
    {
        public static bool getDraggingCardsAngle(Vector3 initalPostion, Vector3 currentPosition)
        {
            // get the angle of attack on the postions
            Vector2 initalPostion2d = new Vector2(initalPostion.x, initalPostion.z);
            Vector2 currentPosition2d = new Vector2(currentPosition.x, currentPosition.z);
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
                hitCard = cardObject.AddComponent<Card>();
            }
            return hitCard;
        }

        public static void moveDraggingCard(Vector3 movingToPoint, List<Card> draggingCards)
        {
            foreach (Card singleDraggingCard in draggingCards)
            {
                Vector3 finalMovingPoint = movingToPoint;
                finalMovingPoint.y = singleDraggingCard.gameObject.transform.position.y;
                singleDraggingCard.gameObject.transform.position = finalMovingPoint;
            }
        }
    }
}