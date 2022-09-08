
using UnityEngine;
using System.Collections.Generic;
namespace Core
{
    public class Node : MonoBehaviour, Stackable ,Dragable
    {
        enum nodeStateTypes
        {
            low,
            medium,
        };

        [System.NonSerialized]
        public CardStack activeStack;

        nodeStateTypes nodeState;

        private void Awake()
        {
            nodeState = nodeStateTypes.low;
        }

        public void stackOnThis(List<Card> draggingCards)
        {
            Debug.Log("Are we called??");
        }

        public void setCardStackOfNode(CardStack cardStack)
        {
            activeStack = cardStack;
        }

        public void clearCardStack()
        {
            activeStack = null;
        }

    }
}