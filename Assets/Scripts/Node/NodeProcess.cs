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

	private bool isInit = false;

	public void Awake()
	{
		isProccessing = false;
	}

	private void FixedUpdate()
	{
		if (!isInit)
		{
			return;
		}

		if (!isAvailableForNextProcess())
		{
			return;
		}

		if (node.isMarket())
		{
			// Market process
			handleMarketProcess();
		}
		else
		{
			// Node process
			handleNodeProcess();
		}
	}

	public void init(Node parentNode)
	{
		isInit = true;
		node = parentNode;
	}

	public IEnumerator queUpTypeDeletion(CardsTypes cardType, int typeValue, float timer, Action callback)
	{
		List<Card> cards = node.processCardStack.getActiveCards();

		TypeAdjustingData data = CardHelpers.handleTypeAdjusting(cards, cardType, typeValue);

		if (timer > 0)
		{
			foreach (Card card in data.removingCards)
			{
				card.disableInteractiveForATime(timer, CardDisableType.Process);
			}
			yield return new WaitForSeconds(timer);
		}

		node.hadleRemovingCards(data.removingCards);
		List<Card> addingCards = node.handleCreatingCards(data.addingCardIds);
		if (addingCards != null && addingCards.Count > 0)
		{
			List<Card> ejectingCards = addingCards
				.Where(
					(card) =>
					{
						return CardHelpers.isNonValueTypeCard(CardDictionary.globalCardDictionary[card.id].type);
					}
				)
				.ToList();
			if (ejectingCards.Count > 0)
			{
				node.ejectCards(ejectingCards);
			}
		}

		if (callback != null)
		{
			callback.Invoke();
		}
	}

	private void handleMarketProcess()
	{
		Card sellingCard = node.processCardStack.cards.Find((card) => CardDictionary.globalCardDictionary[card.id].type != CardsTypes.Gold);

		if (sellingCard == null)
		{
			return;
		}
		StartCoroutine(sellCard(sellingCard));
	}

	private void handleNodeProcess()
	{
		if (node.isActive)
		{
			handleActiveNodeProcess();
		}
		else
		{
			handleInActiveNodeProcess();
		}
	}

	private void handleActiveNodeProcess()
	{
		List<int> cardIds = node.processCardStack.getActiveCardIds();
		RawProcessObject pickedProcess = getAvailableProcess(cardIds, node.id);

		if (pickedProcess != null)
		{
			StartCoroutine(handleProcess(pickedProcess, false));
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
			isProccessing = true;
			StartCoroutine(
				queUpTypeDeletion(
					CardsTypes.Food,
					foodNeededToBeActive,
					timer,
					() =>
					{
						isProccessing = false;
						node.isActive = true;
						StartCoroutine(handleProcessCooldown());
					}
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
				Dictionary<int, int> indexedRequiredIds = CardHelpers.indexCardIds(singleProcess.requiredIds.ToList());

				bool isUnlocked = CardHandler.current.playerCardTracker.didPlayerUnlockCards(singleProcess.unlockCardIds);

				bool isRightNode = getIfInRightNodePassed(singleProcess, nodeId);
				bool ifRequiredCardsPassed = getIfRequiredCardsPassed(indexedRequiredIds, clonedCardIds);
				bool goldPassed = CardHelpers.getTypeValueFromCardIds(CardsTypes.Gold, cardIds) >= singleProcess.requiredGold;
				bool electricityPassed =
					CardHelpers.getTypeValueFromCardIds(CardsTypes.Electricity, cardIds) >= singleProcess.requiredElectricity;
				bool willPassed = CardHelpers.getTypeValueFromCardIds(CardsTypes.Will, cardIds) >= singleProcess.requiredWill;

				if (isUnlocked && isRightNode && ifRequiredCardsPassed && goldPassed && electricityPassed && willPassed)
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

	bool getIfInRightNodePassed(RawProcessObject singleProcess, int nodeId)
	{
		if (
			singleProcess.nodeRequirement != null
			&& singleProcess.nodeRequirement.mustBeNodeIds != null
			&& singleProcess.nodeRequirement.mustBeNodeIds.Length != 0
		)
		{
			return singleProcess.nodeRequirement.mustBeNodeIds.Contains(nodeId);
		}
		return true;
	}

	private IEnumerator handleProcess(RawProcessObject pickedProcess, bool isCombo)
	{
		isProccessing = true;
		List<int> addingCardIds = new List<int>();

		AddingCardsObject pickedAddingCardObject = pickAddingCardsObject(pickedProcess);
		if (pickedAddingCardObject.isOneTime)
		{
			CardHandler.current.playerCardTracker.ensureOneTimeProcessTracked(pickedAddingCardObject.id);
		}

		List<int> addingCardsFromProcess = GetAddingCards(pickedAddingCardObject);
		addingCardIds.AddRange(addingCardsFromProcess);

		List<Card> activeCards = node.processCardStack.getActiveCards();

		TypeAdjustingData adjData = handleProcessAdjustingCardIds(activeCards, pickedProcess);

		addingCardIds.AddRange(adjData.addingCardIds);

		List<Card> removingCards = adjData.removingCards;

		List<int> restNonInteractiveCardIds = getRestNonInteractiveCardIds();

		List<Card> restNonInteractiveCards = node.processCardStack.getCards(restNonInteractiveCardIds);

		foreach (Card card in restNonInteractiveCards)
		{
			card.disableInteractiveForATime(pickedProcess.time, CardDisableType.Process);
		}
		foreach (Card card in removingCards)
		{
			card.disableInteractiveForATime(pickedProcess.time, CardDisableType.Process);
		}

		proccessingLeft = getProcessTime(pickedProcess, isCombo);

		if (isCombo)
		{
			GameManager.current.SpawnFloatingText("COMBO", transform.position);
		}

		while (proccessingLeft > 0)
		{
			yield return new WaitForSeconds(1);
			proccessingLeft = proccessingLeft - 1;

			if (proccessingLeft > node.staticVariables.bufferProcessingTime)
			{
				// Is not in the buffer zone
				if (node.nodeStats.currentNodeStats.currentElectricity > 0)
				{
					// electricity got updated
					int electricityRemove = getElectricityToRemoveFromProcessTime(
						(int)proccessingLeft,
						node.nodeStats.currentNodeStats.currentElectricity,
						isCombo
					);
					StartCoroutine(queUpTypeDeletion(CardsTypes.Electricity, electricityRemove, 0, null));
					proccessingLeft = proccessingLeft - electricityToTime(electricityRemove);
				}
			}
		}

		if (pickedAddingCardObject.updateCurrentNode != 0 && node.id < pickedAddingCardObject.updateCurrentNode)
		{
			node.id = pickedAddingCardObject.updateCurrentNode;
		}

		List<Card> ejectingCards = new List<Card>();
		if (node.isActive)
		{
			node.hadleRemovingCards(removingCards);
			List<Card> addingCards = node.handleCreatingCards(addingCardIds);

			if (pickedProcess.id == 34 || pickedProcess.id == 35 || pickedProcess.id == 28)
			{
				ejectingCards.AddRange(addingCards);
			}
			else
			{
				ejectingCards.AddRange(
					addingCards.Where(
						(card) =>
						{
							return CardHelpers.isNonValueTypeCard(CardDictionary.globalCardDictionary[card.id].type)
								|| CardDictionary.globalCardDictionary[card.id].type == CardsTypes.Electricity;
						}
					)
				);
			}
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

		foreach (int newCardId in addingCardsFromProcess)
		{
			GameManager.current.SpawnFloatingText("[" + CardDictionary.globalCardDictionary[newCardId].name + "]", transform.position);
		}

		ejectingCards.AddRange(
			restNonInteractiveCards.Where(
				(card) =>
				{
					if (CardDictionary.globalCardDictionary[card.id].type == CardsTypes.Infrastructure)
					{
						return false;
					}
					return CardHelpers.isNonValueTypeCard(CardDictionary.globalCardDictionary[card.id].type);
				}
			)
		);

		node.ejectCards(ejectingCards);
		node.consolidateTypeCards();

		isProccessing = false;

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

			foreach (Card removingCard in removingCards)
			{
				int foundId = restNonInteractiveCardIds.FindIndex((cardId) => cardId == removingCard.id);
				if (foundId != -1)
				{
					restNonInteractiveCardIds.RemoveAt(foundId);
				}
			}
			return restNonInteractiveCardIds;
		}
	}

	private int getProcessTime(RawProcessObject pickedProcess, bool isCombo)
	{
		List<int> minusIntervalModuleIds = node.processCardStack.getAllCardIdsOfMinusIntervalModules();
		int processTime = pickedProcess.time;
		foreach (int cardId in minusIntervalModuleIds)
		{
			if (CardDictionary.globalCardDictionary[cardId].module.minusInterval.processIds.Contains(pickedProcess.id))
			{
				processTime = Math.Max(
					processTime - CardDictionary.globalCardDictionary[cardId].module.minusInterval.time,
					// bufferProcessingTime
					node.staticVariables.bufferProcessingTime
				);
			}
		}

		if (isCombo)
		{
			processTime = Math.Max(processTime - node.staticVariables.comboTimeFlatMinus, node.staticVariables.processingTimeMin);
		}

		if (node.nodeStats.currentNodeStats.currentElectricity > node.staticVariables.processingTimeMin)
		{
			int electricityRemove = getElectricityToRemoveFromProcessTime(
				pickedProcess.time,
				node.nodeStats.currentNodeStats.currentElectricity,
				false
			);
			StartCoroutine(queUpTypeDeletion(CardsTypes.Electricity, electricityRemove, 0, null));
			processTime = pickedProcess.time - electricityToTime(electricityRemove);
		}

		return processTime;
	}

	private List<int> GetAddingCards(AddingCardsObject pickedAddingCardObject)
	{
		List<int> addingCardIds = pickedAddingCardObject.addingCardIds.ToList();
		List<int> cardIds = node.processCardStack.getActiveCardIds();

		List<int> unityModuleCount = node.processCardStack.getAllCardIdsOfUnityModules();

		int totalUnityValue = unityModuleCount.Aggregate(
			0,
			(total, cardId) =>
			{
				return total + CardDictionary.globalCardDictionary[cardId].module.unityCount;
			}
		);

		if (pickedAddingCardObject.id == 5176)
		{
			// temp logic
			// equals the food process
			// food outcome is getting multiplied by how many food infra cards there are
			int basicFoodId = 10200;
			int foodInfraId = 1001;

			int infraCount = cardIds.Where((cardId) => cardId == foodInfraId).Count();
			if (infraCount > 1)
			{
				for (int index = 1; index < infraCount; index++)
				{
					addingCardIds.Add(basicFoodId);
				}
			}

			if (totalUnityValue > 0)
			{
				// Double the food count if unity is higher than 0
				int totalFoodCount = addingCardIds.Where((cardId) => cardId == basicFoodId).Count();
				for (int index = 0; index < totalFoodCount; index++)
				{
					addingCardIds.Add(basicFoodId);
				}
			}
		}
		if (pickedAddingCardObject.id == 632030)
		{
			// temp logic
			// equals the electricity process
			// food outcome is getting multiplied by how many food infra cards there are\
			int basicElectricityId = 10100;
			int electricityInfraId = 1002;

			int infraCount = cardIds.Where((cardId) => cardId == electricityInfraId).Count();
			if (infraCount > 1)
			{
				for (int index = 1; index < infraCount; index++)
				{
					addingCardIds.Add(basicElectricityId);
				}
			}

			if (totalUnityValue > 0)
			{
				// Double the food count if unity is higher than 0
				int totalElectricityCount = addingCardIds.Where((cardId) => cardId == basicElectricityId).Count();
				for (int index = 0; index < totalElectricityCount; index++)
				{
					addingCardIds.Add(basicElectricityId);
				}
			}
		}

		// everything else
		return addingCardIds;
	}

	private IEnumerator handleProcessCooldown()
	{
		// isProccessing = false;
		RawProcessObject nextProcess = getNextAvailableProcess();
		if (nextProcess != null)
		{
			// Combo start
			isOnCooldown = false;
			StartCoroutine(handleProcess(nextProcess, true));
			yield break;
		}

		isOnCooldown = true;
		yield return new WaitForSeconds(node.staticVariables.processCooldown);
		isOnCooldown = false;
	}

	private RawProcessObject getNextAvailableProcess()
	{
		List<int> cardIds = node.processCardStack.getActiveCardIds();
		RawProcessObject pickedProcess = getAvailableProcess(cardIds, node.id);
		return pickedProcess;
	}

	private TypeAdjustingData handleProcessAdjustingCardIds(List<Card> activeCards, RawProcessObject pickedProcess)
	{
		TypeAdjustingData data = new TypeAdjustingData();
		data.init();

		List<Card> processRemovingCard = node.processCardStack.getCards(pickedProcess.removingIds.ToList());
		data.removingCards.AddRange(processRemovingCard);

		if (pickedProcess.requiredGold > 0)
		{
			TypeAdjustingData goldData = CardHelpers.handleTypeAdjusting(activeCards, CardsTypes.Gold, pickedProcess.requiredGold);
			data.addingCardIds.AddRange(goldData.addingCardIds);
			data.removingCards.AddRange(goldData.removingCards);
		}

		if (pickedProcess.requiredElectricity > 0)
		{
			TypeAdjustingData elcData = CardHelpers.handleTypeAdjusting(
				activeCards,
				CardsTypes.Electricity,
				pickedProcess.requiredElectricity
			);
			data.addingCardIds.AddRange(elcData.addingCardIds);
			data.removingCards.AddRange(elcData.removingCards);
		}

		if (pickedProcess.requiredWill > 0)
		{
			TypeAdjustingData valueData = CardHelpers.handleTypeAdjusting(activeCards, CardsTypes.Will, pickedProcess.requiredWill);
			data.addingCardIds.AddRange(valueData.addingCardIds);
			data.removingCards.AddRange(valueData.removingCards);
		}
		return data;
	}

	private IEnumerator sellCard(Card card)
	{
		isProccessing = true;
		if (card == null)
		{
			yield break;
		}

		int goldAmount = getGoldAmount(card.id);
		List<int> addingGoldCardIds = CardHelpers.generateTypeValueCards(CardsTypes.Gold, goldAmount);

		card.disableInteractiveForATime(node.staticVariables.sellTimer, CardDisableType.Process);

		yield return new WaitForSeconds(node.staticVariables.sellTimer);

		List<Card> removingCards = new List<Card> { card };

		node.hadleRemovingCards(removingCards);

		List<Card> addingCards = node.handleCreatingCards(addingGoldCardIds);

		node.consolidateTypeCards();

		isProccessing = false;
	}

	private int getGoldAmount(int cardId)
	{
		return CardDictionary.globalCardDictionary[cardId].sellingPrice;
	}

	private int getElectricityToRemoveFromProcessTime(int processTime, int currentElectricity, bool isCombo)
	{
		if (currentElectricity > 0)
		{
			if (timeToElectricity(processTime - node.staticVariables.bufferProcessingTime) <= currentElectricity)
			{
				return timeToElectricity(processTime - node.staticVariables.bufferProcessingTime);
			}

			return currentElectricity;
		}
		return 0;
	}

	private int electricityToTime(int electricityValue)
	{
		return electricityValue * node.staticVariables.electricityTimeMinus;
	}

	private int timeToElectricity(int timeSeconds)
	{
		return timeSeconds / node.staticVariables.electricityTimeMinus;
	}
}
