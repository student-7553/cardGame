using UnityEngine;
using Core;
using System.Collections.Generic;

public class NodePlaneHandler : MonoBehaviour, IStackable
{
    private Node currentNode;

    private void Awake()
    {
        currentNode = GetComponentInParent(typeof(Node)) as Node;
    }

    private void OnDisable()
    {
        currentNode.getCardStack().changeActiveStateOfAllCards(false);
        currentNode.isActive = false;
        GameManager.current.boardPlaneHandler.clearActiveNodePlane();
    }

    private void OnEnable()
    {
        currentNode.getCardStack().changeActiveStateOfAllCards(true);
        currentNode.isActive = true;
        GameManager.current.boardPlaneHandler.setActiveNodePlane(this);
    }

    public void stackOnThis(List<Card> draggingCards)
    {
        currentNode.stackOnThis(draggingCards);
    }
}
