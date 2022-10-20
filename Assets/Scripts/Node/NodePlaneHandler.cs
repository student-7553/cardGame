using UnityEngine;
using Core;
using System.Collections.Generic;

public class NodePlaneHandler : MonoBehaviour, Stackable
{
    private Node currentNode;
    
    private void Awake()
    {
        currentNode = GetComponentInParent(typeof(Node)) as Node;
    }

    public void stackOnThis(List<Card> draggingCards)
    {
        CardStack currentStack = currentNode.getCardStack();
        currentNode.getCardStack().addCardsToStack(draggingCards);
        currentStack.moveRootCardToPosition(gameObject.transform.position.x, gameObject.transform.position.z);
    }
}
