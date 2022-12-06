
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using Core;
using Helpers;

[System.Serializable]
public class BaseNodeStats
{
    public int infraInventoryLimit;
    public int resourceInventoryLimit;

    public int currentFoodCheck;
    public int goldGeneration;
    public int hungerSetIntervalTimer;
}

public class Node : MonoBehaviour, IStackable, IClickable
{
    // -------------------- Unity Component -------------------------
    private TextMeshPro titleTextMesh;
    private TextMeshPro availableInventoryTextMesh;
    private TextMeshPro hungerTextMesh;
    public GameObject rootNodePlane;

    // -------------------- Custom Class -------------------------
    private CardStack activeStack;
    public NodePlaneHandler nodePlaneManagers;


    // -------------------- Node Stats -------------------------

    public int id;

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

    public bool isActive;

    public NodeStateTypes nodeState;

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
        activeStack.cardBaseZ = spawningPosition.z + 1f;

        nodePlaneManagers = gameObject.GetComponentInChildren(typeof(NodePlaneHandler), true) as NodePlaneHandler;

    }

    public void OnClick()
    {
        if (isActive == true)
        {
            nodePlaneManagers.gameObject.SetActive(false);
        }
        else
        {
            nodePlaneManagers.gameObject.SetActive(true);
        }
    }

    public void stackOnThis(List<Card> newCards)
    {
        // base nodes
        this.addCardsToCardStack(newCards);
        computeStats();
        if (isMarket())
        {
            //  Market
            List<int> cardIds = activeStack.getNonTypeCardIds();
            StartCoroutine(sellCards(cardIds));
        }
    }

    private bool isMarket()
    {
        if (nodeState == NodeStateTypes.market_1)
        {
            return true;
        }
        return false;
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
            this.processCards();

        }
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

        if (CardDictionary.globalCardDictionary.ContainsKey(id))
        {
            if (titleTextMesh != null)
            {
                titleTextMesh.text = CardDictionary.globalCardDictionary[id].name;
            }
        }
        availableInventoryTextMesh.text = "" + resourceInventoryUsed + "/" + resourceInventoryLimit;
        hungerTextMesh.text = "" + _currentFoodCheck;
    }

    public IEnumerator sellCards(List<int> cardIds)
    {
        if (cardIds.Count == 0)
        {
            yield break;
        }

        int goldAmount = this.getGoldAmount(cardIds);
        List<int> addingGoldCardIds = CardHelpers.generateTypeValueCards(CardsTypes.Gold, goldAmount);


        List<Card> removingCards = this.handleMarkingForRemoval(cardIds, 5f);

        // I think we are going to sell them one at a time (but for now we are having a hard static)
        yield return new WaitForSeconds(5f);

        this.hadleRemovingCards(removingCards);
        this.handleCreatingCards(addingGoldCardIds);

    }

    public void processCards()
    {
        if (isMarket())
        {
            return;
        }

        isProccessing = true;
        List<int> cardIds = activeStack.getAllCardIds();
        RawProcessObject pickedProcess = this.getAvailableProcess(cardIds);
        if (pickedProcess != null)
        {
            StartCoroutine(handleProcess(pickedProcess));
        }
        else
        {
            StartCoroutine(handleProcessFinish());
        }
    }

    private IEnumerator handleProcess(RawProcessObject pickedProcess)
    {

        List<int> removingCardIds = new List<int>();
        List<int> addingCardIds = new List<int>();

        float totalOddCount = 0;
        List<AddingCardsObject> addingCardList = pickedProcess.addingCardObjects.ToList();
        addingCardList.ForEach(addingCardObject =>
        {
            totalOddCount = totalOddCount + addingCardObject.odds;
        });
        float rndNumber = Random.Range(0f, totalOddCount);
        float oddCount = 0;
        AddingCardsObject pickedAddingCardId = addingCardList.Find(addingCardObject =>
        {
            oddCount = oddCount + addingCardObject.odds;
            return oddCount > rndNumber;
        });

        if (pickedAddingCardId == null)
        {
            Debug.LogError("This should never happen (processCards)");             // error catch: This should never happen 
            pickedAddingCardId = pickedProcess.addingCardObjects[0];
        }

        addingCardIds.AddRange(pickedAddingCardId.addingCardIds);

        List<int> cardIds = activeStack.getAllCardIds();

        this.handleProcessCardIds(cardIds, pickedProcess, ref removingCardIds, ref addingCardIds);

        List<Card> removingCards = this.handleMarkingForRemoval(removingCardIds, pickedProcess.time);
        computeStats();

        yield return new WaitForSeconds(pickedProcess.time);

        this.hadleRemovingCards(removingCards);

        this.handleCreatingCards(addingCardIds);

        StartCoroutine(handleProcessFinish());

    }

    private IEnumerator handleProcessFinish()
    {
        yield return new WaitForSeconds(2);
        isProccessing = false;
    }

    private List<int> handleProcessCardIds(List<int> cardIds, RawProcessObject pickedProcess, ref List<int> removingCardIds, ref List<int> addingCardIds)
    {

        removingCardIds.Add(pickedProcess.baseCardId);
        removingCardIds.AddRange(pickedProcess.requiredIds);

        if (pickedProcess.requiredGold > 0)
        {
            int totalSum = 0;
            List<int> ascGoldCardIds = CardHelpers.getAscTypeValueCardIds(CardsTypes.Gold, cardIds);
            foreach (int goldCardId in ascGoldCardIds)
            {
                removingCardIds.Add(goldCardId);
                totalSum = totalSum + CardDictionary.globalCardDictionary[goldCardId].typeValue;
                if (totalSum == pickedProcess.requiredGold)
                {
                    break;
                }
                if (totalSum > pickedProcess.requiredGold)
                {
                    int addingTypeValue = totalSum - pickedProcess.requiredGold;
                    List<int> newCardIds = CardHelpers.generateTypeValueCards(CardsTypes.Gold, addingTypeValue);
                    addingCardIds.AddRange(newCardIds);
                    break;
                }
            }
        }

        if (pickedProcess.requiredElectricity > 0)
        {
            int totalSum = 0;
            List<int> ascElectricityCardIds = CardHelpers.getAscTypeValueCardIds(CardsTypes.Electricity, cardIds);
            foreach (int cardId in ascElectricityCardIds)
            {
                removingCardIds.Add(cardId);
                totalSum = totalSum + CardDictionary.globalCardDictionary[cardId].typeValue;
                if (totalSum == pickedProcess.requiredElectricity)
                {
                    break;
                }
                if (totalSum > pickedProcess.requiredElectricity)
                {
                    int addingTypeValue = totalSum - pickedProcess.requiredElectricity;
                    List<int> addingCards = CardHelpers.generateTypeValueCards(CardsTypes.Electricity, addingTypeValue);
                    addingCardIds.AddRange(addingCards);
                    break;
                }
            }
        }

        return removingCardIds;
    }

    private void addCardsToCardStack(List<Card> newCards)
    {
        bool isRootCardChanged = activeStack.cards.Count == 0 ? true : false;
        activeStack.addCardsToStack(newCards);
        if (isRootCardChanged) this.alignCardStacksPosition();

        if (isActive == false)
        {
            activeStack.changeActiveStateOfAllCards(false);
        }

    }

    private void alignCardStacksPosition()
    {
        activeStack.moveRootCardToPosition(nodePlaneManagers.gameObject.transform.position.x,
            nodePlaneManagers.gameObject.transform.position.y);
    }

    private RawProcessObject getAvailableProcess(List<int> cardIds)
    {
        RawProcessObject possibleProcesses = null;

        for (int index = 0; index < cardIds.Count; index++)
        {
            // looping through all cards
            List<int> clonedCardIds = new List<int>(cardIds);
            clonedCardIds.RemoveAt(index);
            if (CardDictionary.globalProcessDictionary.ContainsKey(cardIds[index]))
            {
                foreach (RawProcessObject singleProcess in CardDictionary.globalProcessDictionary[cardIds[index]])
                {
                    Dictionary<int, int> indexedRequiredIds = this.indexCardIds(singleProcess.requiredIds.ToList());
                    bool ifRequiredCardsPassed = this.ifRequiredCardsPassed(indexedRequiredIds, clonedCardIds);
                    if (ifRequiredCardsPassed && currentGold >= singleProcess.requiredGold && currentElectricity >= singleProcess.requiredElectricity)
                    {
                        possibleProcesses = singleProcess;
                        break;
                    }
                }
            }
            if (possibleProcesses != null)
            {
                break;
            }
        }
        return possibleProcesses;
    }

    private Dictionary<int, int> indexCardIds(List<int> requiredIds)
    {
        Dictionary<int, int> indexedRequiredIds = new Dictionary<int, int>();
        foreach (int requiredId in requiredIds)
        {
            if (indexedRequiredIds.ContainsKey(requiredId))
            {
                indexedRequiredIds[requiredId] = indexedRequiredIds[requiredId] + 1;
            }
            else
            {
                indexedRequiredIds.Add(requiredId, 1);
            }
        }
        return indexedRequiredIds;
    }

    private bool ifRequiredCardsPassed(Dictionary<int, int> indexedRequiredIds, List<int> clonedCardIds)
    {
        bool isAvailableToProcess = true;
        foreach (int requiredId in indexedRequiredIds.Keys)
        {
            int howManyRequired = indexedRequiredIds[requiredId];
            int howManyIsAvailable = clonedCardIds.Where(x => x.Equals(requiredId)).Count();
            if (howManyIsAvailable < howManyRequired)
            {
                isAvailableToProcess = false;
                break;
            }
        }
        return isAvailableToProcess;
    }

    private int getGoldAmount(List<int> cardIds)
    {
        int goldAmount = 0;
        foreach (int cardId in cardIds)
        {
            if (CardDictionary.globalCardDictionary[cardId].sellingPrice > 0)
            {
                goldAmount = goldAmount + CardDictionary.globalCardDictionary[cardId].sellingPrice;
            }

        }
        return goldAmount;
    }

    private void hadleRemovingCards(List<Card> removingCards)
    {

        activeStack.removeCardsFromStack(removingCards);

        foreach (Card singleRemovingCard in removingCards)
        {
            Destroy(singleRemovingCard.gameObject);
        }
    }

    private void handleCreatingCards(List<int> cardIds)
    {
        List<Card> addingCards = new List<Card>();
        foreach (int singleAddingCardId in cardIds)
        {
            if (CardDictionary.globalCardDictionary[singleAddingCardId].type == CardsTypes.Node)
            {
                // we are creating a Node
                CardHandler.current.createNode(singleAddingCardId);
            }
            else
            {
                Card createdCard = CardHandler.current.createCard(singleAddingCardId);
                addingCards.Add(createdCard);
            }

        }
        this.addCardsToCardStack(addingCards);
    }

    private List<Card> handleMarkingForRemoval(List<int> cardIds, float timer = 0)
    {

        Dictionary<int, int> indexedRemovingCardIds = this.indexCardIds(cardIds);
        List<Card> removedCards = new List<Card>();
        foreach (Card singleCard in activeStack.cards)
        {
            if (indexedRemovingCardIds.ContainsKey(singleCard.id))
            {
                indexedRemovingCardIds[singleCard.id]--;
                if (indexedRemovingCardIds[singleCard.id] == 0)
                {
                    indexedRemovingCardIds.Remove(singleCard.id);
                }
                removedCards.Add(singleCard);
                singleCard.isDisabled = true;
                if (timer > 0)
                {
                    singleCard.timer = timer;
                }
                singleCard.reflectScreen();
            }
        }

        return removedCards;
    }

    private void computeStats()
    {
        List<int> cardIds = activeStack.getActiveCardIds();
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

}
