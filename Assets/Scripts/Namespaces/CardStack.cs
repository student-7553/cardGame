using System.Collections.Generic;
using UnityEngine;
namespace Core
{
    public class CardStack
    {
        private static float stackDistance = 5;
        private static float distancePerCards = 0.01f;

        public float cardBaseY;

        public CardStackType cardStackType;

        public List<Card> cards;

        public CardStack(CardStackType givenStackType)
        {
            cardStackType = givenStackType;
            cardBaseY = 1;
            cards = new List<Card>();
        }


        public void alignCards()
        {
            if (cards.Count <= 1)
            {
                return;
            }

            Card rootCard = this.getRootCard();
            rootCard.transform.position = new Vector3(rootCard.transform.position.x, cardBaseY, rootCard.transform.position.z);
            // we are not loopting through first card because it's the origin point
            for (int i = 1; i < cards.Count; i++)
            {
                Card cardInSubject = cards[i];
                Vector3 newPostionForCardInSubject = new Vector3(rootCard.transform.position.x, rootCard.transform.position.y + (i * distancePerCards), rootCard.transform.position.z - (stackDistance * i));
                cardInSubject.transform.position = newPostionForCardInSubject;
                cardInSubject.generateTheCorners();
            }
        }

        public void changeActiveStateOfAllCards(bool isActive)
        {
            foreach (Card singleCard in cards)
            {
                singleCard.gameObject.SetActive(isActive);
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
            this.alignCards();
        }

        public Card getRootCard()
        {
            if (cards.Count > 0)
            {
                return cards[0];
            }
            return null;
        }

        public void addCardsToStack(List<Card> addingCards)
        {
            cards.AddRange(addingCards);
            foreach (Card singleCard in addingCards)
            {
                singleCard.addToCardStack(this);
            }

            this.alignCards();
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

        public void moveRootCardToPosition(float newX, float newZ)
        {
            Card rootCard = this.getRootCard();
            if (rootCard == null)
            {
                return;
            }
            rootCard.gameObject.transform.position = new Vector3(
                newX,
                rootCard.gameObject.transform.position.y,
                newZ);
            alignCards();
        }

    }



}