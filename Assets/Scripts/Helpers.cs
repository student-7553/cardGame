
using UnityEngine;
using Core;
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
                 return null;
            }
            return hitCard;
        }

        public static Interactable getInteractableFromGameObject(GameObject cardObject)
        {
            Interactable interactable = cardObject.GetComponent(typeof(Interactable)) as Interactable;
            if (interactable == null)
            {
                 return null;
            }
            return interactable;
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
        

        // public static Card generateCardClassOnObject(GameObject subject){
        //     Card newCard = subject.AddComponent<Card>();
        //     return newCard;
        // }

        public static void moveDraggingObjects(Vector3 movingToPoint, List<Interactable> draggingObjects)
        {
            foreach (Interactable singleDraggingObject in draggingObjects)
            {
                Vector3 finalMovingPoint = movingToPoint;
                finalMovingPoint.y = singleDraggingObject.gameObject.transform.position.y;
                singleDraggingObject.gameObject.transform.position = finalMovingPoint;
            }
        }
    }
}