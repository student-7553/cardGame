using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Core;
using System.Linq;
using Helpers;

public class NodeProcess : MonoBehaviour
{
	private Node node;

	public bool isOnCooldown; // *********  if currently on cd for processing *********

	public bool isProccessing; // *********  if currently processing cards *********

	public float proccessingLeft; // ********* sec, how many seconds are left for process to finish  *********

	// ********* Readonly static variables *********

	private readonly float processCooldown = 2f;

	private readonly float sellTimer = 1f;

	private bool isInit = false;

	public void Awake()
	{
		isProccessing = false;
	}

	public void init(Node parentNode)
	{
		isInit = true;
		node = parentNode;
	}

	public IEnumerator queUpTypeDeletion(CardsTypes cardType, int typeValue, float timer, Action callback, NodeCardStackType cardStackType)
	{
		List<int> removingCardIds = new List<int>();
		List<int> addingCardIds = new List<int>();
		List<int> cardIds =
			cardStackType == NodeCardStackType.process
				? node.processCardStack.getActiveCardIds()
				: node.storageCardStack.getActiveCardIds();

		this.handleTypeAdjusting(cardIds, cardType, typeValue, ref removingCardIds, ref addingCardIds);

		List<Card> removingCards = node.getCards(removingCardIds, cardStackType);
		foreach (Card card in removingCards)
		{
			card.disableInteractiveForATime(timer, CardDisableType.Process);
		}

		yield return new WaitForSeconds(timer);

		node.hadleRemovingCards(removingCards, cardStackType);
		node.handleCreatingCards(addingCardIds);

		if (callback != null)
		{
			callback.Invoke();
		}
	}

	public void handleTypeAdjusting(
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

	private void FixedUpdate()
	{
		this.handleFixedFrameCounter();
		if (!isInit)
		{
			return;
		}

		if (!this.isAvailableForNextProcess())
		{
			return;
		}

		this.isProccessing = true;

		if (node.isMarket())
		{
			// Market process
			this.handleMarketProcess();
		}
		else
		{
			// Node process
			this.handleNodeProcess();
		}
	}

	private void handleFixedFrameCounter()
	{
		if (this.proccessingLeft != 0f)
		{
			this.proccessingLeft = Math.Max(this.proccessingLeft - Time.fixedDeltaTime, 0f);
		}
	}

	private void handleMarketProcess()
	{
		// Market process
		List<int> activeCardsIds = node.processCardStack.getNonTypeActiveCardIds();
		if (activeCardsIds.Count == 0)
		{
			this.isProccessing = false;
			return;
		}

		StartCoroutine(sellCard(node.processCardStack.cards[0]));
	}

	private void handleNodeProcess()
	{
		if (!node.isActive)
		{
			this.handleInActiveNodeProcess();
		}
		else
		{
			this.handleActiveNodeProcess();
		}
	}

	private void handleActiveNodeProcess()
	{
		List<int> cardIds = node.processCardStack.getActiveCardIds();
		RawProcessObject pickedProcess = this.getAvailableProcess(cardIds, node.id);

		if (pickedProcess != null)
		{
			StartCoroutine(handleProcess(pickedProcess));
		}
		else
		{
			StartCoroutine(handleProcessCooldown());
		}
	}

	private void handleInActiveNodeProcess()
	{
		float timer = 5f;

		int foodNeededToBeActive = node.nodeStats.currentNodeStats.currentFoodCheck * 3;

		bool shouldBeActive = node.nodeStats.currentNodeStats.currentFood > foodNeededToBeActive;

		if (shouldBeActive)
		{
			StartCoroutine(
				this.queUpTypeDeletion(
					CardsTypes.Food,
					foodNeededToBeActive,
					timer,
					() =>
					{
						node.isActive = true;
						StartCoroutine(handleProcessCooldown());
					},
					NodeCardStackType.storage
				)
			);
		}
		else
		{
			StartCoroutine(handleProcessCooldown());
		}
	}

	private bool isAvailableForNextProcess()
	{
		if (isProccessing || isOnCooldown)
		{
			return false;
		}
		return true;
	}

	private RawProcessObject getAvailableProcess(List<int> cardIds, int nodeId)
	{
		RawProcessObject possibleProcesses = null;

		for (int index = 0; index < cardIds.Count; index++)
		{
			if (!CardDictionary.globalProcessDictionary.ContainsKey(cardIds[index]))
			{
				continue;
			}

			// looping through all cards
			List<int> clonedCardIds = new List<int>(cardIds);
			clonedCardIds.RemoveAt(index);

			foreach (RawProcessObject singleProcess in CardDictionary.globalProcessDictionary[cardIds[index]])
			{
				Dictionary<int, int> indexedRequiredIds = node.indexCardIds(singleProcess.requiredIds.ToList());

				bool isUnlocked = CardHandler.current.playerCardTracker.didPlayerUnlockCards(singleProcess.unlockCardIds);

				bool isInNode = singleProcess.inNodeId != 0 ? singleProcess.inNodeId == nodeId : true;
				bool ifRequiredCardsPassed = getIfRequiredCardsPassed(indexedRequiredIds, clonedCardIds);
				bool goldPassed = CardHelpers.getTypeValueFromCardIds(CardsTypes.Gold, cardIds) >= singleProcess.requiredGold;
				bool electricityPassed =
					CardHelpers.getTypeValueFromCardIds(CardsTypes.Electricity, cardIds) >= singleProcess.requiredElectricity;

				if (isUnlocked && isInNode && ifRequiredCardsPassed && goldPassed && electricityPassed)
				{
					possibleProcesses = singleProcess;
					break;
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
			foreach (int baseRequiredId in indexedRequiredIds.Keys)
			{
				int howManyRequired = indexedRequiredIds[baseRequiredId];
				int howManyIsAvailable = clonedCardIds.Where(x => x.Equals(baseRequiredId)).Count();
				if (howManyIsAvailable < howManyRequired)
				{
					isAvailableToProcess = false;
					break;
				}
			}
			return isAvailableToProcess;
		}
	}

	private IEnumerator handleProcess(RawProcessObject pickedProcess)
	{
		List<int> removingCardIds = new List<int>();
		List<int> addingCardIds = new List<int>();

		AddingCardsObject pickedAddingCardObject = pickAddingCardsObject(pickedProcess);
		if (pickedAddingCardObject.isOneTime)
		{
			CardHandler.current.playerCardTracker.ensureOneTimeProcessTracked(pickedAddingCardObject.id);
		}

		addingCardIds.AddRange(pickedAddingCardObject.addingCardIds);

		List<int> cardIds = node.processCardStack.getActiveCardIds();

		this.handleProcessAdjustingCardIds(cardIds, pickedProcess, ref removingCardIds, ref addingCardIds);

		List<Card> removingCards = node.getCards(removingCardIds, NodeCardStackType.process);

		List<int> restNonInteractiveCardIds = getRestNonInteractiveCardIds();

		List<Card> restNonInteractiveCards = node.getCards(restNonInteractiveCardIds, NodeCardStackType.process);

		foreach (Card card in restNonInteractiveCards)
		{
			card.disableInteractiveForATime(pickedProcess.time, CardDisableType.Process);
		}
		foreach (Card card in removingCards)
		{
			card.disableInteractiveForATime(pickedProcess.time, CardDisableType.Process);
		}

		this.proccessingLeft = pickedProcess.time;

		yield return new WaitForSeconds(pickedProcess.time);

		if (pickedAddingCardObject.updateCurrentNode != 0 && node.id < pickedAddingCardObject.updateCurrentNode)
		{
			node.id = pickedAddingCardObject.updateCurrentNode;
		}

		if (node.isActive)
		{
			node.hadleRemovingCards(removingCards, NodeCardStackType.process);
			node.handleCreatingCards(addingCardIds);
		}
		else
		{
			// Todo: handle failed process
			foreach (Card card in removingCards)
			{
				card.isInteractiveDisabled = false;
			}
		}

		foreach (Card card in restNonInteractiveCards)
		{
			card.isInteractiveDisabled = false;
		}
		node.processCardStack.removeCardsFromStack(restNonInteractiveCards);
		node.storageCardStack.addCardToStack(restNonInteractiveCards);
		node.consolidateTypeCards();

		StartCoroutine(handleProcessCooldown());

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

						// bool isOneTimeUnlocked = PlayerCardTracker.current.didPlayerUnlockOneTimeProcess(addingCardObject.id);
						bool isOneTimeUnlocked = CardHandler.current.playerCardTracker.didPlayerUnlockOneTimeProcess(addingCardObject.id);
						if (isOneTimeUnlocked)
						{
							// Player Already unlocked this oneTimeReward
							return false;
						}
					}

					bool isUnlocked = CardHandler.current.playerCardTracker.didPlayerUnlockCards(addingCardObject.extraUnlockCardIds);
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
				Debug.LogError("This should never happen (nodeProcessCards)"); // error catch: This should never happen
				pickedAddingCardId = pickedProcess.addingCardObjects[0];
			}
			return pickedAddingCardId;
		}

		List<int> getRestNonInteractiveCardIds()
		{
			List<int> restNonInteractiveCardIds = pickedProcess.requiredIds.ToList();
			restNonInteractiveCardIds.Add(pickedProcess.baseRequiredId);

			foreach (int removingCardId in removingCardIds)
			{
				int foundId = restNonInteractiveCardIds.FindIndex((cardId) => cardId == removingCardId);
				if (foundId != -1)
				{
					restNonInteractiveCardIds.RemoveAt(foundId);
				}
			}
			return restNonInteractiveCardIds;
		}
	}

