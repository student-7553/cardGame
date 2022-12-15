using UnityEngine;
using Core;
using System.Collections.Generic;
using TMPro;

public class NodePlaneHandler : MonoBehaviour, IStackable
{
    private Node currentNode;
    public TextMeshPro textMesh;


    private void Awake()
    {
        currentNode = GetComponentInParent(typeof(Node)) as Node;
        Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));
        textMesh = textMeshes[0] as TextMeshPro;
    }

    private void OnDisable()
    {
        currentNode.getCardStack().changeActiveStateOfAllCards(false);
        currentNode.isNodePlaneActive = false;
        GameManager.current.boardPlaneHandler.clearActiveNodePlane();
    }

    private void OnEnable()
    {
        currentNode.getCardStack().changeActiveStateOfAllCards(true);
        currentNode.isNodePlaneActive = true;
        GameManager.current.boardPlaneHandler.setActiveNodePlane(this);
    }

    public void stackOnThis(List<Card> draggingCards)
    {
        currentNode.stackOnThis(draggingCards);
    }

    // public void reflectToScreen()
    // {

    // }
}
