
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Core;

[System.Serializable]
public class BaseNodeStats
{
    public int totalInfraInventory;
    public int totalResourceInventory;
    public int currentHungerCheck;
    public int goldGeneration;
    public int hungerSetIntervalTimer;
}

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


    // -------------------- Node Stats -------------------------

    private BaseNodeStats baseNodeStat;

    private int _resourceInventoryLimit;
    public int resourceInventoryLimit
    {
        get { return _resourceInventoryLimit; }
        set { _resourceInventoryLimit = value; }
    }

    private int _currentAvailableResourceInventory;
    public int currentAvailableResourceInventory
    {
        get { return _currentAvailableResourceInventory; }
        set { _currentAvailableResourceInventory = value; }
    }

    private int _infraInventoryLimit;
    public int infraInventoryLimit
    {
        get { return _infraInventoryLimit; }
        set { _infraInventoryLimit = value; }
    }

    private int _currentAvailableInfraInventory;
    public int currentAvailableInfraInventory
    {
        get { return _currentAvailableInfraInventory; }
        set { _currentAvailableInfraInventory = value; }
    }

    private int _currentHungerCheck;
    public int currentHungerCheck
    {
        get { return _currentHungerCheck; }
        set { _currentHungerCheck = value; }
    }

    private int _currentElectricity;
    public int currentElectricity
    {
        get { return _currentElectricity; }
        set { _currentElectricity = value; }
    }

    private int _currentGold;
    public int currentGold
    {
        get { return _currentGold; }
        set { _currentGold = value; }
    }

    private int _goldGeneration;
    public int goldGeneration
    {
        get { return _goldGeneration; }
        set { _goldGeneration = value; }
    }


    // -------------------- Meta Stats -------------------------

    public string title;

    public bool isActive;

    private float intervalTimer;  // ********* Loop timer *********

    NodeStateTypes nodeState;

    private int _hungerSetIntervalTimer; // ********* sec, next time the check is applied  *********
    public int hungerSetIntervalTimer
    {
        get { return _hungerSetIntervalTimer; }
        set { _hungerSetIntervalTimer = value; }
    }


    // --------------------Readonly Stats-------------------------

    private readonly float nodePlaneBaseY = 3f;


    private void initlizeBaseStats()
    {
        BaseNodeStats baseNodeStat = new BaseNodeStats();
        baseNodeStat.totalInfraInventory = 10;
        baseNodeStat.totalResourceInventory = 10;
        baseNodeStat.currentHungerCheck = 1;
        baseNodeStat.goldGeneration = 1;
        baseNodeStat.hungerSetIntervalTimer = 60;
        this.baseNodeStat = baseNodeStat;

        _resourceInventoryLimit = baseNodeStat.totalResourceInventory;
        _currentHungerCheck = baseNodeStat.currentHungerCheck;
        _goldGeneration = baseNodeStat.goldGeneration;
        _hungerSetIntervalTimer = baseNodeStat.hungerSetIntervalTimer;
        _infraInventoryLimit = baseNodeStat.totalInfraInventory;

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

    public void computeStats()
    {
        List<int> cardIds = activeStack.getCardIds();
        float calcTotalResourceInventory = 0;
        float calcUsedResourceInventory = 0;
        float calcHungerCheck = 0;
        float calcElectricity = 0;
        float calcGold = 0;

        foreach (int id in cardIds)
        {
            if (CardDictionary.globalCardDictionary.ContainsKey(id))
            {
                calcUsedResourceInventory += CardDictionary.globalCardDictionary[id].resourceInventoryCount;
                calcHungerCheck += CardDictionary.globalCardDictionary[id].foodCost;
                if (CardDictionary.globalCardDictionary[id].type == CardsTypes.Electricity)
                {
                    calcElectricity += CardDictionary.globalCardDictionary[id].typeValue;
                }
                if (CardDictionary.globalCardDictionary[id].type == CardsTypes.Gold)
                {
                    calcGold += CardDictionary.globalCardDictionary[id].typeValue;
                }
                if (CardDictionary.globalCardDictionary[id].type == CardsTypes.Module)
                {
                    calcTotalResourceInventory += CardDictionary.globalCardDictionary[id].module.resourceInventory;
                }

            }
        }

        // get the cards info from dictionary
        // compute our stats

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
        computeStats();
    }

    public CardStack getCardStack()
    {
        return activeStack;
    }


    private void reflectToScreen()
    {
        titleTextMesh.text = title;
        availableInventoryTextMesh.text = "" + resourceInventoryLimit + "/" + currentAvailableResourceInventory;
        hungerTextMesh.text = "" + _currentHungerCheck;
    }

}
