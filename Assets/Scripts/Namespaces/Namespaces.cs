
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
namespace CardGlobal
{
    public class Card : MonoBehaviour
    {

        public bool isStacked;
        [System.NonSerialized]
        public Vector3 leftTopCorner;
        [System.NonSerialized]
        public Vector3 rightTopCorner;
        [System.NonSerialized]
        public Vector3 leftBottomCorner;

        [System.NonSerialized]
        public Vector3 rightBottomCorner;

        // [System.NonSerialized]
        public CardStack joinedStack;

        [System.NonSerialized]
        public static float cardBaseY = 1;


        private float baseCardX = 5;
        private float baseCardY = 8;

        private void Awake()
        {
            this.generateTheCorners();
            isStacked = false;
        }

        public void generateTheCorners()
        {
            Vector3 leftTopCornerPoint = new Vector3(gameObject.transform.position.x - (baseCardX / 2),
            gameObject.transform.position.y, gameObject.transform.position.z + (baseCardY / 2));

            Vector3 rightTopCornerPoint = new Vector3(gameObject.transform.position.x + (baseCardX / 2),
            gameObject.transform.position.y, gameObject.transform.position.z + (baseCardY / 2));

            Vector3 leftBottomCornerPoint = new Vector3(gameObject.transform.position.x - (baseCardX / 2),
            gameObject.transform.position.y, gameObject.transform.position.z - (baseCardY / 2));

            Vector3 rightBottomCornerPoint = new Vector3(gameObject.transform.position.x + (baseCardX / 2),
            gameObject.transform.position.y, gameObject.transform.position.z - (baseCardY / 2));

            leftTopCorner = leftTopCornerPoint;
            rightTopCorner = rightTopCornerPoint;
            leftBottomCorner = leftBottomCornerPoint;
            rightBottomCorner = rightBottomCornerPoint;
        }

        public void removeFromCardStack(){
            isStacked = false;
            joinedStack = null;
        }

        public void addToCardStack(CardStack newCardStack){
            isStacked = true;
            joinedStack = newCardStack;
        }



    }

    public class CardStack
    {


        private static float stackDistance = 5;
        public static float distancePerCards = 0.01f;

        // we are assuming that origin card is 0 indexed on array
        public List<Card> cards;

        public CardStack(Card originCard, Card newCard)
        {
            cards = new List<Card>(new Card[] { originCard, newCard });
            foreach (Card singleCard in cards)
            {
                singleCard.isStacked = true;
                singleCard.joinedStack = this;
            }
            this.alignCards(1);
        }

        public void alignCards(int from)
        {
            Card originCard = cards[0];
            originCard.transform.position = new Vector3(originCard.transform.position.x, Card.cardBaseY, originCard.transform.position.z);
            // we are not loopting through first card because it's the origin point
            for (int i = from; i < cards.Count; i++)
            {
                Card cardInSubject = cards[i];
                Vector3 newPostionForCardInSubject = new Vector3(originCard.transform.position.x, originCard.transform.position.y + (i * distancePerCards), originCard.transform.position.z - (stackDistance* i));
                cardInSubject.transform.position = newPostionForCardInSubject;
                cardInSubject.generateTheCorners();
            }
        }

        public void removeCardsFromStack(List<Card> removingCards)
        {
            foreach(Card singleCard in removingCards){
                cards.Remove(singleCard);
                singleCard.removeFromCardStack();
            }

            if(cards.Count > 1){
                this.alignCards(1);
            } else {
                this.checkIfDead();
            }
        }

        public void addCardToStack(Card addingCard)
        {
            int previousLength = cards.Count;
            cards.Add(addingCard);
            addingCard.addToCardStack(this);
            this.alignCards(previousLength);
        }

        private void printCards(){
            foreach(Card singleCard in cards){
                Debug.Log(singleCard);
            }
        }

        private void checkIfDead(){
            if(cards.Count > 1){
                return;
            }
            foreach(Card singleCard in cards){
                singleCard.removeFromCardStack();
            }

            
        }
    }

    public class Node
    {

    }
}