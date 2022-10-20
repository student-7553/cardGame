
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Core;

public class Node : MonoBehaviour, Stackable, IClickable
{
    // -------------------- Unity Component -------------------------
    private TextMeshPro titleTextMesh;
    private TextMeshPro availableInventoryTextMesh;
    private TextMeshPro hungerTextMesh;
    public GameObject rootNodePlane;

    // -------------------- Custom Class -------------------------
    private CardStack activeStack;
    private NodePlaneHandler nodePlaneManagers;


    // -------------------- Card Stats -------------------------

    private int _inventoryLimit;
    public int inventoryLimit
    {
        get { return _inventoryLimit; }
        set { _inventoryLimit = value; }
    }

    private int _currentAvailableInventory;
    public int currentAvailableInventory
    {
        get { return _currentAvailableInventory; }
        set { _currentAvailableInventory = value; }
    }

    private int _currentHungerCheck;
    public int currentHungerCheck
    {
        get { return _currentHungerCheck; }
        set { _currentHungerCheck = value; }
    }

    private int _hungerSetIntervalTimer; // ********* sec, next time the check is applied  *********
    public int hungerSetIntervalTimer
    {
        get { return _hungerSetIntervalTimer; }
        set { _hungerSetIntervalTimer = value; }
    }


    // -------------------- Meta Stats -------------------------

    public string title;

    public bool isActive;

    private float intervalTimer;  // ********* Loop timer *********

    NodeStateTypes nodeState;



    // --------------------Readonly Stats-------------------------

    private readonly float nodePlaneBaseY = 3f;


    private void initlizeBaseStats()
    {
        _inventoryLimit = 10;
        _currentAvailableInventory = 10;
        _hungerSetIntervalTimer = 60;
        _currentHungerCheck = 1;
    }


    private void Awake()
    {
        nodeState = NodeStateTypes.low;
        isActive = false;
        activeStack = new CardStack(CardStackType.Nodes);

        Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));
        titleTextMesh = textMeshes[0] as TextMeshPro;
        availableInventoryTextMesh = textMeshes[1] as TextMeshPro;
        hungerTextMesh = textMeshes[2] as TextMeshPro;
        initlizeBaseStats();
    }

    public void init()
    {
        reflectToScreen();
        Vector3 spawningPosition = new Vector3(70, nodePlaneBaseY, 40);
        GameObject newNodePlane = Instantiate(rootNodePlane, spawningPosition, Quaternion.identity, gameObject.transform);
        newNodePlane.SetActive(false);
        activeStack.cardBaseY = spawningPosition.y + 1f;
        nodePlaneManagers = newNodePlane.GetComponent(typeof(NodePlaneHandler)) as NodePlaneHandler;
    }


    public void OnClick()
    {
        if (isActive == true)
        {
            isActive = false;
            nodePlaneManagers.gameObject.SetActive(false);
            activeStack.changeActiveStateOfAllCards(false);
        }
        else
        {
            isActive = true;
            nodePlaneManagers.gameObject.SetActive(true);
            activeStack.changeActiveStateOfAllCards(true);
        }
    }

    public void stackOnThis(List<Card> newCards)
    {
        bool isRootCardChanged = activeStack.cards.Count == 0 ? true : false;
        activeStack.addCardsToStack(newCards);
        if (isActive == false)
        {
            activeStack.changeActiveStateOfAllCards(false);
        }
        if (isRootCardChanged)
        {
            activeStack.moveRootCardToPosition(nodePlaneManagers.gameObject.transform.position.x, nodePlaneManagers.gameObject.transform.position.z);
        }

        computeStats();
    }

    private void computeStats()
    {

    }



    private void Update()
    {
        intervalTimer = intervalTimer + Time.deltaTime;
        if (intervalTimer > hungerSetIntervalTimer)
        {
            handleHungerInterval();
            intervalTimer = 0;
        }
    }

    private void handleHungerInterval()
    {

    }

    public CardStack getCardStack()
    {
        return activeStack;
    }


    private void reflectToScreen()
    {
        titleTextMesh.text = title;
        availableInventoryTextMesh.text = "" + _inventoryLimit + "/" + _currentAvailableInventory;
        hungerTextMesh.text = "" + _currentHungerCheck;
    }

}
