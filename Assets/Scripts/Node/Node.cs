using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
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

	// private TextMeshPro hungerTextMesh;
	private TextMeshPro processTimerTextMesh;

	public GameObject rootNodePlane;

	// -------------------- Custom Class -------------------------
	private CardStack activeStack;
	public NodePlaneHandler nodePlaneManager;

	// -------------------- Node Stats -------------------------

	public int id;

	private BaseNodeStats baseNodeStat;

	private int resourceInventoryLimit;

	private int resourceInventoryUsed;

	private int infraInventoryLimit;

	private int infraInventoryUsed;

	private int currentFoodCheck;

	private int currentElectricity;

	private int currentGold;

	private int goldGeneration;

	private int currentFood;

	public bool isActive;

	// -------------------- Meta Stats -------------------------

	public bool isNodePlaneActive;

	public NodeStateTypes nodeState;

	private float intervalTimer; // ********* Loop timer *********

	private int hungerSetIntervalTimer; // ********* sec, next time the check is applied  *********

	private bool isProccessing; // ********* sec, next time the check is applied  *********

	private float proccessingLeft; // ********* sec, how many seconds are left for process to finish  *********

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
		isActive = true;
		isNodePlaneActive = false;
		isProccessing = false;
		activeStack = new CardStack(CardStackType.Nodes);

		Component[] textMeshes = gameObject.GetComponentsInChildren(typeof(TextMeshPro));
		titleTextMesh = textMeshes[0] as TextMeshPro;
		availableInventoryTextMesh = textMeshes[1] as TextMeshPro;
		processTimerTextMesh = textMeshes[2] as TextMeshPro;

		initlizeBaseStats();
	}

	public void init()
	{
		if (CardDictionary.globalCardDictionary.ContainsKey(id))
		{
			if (titleTextMesh != null)
			{
				titleTextMesh.text = CardDictionary.globalCardDictionary[id].name;
			}
		}
		reflectToScreen();
		Vector3 spawningPosition = new Vector3(120, 0, 5);
		activeStack.cardBaseZ = spawningPosition.z + 1f;

		nodePlaneManager = gameObject.GetComponentInChildren(typeof(NodePlaneHandler), true) as NodePlaneHandler;
	}

	public void OnClick()
	{
		if (isNodePlaneActive == true)
		{
			nodePlaneManager.gameObject.SetActive(false);
		}
		else
		{
			nodePlaneManager.gameObject.SetActive(true);
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

	private void FixedUpdate()
	{
		intervalTimer = intervalTimer + Time.deltaTime;
		if (intervalTimer > hungerSetIntervalTimer)
		{
			handleHungerInterval();
			intervalTimer = 0;
		}
		if (!isProccessing)
		{
			if (isActive)
			{
				this.processCards();
			}
			else
			{
				this.handleInActiveNode();
			}
		}
	}

	private void handleHungerInterval()
	{
		computeStats();
		int foodMinus = currentFoodCheck;

		if (currentFood - foodMinus <= 0)
		{
			handleFoodEmpty();
		}
		else
		{
			StartCoroutine(handleTypeDeletion(CardsTypes.Food, foodMinus, 3f, null));
		}
	}

	private void handleFoodEmpty()
	{
		currentFood = 0;
		isActive = false;
	}

	public CardStack getCardStack()
	{
		return activeStack;
	}

	public IEnumerator sellCards(List<int> cardIds)
	{
		if (cardIds.Count == 0)
		{
			yield break;
		}
		float sellTimer = 5f;
		int goldAmount = this.getGoldAmount(cardIds);
		List<int> addingGoldCardIds = CardHelpers.generateTypeValueCards(CardsTypes.Gold, goldAmount);

		List<Card> removingCards = this.handleMarkingForRemoval(cardIds, sellTimer);

		// I think we are going to sell them one at a time (but for now we are having a hard static)
		yield return new WaitForSeconds(sellTimer);

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

	private void handleInActiveNode()
	{
		if (isMarket())
		{
			return;
		}

		float timer = 5f;
		int foodNeededToBeActive = currentFoodCheck * 3;

		isProccessing = true;
		bool shouldBeActive = currentFood > foodNeededToBeActive;
		if (shouldBeActive)
		{
			StartCoroutine(
				handleTypeDeletion(
					CardsTypes.Food,
					foodNeededToBeActive,
					timer,
					() =>
					{
						isActive = true;
						StartCoroutine(handleProcessFinish());
					}
				)
			);
		}
		else
		{
			StartCoroutine(handleProcessFinish());
		}
	}

	private IEnumerator handleTypeDeletion(CardsTypes cardType, int typeValue, float timer, Action callback)
	{
		List<int> removingCardIds = new List<int>();
		List<int> addingCardIds = new List<int>();

		List<int> cardIds = activeStack.getActiveCardIds();

		this.handleTypeProcess(cardIds, cardType, typeValue, ref removingCardIds, ref addingCardIds);

		List<Card> removingCards = this.handleMarkingForRemoval(removingCardIds, timer);
		computeStats();

		yield return new WaitForSeconds(timer);

		this.hadleRemovingCards(removingCards);
		this.handleCreatingCards(addingCardIds);

		if (callback != null)
		{
			callback.Invoke();
		}
	}

	private IEnumerator handleProcess(RawProcessObject pickedProcess)
	{
		List<int> removingCardIds = new List<int>();
		List<int> addingCardIds = new List<int>();

		AddingCardsObject pickedAddingCardId = this.pickAddingCardsObject(pickedProcess);
		if (pickedAddingCardId.isOneTime)
		{
			PlayerCardTracker.current.ensureOneTimeProcessTracked(pickedAddingCardId.id);
		}

		addingCardIds.AddRange(pickedAddingCardId.addingCardIds);

		List<int> cardIds = activeStack.getActiveCardIds();

		this.handleProcessCardIds(cardIds, pickedProcess, ref removingCardIds, ref addingCardIds);

		List<Card> removingCards = this.handleMarkingForRemoval(removingCardIds, pickedProcess.time);
		computeStats();
		StartCoroutine(handleProcessTimer(pickedProcess.time));

		yield return new WaitForSeconds(pickedProcess.time);

		if (isActive)
		{
			this.hadleRemovingCards(removingCards);
			this.handleCreatingCards(addingCardIds);
		}
		StartCoroutine(handleProcessFinish());
	}

	private IEnumerator handleProcessTimer(float processTime)
	{
		this.proccessingLeft = processTime;
		while (this.proccessingLeft > 0.1f)
		{
			this.proccessingLeft -= 1f;
			nodePlaneManager.textMesh.text = $"{this.proccessingLeft}";
			reflectToScreen();
			yield return new WaitForSeconds(1);
		}
	}

	private IEnumerator handleProcessFinish()
	{
		computeStats();
		yield return new WaitForSeconds(2);
		isProccessing = false;
	}

	private List<int> handleProcessCardIds(
		List<int> cardIds,
		RawProcessObject pickedProcess,
		ref List<int> removingCardIds,
		ref List<int> addingCardIds
	)
	{
		removingCardIds.Add(pickedProcess.baseCardId);
		removingCardIds.AddRange(pickedProcess.requiredIds);

		if (pickedProcess.requiredGold > 0)
		{
			this.handleTypeProcess(cardIds, CardsTypes.Gold, pickedProcess.requiredGold, ref removingCardIds, ref addingCardIds);
		}

		if (pickedProcess.requiredElectricity > 0)
		{
			this.handleTypeProcess(cardIds, CardsTypes.Electricity, pickedProcess.requiredElectricity, ref removingCardIds, ref addingCardIds);
		}

		return removingCardIds;
	}

	private void handleTypeProcess(
		List<int> availableCardIds,
		CardsTypes cardType,
		int requiredTypeValue,
		ref List<int> removingCardIds,
		ref List<int> addingCardIds
	)
	{
		int totalSum = 0;
		List<int> ascTypeCardIds = CardHelpers.getAscTypeValueCardIds(cardType, availableCardIds);
		foreach (int typeCardId in ascTypeCardIds)
		{
			removingCardIds.Add(typeCardId);
			totalSum = totalSum + CardDictionary.globalCardDictionary[typeCardId].typeValue;
			if (totalSum == requiredTypeValue)
			{
				break;
			}
			if (totalSum > requiredTypeValue)
			{
				int addingTypeValue = totalSum - requiredTypeValue;
				List<int> newCardIds = CardHelpers.generateTypeValueCards(cardType, addingTypeValue);
				addingCardIds.AddRange(newCardIds);
				break;
			}
		}
	}

	private void addCardsToCardStack(List<Card> newCards)
	{
		bool isRootCardChanged = activeStack.cards.Count == 0 ? true : false;
		activeStack.addCardsToStack(newCards);
		if (isRootCardChanged)
		{
			this.alignCardStacksPosition();
		}
		;

		if (isNodePlaneActive == false)
		{
			activeStack.changeActiveStateOfAllCards(false);
		}
	}

	private void alignCardStacksPosition()
	{
		activeStack.moveRootCardToPosition(nodePlaneManager.gameObject.transform.position.x, nodePlaneManager.gameObject.transform.position.y);
	}

	private RawProcessObject getAvailableProcess(List<int> cardIds)
	{
		RawProcessObject possibleProcesses = null;

		for (int index = 0; index < cardIds.Count; index++)
		{
			if (CardDictionary.globalProcessDictionary.ContainsKey(cardIds[index]))
			{
				// looping through all cards
				List<int> clonedCardIds = new List<int>(cardIds);
				clonedCardIds.RemoveAt(index);
				foreach (RawProcessObject singleProcess in CardDictionary.globalProcessDictionary[cardIds[index]])
				{
					Dictionary<int, int> indexedRequiredIds = this.indexCardIds(singleProcess.requiredIds.ToList());
					bool isUnlocked = this.isListUnlocked(singleProcess.unlockCardIds);

					bool ifRequiredCardsPassed = getIfRequiredCardsPassed(indexedRequiredIds, clonedCardIds);
					if (
						isUnlocked
						&& ifRequiredCardsPassed
						&& currentGold >= singleProcess.requiredGold
						&& currentElectricity >= singleProcess.requiredElectricity
					)
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

		bool getIfRequiredCardsPassed(Dictionary<int, int> indexedRequiredIds, List<int> clonedCardIds)
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
				// if (timer > 0)
				// {
				//     singleCard.timer = timer;
				// }
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
		resourceInventoryUsed = calcResourceInventoryUsed;
		resourceInventoryLimit = baseNodeStat.resourceInventoryLimit + calcResourceInventoryLimit;

		infraInventoryUsed = calcInfraInventoryUsed;
		infraInventoryLimit = baseNodeStat.infraInventoryLimit + calcInfraInventoryLimit;

		goldGeneration = baseNodeStat.goldGeneration + calcGoldGeneration;
		currentFoodCheck = baseNodeStat.currentFoodCheck + calcHungerCheck;
		currentElectricity = calcElectricity;
		currentGold = calcGold;
		currentFood = calcFood;

		hungerSetIntervalTimer = baseNodeStat.hungerSetIntervalTimer;
	}

	private AddingCardsObject pickAddingCardsObject(RawProcessObject pickedProcess)
	{
		float totalOddCount = 0;
		List<AddingCardsObject> addingCardList = pickedProcess.addingCardObjects.ToList();

		List<AddingCardsObject> fitleredList = addingCardList
			.Where(addingCardObject =>
			{
				if (addingCardObject.isOneTime)
				{
					// addingCardObject.id can't be inside CardTracker
					bool isOneTimeUnlocked = PlayerCardTracker.current.didPlayerUnlockOneTimeProcess(addingCardObject.id);
					if (isOneTimeUnlocked)
					{
						// Player Already unlocked this oneTimeReward
						return false;
					}
				}

				bool isUnlocked = this.isListUnlocked(addingCardObject.extraUnlockCardIds);
				return isUnlocked;
			})
			.ToList();

		// We need to filter here
		fitleredList.ForEach(addingCardObject =>
		{
			totalOddCount = totalOddCount + addingCardObject.odds;
		});
		float rndNumber = UnityEngine.Random.Range(0f, totalOddCount);
		float oddCount = 0;
		AddingCardsObject pickedAddingCardId = fitleredList.Find(addingCardObject =>
		{
			oddCount = oddCount + addingCardObject.odds;
			return oddCount > rndNumber;
		});

		if (pickedAddingCardId == null)
		{
			Debug.LogError("This should never happen (processCards)"); // error catch: This should never happen
			pickedAddingCardId = pickedProcess.addingCardObjects[0];
		}
		return pickedAddingCardId;
	}

	private bool isListUnlocked(int[] unlockCardIds)
	{
		bool isUnlocked = true;
		if (unlockCardIds.Length > 0)
		{
			foreach (int cardId in unlockCardIds)
			{
				if (!PlayerCardTracker.current.didPlayerUnlockCard(cardId))
				{
					isUnlocked = false;
					break;
				}
			}
		}
		return isUnlocked;
	}

	private void reflectToScreen()
	{
		availableInventoryTextMesh.text = $"{resourceInventoryUsed}/{resourceInventoryLimit}";

		if (isProccessing)
		{
			processTimerTextMesh.text = $"{proccessingLeft}";
		}
		else
		{
			processTimerTextMesh.text = "";
		}
	}
}