	private IEnumerator handleProcessCooldown()
	{
		this.isProccessing = false;
		this.isOnCooldown = true;
		yield return new WaitForSeconds(processCooldown);
		this.isOnCooldown = false;
	}

	private void handleProcessAdjustingCardIds(
		List<int> cardIds,
		RawProcessObject pickedProcess,
		ref List<int> removingCardIds,
		ref List<int> addingCardIds
	)
	{
		removingCardIds.AddRange(pickedProcess.removingIds);

		if (pickedProcess.requiredGold > 0)
		{
			this.handleTypeAdjusting(cardIds, CardsTypes.Gold, pickedProcess.requiredGold, ref removingCardIds, ref addingCardIds);
		}

		if (pickedProcess.requiredElectricity > 0)
		{
			this.handleTypeAdjusting(
				cardIds,
				CardsTypes.Electricity,
				pickedProcess.requiredElectricity,
				ref removingCardIds,
				ref addingCardIds
			);
		}
		return;
	}

	private IEnumerator sellCard(Card card)
	{
		if (card == null)
		{
			yield break;
		}

		int goldAmount = this.getGoldAmount(card.id);
		List<int> addingGoldCardIds = CardHelpers.generateTypeValueCards(CardsTypes.Gold, goldAmount);

		card.disableInteractiveForATime(sellTimer, CardDisableType.Process);

		yield return new WaitForSeconds(sellTimer);

		List<Card> removingCards = new List<Card> { card };

		node.hadleRemovingCards(removingCards, NodeCardStackType.process);
		node.handleCreatingCards(addingGoldCardIds);
		node.consolidateTypeCards();

		isProccessing = false;
	}

	private int getGoldAmount(int cardId)
	{
		return CardDictionary.globalCardDictionary[cardId].sellingPrice;
	}
}
