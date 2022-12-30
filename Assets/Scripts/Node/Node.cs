using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using Core;
using Helpers;

public class Node : MonoBehaviour, IStackable, IClickable
{
	// -------------------- Custom Class -------------------------
	public NodeCardQue nodeCardQue;
	public NodeTextHandler nodeTextHandler;
	public NodeStats nodeStats;

	public NodePlaneHandler nodePlaneManager;
	public CardStack activeStack;

	// // -------------------- Node Stats -------------------------

	public int id;

	public bool isActive;

	// -------------------- Meta Stats -------------------------

	public NodeStateTypes nodeState;

	public bool isProccessing; // ********* sec, next time the check is applied  *********

	public float proccessingLeft; // ********* sec, how many seconds are left for process to finish  *********

	// --------------------Readonly Stats-------------------------

	private readonly float nodePlaneBaseZ = 3f;

	private void Awake()
	{
		isActive = true;
		isProccessing = false;
		activeStack = new CardStack(CardStackType.Nodes, this);

		nodeCardQue = gameObject.AddComponent<NodeCardQue>();
		nodeTextHandler = gameObject.AddComponent<NodeTextHandler>();
		nodeStats = new NodeStats(this);
	}

	public void init()
	{
		Vector3 spawningPosition = new Vector3(120, 0, 5);
		activeStack.cardBaseZ = spawningPosition.z + 1f;
		nodePlaneManager = gameObject.GetComponentInChildren(typeof(NodePlaneHandler), true) as NodePlaneHandler;
	}

	public void OnClick()
	{
		if (nodePlaneManager.gameObject.activeSelf == true)
		{
			nodePlaneManager.gameObject.SetActive(false);
		}
		else
		{
			// alignCardStacksPosition();
			nodePlaneManager.gameObject.SetActive(true);
		}
	}

	public void stackOnThis(List<Card> newCards)
	{
		this.addCardsToCardStack(newCards);
		if (isMarket())
		{
			List<int> cardIds = activeStack.getNonTypeCardIds();
			StartCoroutine(sellCards(cardIds));
		}
		else
		{
			nodeCardQue.addCards(newCards);
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

		nodeStats.computeStats();
		nodeStats.handleLimits();
	}

	private IEnumerator sellCards(List<int> cardIds)
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
		int foodNeededToBeActive = nodeStats.currentNodeStats.currentFoodCheck * 3;

		isProccessing = true;
		bool shouldBeActive = nodeStats.currentNodeStats.currentFood > foodNeededToBeActive;
		if (shouldBeActive)
		{
			StartCoroutine(
				handleCardTypeDeletion(
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

	public IEnumerator handleCardTypeDeletion(CardsTypes cardType, int typeValue, float timer, Action callback)
	{
		List<int> removingCardIds = new List<int>();
		List<int> addingCardIds = new List<int>();

		List<int> cardIds = activeStack.getActiveCardIds();

		this.handleTypeProcess(cardIds, cardType, typeValue, ref removingCardIds, ref addingCardIds);

		List<Card> removingCards = this.handleMarkingForRemoval(removingCardIds, timer);

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

		AddingCardsObject pickedAddingCardId = pickAddingCardsObject(pickedProcess);
		if (pickedAddingCardId.isOneTime)
		{
			PlayerCardTracker.current.ensureOneTimeProcessTracked(pickedAddingCardId.id);
		}

		addingCardIds.AddRange(pickedAddingCardId.addingCardIds);

		List<int> cardIds = activeStack.getActiveCardIds();

		this.handleProcessAdjustingCardIds(cardIds, pickedProcess, ref removingCardIds, ref addingCardIds);

		List<Card> removingCards = this.handleMarkingForRemoval(removingCardIds, pickedProcess.time);
		StartCoroutine(handleProcessTimer(pickedProcess.time));

		yield return new WaitForSeconds(pickedProcess.time);

		if (isActive)
		{
			this.hadleRemovingCards(removingCards);
			this.handleCreatingCards(addingCardIds);
		}
		StartCoroutine(handleProcessFinish());

		AddingCardsObject pickAddingCardsObject(RawProcessObject pickedProcess)
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

					bool isUnlocked = this.isUnlocked(addingCardObject.extraUnlockCardIds);
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
	}

	private IEnumerator handleProcessTimer(float processTime)
	{
		this.proccessingLeft = processTime;
		while (this.proccessingLeft > 0.1f)
		{
			this.proccessingLeft -= 1f;
			nodePlaneManager.textMesh.text = $"{this.proccessingLeft}";
			yield return new WaitForSeconds(1);
		}
	}

	private IEnumerator handleProcessFinish()
	{
		yield return new WaitForSeconds(2);
		isProccessing = false;
	}

	private List<int> handleProcessAdjustingCardIds(
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
			this.handleTypeProcess(
				cardIds,
				CardsTypes.Electricity,
				pickedProcess.requiredElectricity,
				ref removingCardIds,
				ref addingCardIds
			);
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
			// Todo: Move this to a hook inside cardStack
			// this.alignCardStacksPosition();
		}
	}

	private RawProcessObject getAvailableProcess(List<int> cardIds)
	{
		RawProcessObject possibleProcesses = null;

		for (int index = 0; index < cardIds.Count; index++)
		{
			// if (CardDictionary.globalProcessDictionary.ContainsKey(cardIds[index]))
			// {
			// looping through all cards
			List<int> clonedCardIds = new List<int>(cardIds);
			clonedCardIds.RemoveAt(index);
			foreach (RawProcessObject singleProcess in CardDictionary.globalProcessDictionary[cardIds[index]])
			{
				Dictionary<int, int> indexedRequiredIds = this.indexCardIds(singleProcess.requiredIds.ToList());
				bool isUnlocked = this.isUnlocked(singleProcess.unlockCardIds);

				bool ifRequiredCardsPassed = getIfRequiredCardsPassed(indexedRequiredIds, clonedCardIds);
				if (
					isUnlocked
					&& ifRequiredCardsPassed
					&& nodeStats.currentNodeStats.currentGold >= singleProcess.requiredGold
					&& nodeStats.currentNodeStats.currentElectricity >= singleProcess.requiredElectricity
				)
				{
					possibleProcesses = singleProcess;
					break;
				}
			}
			// }
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
		foreach (Card singleRemovingCard in removingCards)
		{
			Destroy(singleRemovingCard.gameObject);
		}
		activeStack.removeCardsFromStack(removingCards);
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

	public void ejectCards(List<Card> cards)
	{
		int positionMinusInterval = 4;
		Vector3 startingPosition = new Vector3(
			gameObject.transform.position.x,
			gameObject.transform.position.y - 10,
			gameObject.transform.position.z
		);
		activeStack.removeCardsFromStack(cards);

		foreach (Card card in cards)
		{
			Vector3 cardPostion = startingPosition;
			cardPostion.y = cardPostion.y - positionMinusInterval;
			card.moveCard(cardPostion);
		}
	}

	private bool isUnlocked(int[] unlockCardIds)
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
}
