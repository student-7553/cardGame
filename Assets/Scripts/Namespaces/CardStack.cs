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
        public CardStack(List<Card> cardStackCards)
        {
            cards = cardStackCards;
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
                Vector3 newPostionForCardInSubject = new Vector3(originCard.transform.position.x, originCard.transform.position.y + (i * distancePerCards), originCard.transform.position.z - (stackDistance * i));
                cardInSubject.transform.position = newPostionForCardInSubject;
                cardInSubject.generateTheCorners();
            }
        }

        public void removeCardsFromStack(List<Card> removingCards)
        {
            foreach (Card singleCard in removingCards)
            {
                cards.Remove(singleCard);
                singleCard.removeFromCardStack();
            }
            if (cards.Count > 1)
            {
                this.alignCards(1);
            }
            else
            {
                this.checkIfDead();
            }
        }

        public void addCardsToStack(List<Card> addingCards)
        {
            int previousLength = cards.Count;
            cards.AddRange(addingCards);
            foreach (Card singleCard in cards)
            {
                singleCard.addToCardStack(this);
            }
            this.alignCards(previousLength);
        }

        private void printCards()
        {
            foreach (Card singleCard in cards)
            {
                Debug.Log(singleCard);
            }
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
    }



}