
using UnityEngine;

namespace CardGlobal
{
    public class Card : MonoBehaviour 
    {
        public Vector3 leftTopCorner;
        public Vector3 rightTopCorner;
        public Vector3 leftBottomCorner;
        public Vector3 rightBottomCorner;
        public GameObject cardGameObject;
        public bool isStacked;

        public Card(GameObject clickedObject){

            Vector3 leftTopCornerPoint = new Vector3(clickedObject.transform.position.x - (clickedObject.transform.localScale.x / 2),
            clickedObject.transform.position.y, clickedObject.transform.position.z + (clickedObject.transform.localScale.z / 2));

            Vector3 rightTopCornerPoint = new Vector3(clickedObject.transform.position.x + (clickedObject.transform.localScale.x / 2),
            clickedObject.transform.position.y, clickedObject.transform.position.z + (clickedObject.transform.localScale.z / 2));

            Vector3 leftBottomCornerPoint = new Vector3(clickedObject.transform.position.x - (clickedObject.transform.localScale.x / 2),
            clickedObject.transform.position.y, clickedObject.transform.position.z - (clickedObject.transform.localScale.z / 2));

            Vector3 rightBottomCornerPoint = new Vector3(clickedObject.transform.position.x + (clickedObject.transform.localScale.x / 2),
            clickedObject.transform.position.y, clickedObject.transform.position.z - (clickedObject.transform.localScale.z / 2));

            leftTopCorner = leftTopCornerPoint;
            rightTopCorner = rightTopCornerPoint;
            leftBottomCorner = leftBottomCornerPoint;
            rightBottomCorner = rightBottomCornerPoint;
            cardGameObject = clickedObject;

            isStacked = false;
        }
    }

    public class Stack : MonoBehaviour
    {   
        public Card[] cards;
        public Stack(Card[] newCards){
            cards = newCards;
            foreach (Card singleCard in cards){

            }
        }
    }

     public class Node : MonoBehaviour
    {
        
    }
}