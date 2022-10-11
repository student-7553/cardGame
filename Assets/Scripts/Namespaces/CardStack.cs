using System.Collections.Generic;
using UnityEngine;
namespace Core
{
    public class CardStack
    {
        private static float stackDistance = 5;
        public static float distancePerCards = 0.01f;

        // we are assuming that origin card is 0 indexed on array
        public List<Card> cards;
        public bool isNodeStack;

        public CardStack(bool isNode)
        {
            isNodeStack = isNode;
            cards = new List<Card>();
        }

        public void alignCards(int from)
        {
            if (cards.Count <= 1)
            {
                return;
            }

            Card originCard = cards[0];
            originCard.transform.position = new Vector3(originCard.transform.position.x, Card.cardBaseY, originCard.transform.position.z);
            // we are not loopting through first card because it's the origin point
            for (int i = from; i < cards.Count; i++)
            {
                Card cardInSubject = cards[i];
                Vector3 newPostionForCardInSubject = new Vector3(originCard.transform.position.x, originCard.transform.position.y + (i * distancePerCards), originCard.transform.position.z - (stackDistance * i));
                cardInSubject.transform.position = newPostionForCardInSubject;
                cardInSubject.generateTheCorners();
            }
        }

        public void hideAllCards(){
           foreach (Card singleCard in cards)
            {
                singleCard.gameObject.SetActive(false);
            } 

        }

        public void removeCardsFromStack(List<Card> removingCards)
        {
            foreach (Card singleCard in removingCards)
            {
                cards.Remove(singleCard);
                singleCard.removeFromCardStack();
            }
            this.checkIfDead();
            this.alignCards(1);
        }

        public void addCardsToStack(List<Card> addingCards)
        {
            cards.AddRange(addingCards);
            foreach (Card singleCard in addingCards)
            {
                singleCard.addToCardStack(this);
            }
        }

        public void addCardToStack(Card addingCard)
        {
            cards.Add(addingCard);
            addingCard.addToCardStack(this);
        }

        private void checkIfDead()
        {
            if (cards.Count > 1)
            {
                return;
            }
            foreach (Card singleCard in cards)
            {
                singleCard.removeFromCardStack();
            }
        }

        private void logCards()
        {
            foreach (Card singleCard in cards)
            {
                Debug.Log(singleCard);
            }
        }
    }



}