using UnityEngine;
using Core;

public class NodePlaneManagers : MonoBehaviour, IClickable
{
    //   This script handles opening and closing of the node plane
    private int baseYPosition = 3;

    public GameObject rootNodePlane;

    public GameObject currentNodePlane;

    private Node currentNode;

    public void OnClick()
    {
        if (currentNodePlane.activeSelf == true)
        {
            handleDeActive();

        }
        else
        {
            handleActive();
        }
    }
    public void init()
    {
        Vector3 spawningPosition = new Vector3(70, baseYPosition, 40);
        Quaternion rotation = Quaternion.Euler(0, 0, 0);
        currentNodePlane = Instantiate(rootNodePlane, spawningPosition, Quaternion.identity, gameObject.transform);
        currentNodePlane.SetActive(false);
        currentNode.getCardStack().cardBaseY = spawningPosition.y + 1f;
    }

    public void notifyCardsChanged(bool rootChanged)
    {
        CardStack nodeStack = currentNode.getCardStack();
        if (rootChanged)
        {
            nodeStack.moveRootCardToPosition(currentNodePlane.transform.position.x, currentNodePlane.transform.position.z);
        }

    }


    private void Awake()
    {
        currentNode = gameObject.GetComponent(typeof(Node)) as Node;
    }

    private void handleActive()
    {
        currentNodePlane.SetActive(true);
        currentNode.isActive = true;
        CardStack nodeStack = currentNode.getCardStack();
        nodeStack.changeActiveStateOfAllCards(true);

    }
    private void handleDeActive()
    {
        currentNodePlane.SetActive(false);
        currentNode.isActive = false;
        CardStack nodeStack = currentNode.getCardStack();
        // nodeStack.moveRootCardToPosition(currentNodePlane.transform.position.x, currentNodePlane.transform.position.z);
        nodeStack.changeActiveStateOfAllCards(false);
    }

}
