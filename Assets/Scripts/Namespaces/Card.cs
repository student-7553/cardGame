
using UnityEngine;
using System.Collections.Generic;
namespace Core
{
    public class Card : MonoBehaviour, Stackable
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
        [System.NonSerialized]
        public CardStack joinedStack;


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

        public void removeFromCardStack()
        {
            isStacked = false;
            joinedStack = null;
        }

        public void addToCardStack(CardStack newCardStack)
        {
            isStacked = true;
            joinedStack = newCardStack;
        }

        public void stackOnThis(List<Card> draggingCards)
        {
            if (isStacked)
            {
                CardStack existingstack = joinedStack;
                existingstack.addCardsToStack(draggingCards);
            }
            else
            {
                List<Card> newCardStackCards = new List<Card>(new Card[] { this });
                newCardStackCards.AddRange(draggingCards);
                CardStack newStack = new CardStack(CardStackType.Cards);
                newStack.addCardsToStack(newCardStackCards);
            }
        }
    }
}