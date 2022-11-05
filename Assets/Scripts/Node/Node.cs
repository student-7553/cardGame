
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using Core;

[System.Serializable]
public class BaseNodeStats
{
    public int infraInventoryLimit;
    public int resourceInventoryLimit;

    public int currentFoodCheck;
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

    private int _resourceInventoryUsed;
    public int resourceInventoryUsed
    {
        get { return _resourceInventoryUsed; }
        set { _resourceInventoryUsed = value; }
    }

    private int _infraInventoryLimit;
    public int infraInventoryLimit
    {
        get { return _infraInventoryLimit; }
        set { _infraInventoryLimit = value; }
    }

    private int _infraInventoryUsed;
    public int currentAvailableInfraInventory
    {
        get { return _infraInventoryUsed; }
        set { _infraInventoryUsed = value; }
    }

    private int _currentFoodCheck;
    public int currentFoodCheck
    {
        get { return _currentFoodCheck; }
        set { _currentFoodCheck = value; }
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

    private int _currentFood;
    public int currentFood
    {
        get { return _currentFood; }
        set { _currentFood = value; }
    }


    // -------------------- Meta Stats -------------------------

    public string title;

    public bool isActive;

    NodeStateTypes nodeState;

    private float intervalTimer;  // ********* Loop timer *********

    private int hungerSetIntervalTimer; // ********* sec, next time the check is applied  *********

    private bool isProccessing; // ********* sec, next time the check is applied  *********


    // --------------------Readonly Stats-------------------------

    private readonly float nodePlaneBaseZ = 3f;


    private void initlizeBaseStats()
    {
        BaseNodeStats baseNodeStat = new BaseNodeStats();
        baseNodeStat.infraInventoryLimit = 10;
        baseNodeStat.resourceInventoryLimit = 10;
        baseNodeStat.currentFoodCheck = 1;
        baseNodeStat.goldGeneration = 1;
        baseNodeStat.hungerSetIntervalTimer = 60;
        this.baseNodeStat = baseNodeStat;

        computeStats();

    }


    private void Awake()
    {
        nodeState = NodeStateTypes.low;
        isActive = false;
        isProccessing = false;
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
        Vector3 spawningPosition = new Vector3(120, 0, 5);
        GameObject newNodePlane = Instantiate(rootNodePlane, gameObject.transform);
        newNodePlane.transform.position = spawningPosition;
        newNodePlane.SetActive(false);
        activeStack.cardBaseZ = spawningPosition.z + 1f;
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
            activeStack.moveRootCardToPosition(nodePlaneManagers.gameObject.transform.position.x,
                nodePlaneManagers.gameObject.transform.position.y);
        }

        computeStats();
        // processCards();
    }

    public void computeStats()
    {
        List<int> cardIds = activeStack.getCardIds();
        int calcResourceInventoryUsed = 0;
        int calcResourceInventoryLimit = 0;

        int calcInfraInventoryUsed = 0;
        int calcInfraInventoryLimit = 0;

        int calcGoldGeneration = 0;

        int calcHungerCheck = 0;
        int calcElectricity = 0;
        int calcGold = 0;
        int calcFood = 0;

        foreach (int id in cardIds)
        {
            if (CardDictionary.globalCardDictionary.ContainsKey(id))
            {
                calcResourceInventoryUsed += CardDictionary.globalCardDictionary[id].resourceInventoryCount;
                calcInfraInventoryUsed += CardDictionary.globalCardDictionary[id].infraInventoryCount;
                calcHungerCheck += CardDictionary.globalCardDictionary[id].foodCost;
                switch (CardDictionary.globalCardDictionary[id].type)
                {
                    case CardsTypes.Electricity:
                        calcElectricity += CardDictionary.globalCardDictionary[id].typeValue;
                        break;
                    case CardsTypes.Gold:
                        calcGold += CardDictionary.globalCardDictionary[id].typeValue;
                        break;
                    case CardsTypes.Food:
                        calcFood += CardDictionary.globalCardDictionary[id].typeValue;
                        break;
                    case CardsTypes.Module:
                        calcResourceInventoryLimit += CardDictionary.globalCardDictionary[id].module.resourceInventoryIncrease;
                        calcInfraInventoryLimit += CardDictionary.globalCardDictionary[id].module.infraInventoryIncrease;
                        calcGoldGeneration += CardDictionary.globalCardDictionary[id].module.increaseGoldGeneration;
                        break;
                    default:
                        break;

                }

            }
        }
        _resourceInventoryUsed = calcResourceInventoryUsed;
        _resourceInventoryLimit = baseNodeStat.resourceInventoryLimit + calcResourceInventoryLimit;

        _infraInventoryUsed = calcInfraInventoryUsed;
        _infraInventoryLimit = baseNodeStat.infraInventoryLimit + calcInfraInventoryLimit;

        _goldGeneration = baseNodeStat.goldGeneration + calcGoldGeneration;
        _currentFoodCheck = baseNodeStat.currentFoodCheck + calcHungerCheck;
        _currentElectricity = calcElectricity;
        _currentGold = calcGold;
        _currentFood = calcFood;

        hungerSetIntervalTimer = baseNodeStat.hungerSetIntervalTimer;

    }



    private void Update()
    {
        intervalTimer = intervalTimer + Time.deltaTime;
        if (intervalTimer > hungerSetIntervalTimer)
        {
            handleHungerInterval();
            intervalTimer = 0;
        }
        if (!isProccessing)
        {
            StartCoroutine(processCards());

        }
        // que up the next cards
    }

    public IEnumerator processCards()
    {
        isProccessing = true;



        List<int> cardIds = activeStack.getCardIds();
        Debug.Log("Proccessing Card ..." + cardIds.Count);

        for (int index = 0; index < cardIds.Count; index++)
        {
            if (CardDictionary.globalProcessDictionary.ContainsKey(cardIds[index]))
            {
                List<int> clonedCardIds = new List<int>(cardIds);
                clonedCardIds.RemoveAt(index);

                Debug.Log("clonedCardIds/ [" + string.Join(",", clonedCardIds) + "]");

                foreach (RawProcessObject singleProcess in CardDictionary.globalProcessDictionary[cardIds[index]])
                {
                    int[] requiredIds = singleProcess.requiredIds;
                    Debug.Log("requiredIds/ [" + string.Join(",", requiredIds) + "]");
                    // IEnumerable<int> result = clonedCardIds.Intersect(requiredIds);

                    List<int> result = requiredIds.Where(i => clonedCardIds.Contains(i)).ToList();

                    Debug.Log("List Count/" + result.Count);

                    // int indexTemp = 0;
                    // foreach (int single in result)
                    // {
                    //     indexTemp++;
                    // }
                    // Debug.Log("indexTemp result /" + indexTemp);
                }
            }
        }


        yield return new WaitForSeconds(6);
        // yield return null;

        isProccessing = false;


    }




    private void handleHungerInterval()
    {
        computeStats();
        _currentFood = _currentFood - _currentFoodCheck;
        if (_currentFood <= 0)
        {
            handleFoodEmpty();
        }
        else
        {
            // Todo: handle food deletion
        }
    }

    private void handleFoodEmpty()
    {
        // Todo: handle empty food 
    }

    public CardStack getCardStack()
    {
        return activeStack;
    }


    private void reflectToScreen()
    {
        titleTextMesh.text = title;
        availableInventoryTextMesh.text = "" + resourceInventoryUsed + "/" + resourceInventoryLimit;
        hungerTextMesh.text = "" + _currentFoodCheck;
    }

}
