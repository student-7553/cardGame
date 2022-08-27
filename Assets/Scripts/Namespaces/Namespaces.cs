
using UnityEngine;

namespace CardGlobal
{
    public class Card : MonoBehaviour
    {
        public Vector3 leftTopCorner;
        public Vector3 rightTopCorner;
        public Vector3 leftBottomCorner;
        public Vector3 rightBottomCorner;
        public bool isStacked;
        public CardStack joinedStack;

        private void Awake()
        {
            this.generateTheCorners();
            isStacked = false;
        }



        public void generateTheCorners()
        {

            Vector3 leftTopCornerPoint = new Vector3(gameObject.transform.position.x - (gameObject.transform.localScale.x / 2),
            gameObject.transform.position.y, gameObject.transform.position.z + (gameObject.transform.localScale.z / 2));

            Vector3 rightTopCornerPoint = new Vector3(gameObject.transform.position.x + (gameObject.transform.localScale.x / 2),
            gameObject.transform.position.y, gameObject.transform.position.z + (gameObject.transform.localScale.z / 2));

            Vector3 leftBottomCornerPoint = new Vector3(gameObject.transform.position.x - (gameObject.transform.localScale.x / 2),
            gameObject.transform.position.y, gameObject.transform.position.z - (gameObject.transform.localScale.z / 2));

            Vector3 rightBottomCornerPoint = new Vector3(gameObject.transform.position.x + (gameObject.transform.localScale.x / 2),
            gameObject.transform.position.y, gameObject.transform.position.z - (gameObject.transform.localScale.z / 2));

            leftTopCorner = leftTopCornerPoint;
            rightTopCorner = rightTopCornerPoint;
            leftBottomCorner = leftBottomCornerPoint;
            rightBottomCorner = rightBottomCornerPoint;

        }

    }

    public class CardStack
    {
        public Card[] cards;
        public CardStack(Card[] newCards)
        {
            cards = newCards;
            foreach (Card singleCard in cards)
            {
                singleCard.isStacked = true;
                singleCard.joinedStack = this;
            }
        }

        public void alignCards()
        {

        }
    }

    public class Node
    {

    }
}