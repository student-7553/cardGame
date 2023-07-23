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

		if (!this.isAvailableForNextProcess())
		{
			return;
		}

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
		Card sellingCard = node.processCardStack.cards.Find(
			(card) => CardHelpers.isNonValueTypeCard(CardDictionary.globalCardDictionary[card.id].type)
		);

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
			this.handleActiveNodeProcess();
		}
		else
		{
			this.handleInActiveNodeProcess();
		}
	}

	private void handleActiveNodeProcess()
	{
		List<int> cardIds = node.processCardStack.getActiveCardIds();
		RawProcessObject pickedProcess = this.getAvailableProcess(cardIds, node.id);

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
			this.isProccessing = true;
			StartCoroutine(
				this.queUpTypeDeletion(
					CardsTypes.Food,
					foodNeededToBeActive,
					timer,
					() =>
					{
						this.isProccessing = false;
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

				bool isRightNode = singleProcess.mustBeNodeId != 0 ? singleProcess.mustBeNodeId == nodeId : true;
				bool ifRequiredCardsPassed = getIfRequiredCardsPassed(indexedRequiredIds, clonedCardIds);
				bool goldPassed = CardHelpers.getTypeValueFromCardIds(CardsTypes.Gold, cardIds) >= singleProcess.requiredGold;
				bool electricityPassed =
					CardHelpers.getTypeValueFromCardIds(CardsTypes.Electricity, cardIds) >= singleProcess.requiredElectricity;

				if (isUnlocked && isRightNode && ifRequiredCardsPassed && goldPassed && electricityPassed)
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

	private IEnumerator handleProcess(RawProcessObject pickedProcess, bool isCombo)
	{
		this.isProccessing = true;
		List<int> addingCardIds = new List<int>();

		AddingCardsObject pickedAddingCardObject = pickAddingCardsObject(pickedProcess);
		if (pickedAddingCardObject.isOneTime)
		{
			CardHandler.current.playerCardTracker.ensureOneTimeProcessTracked(pickedAddingCardObject.id);
		}

		addingCardIds.AddRange(this.getAddingCards(pickedAddingCardObject));

		List<Card> activeCards = node.processCardStack.getActiveCards();

		TypeAdjustingData adjData = this.handleProcessAdjustingCardIds(activeCards, pickedProcess);

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

		this.proccessingLeft = this.getProcessTime(pickedProcess, isCombo);

		// if (this.node.nodeStats.currentNodeStats.currentElectricity > 0)
		// {
		// 	int electricityRemove = this.getElectricityToRemoveFromProcessTime(
		// 		pickedProcess.time,
		// 		this.node.nodeStats.currentNodeStats.currentElectricity
		// 	);
		// 	StartCoroutine(this.queUpTypeDeletion(CardsTypes.Electricity, electricityRemove, 0, null));
		// 	this.proccessingLeft = pickedProcess.time - this.electricityToTime(electricityRemove);
		// }

		while (this.proccessingLeft > 0)
		{
			yield return new WaitForSeconds(1);
			this.proccessingLeft = this.proccessingLeft - 1;

			if (this.proccessingLeft > this.node.staticVariables.bufferProcessingTime)
			{
				// Is not in the buffer zone
				if (this.node.nodeStats.currentNodeStats.currentElectricity > 0)
				{
					// electricity got updated
					int electricityRemove = this.getElectricityToRemoveFromProcessTime(
						(int)this.proccessingLeft,
						this.node.nodeStats.currentNodeStats.currentElectricity,
						isCombo
					);
					StartCoroutine(this.queUpTypeDeletion(CardsTypes.Electricity, electricityRemove, 0, null));
					this.proccessingLeft = pickedProcess.time - this.electricityToTime(electricityRemove);
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

		this.isProccessing = false;

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
					this.node.staticVariables.bufferProcessingTime
				);
			}
		}

		if (isCombo)
		{
			processTime = Math.Max(processTime - this.node.staticVariables.comboTimeFlatMinus, this.node.staticVariables.processingTimeMin);
		}

		if (this.node.nodeStats.currentNodeStats.currentElectricity > this.node.staticVariables.processingTimeMin)
		{
			int electricityRemove = this.getElectricityToRemoveFromProcessTime(
				pickedProcess.time,
				this.node.nodeStats.currentNodeStats.currentElectricity,
				false
			);
			StartCoroutine(this.queUpTypeDeletion(CardsTypes.Electricity, electricityRemove, 0, null));
			processTime = pickedProcess.time - this.electricityToTime(electricityRemove);
		}

		return processTime;
	}

	private List<int> getAddingCards(AddingCardsObject pickedAddingCardObject)
	{
		List<int> addingCardIds = pickedAddingCardObject.addingCardIds.ToList();
		List<int> cardIds = node.processCardStack.getActiveCardIds();

		List<int> unityModuleCount = node.processCardStack.getAllCardIdsOfUnityModules();
		int totalUnityValue = unityModuleCount.Aggregate(0, (total, next) => total + next);

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
		// this.isProccessing = false;
		RawProcessObject nextProcess = this.getNextAvailableProcess();
		if (nextProcess != null)
		{
			// Combo start
			this.isOnCooldown = false;
			StartCoroutine(handleProcess(nextProcess, true));
			yield break;
		}

		this.isOnCooldown = true;
		yield return new WaitForSeconds(this.node.staticVariables.processCooldown);
		this.isOnCooldown = false;
	}

	private RawProcessObject getNextAvailableProcess()
	{
		List<int> cardIds = node.processCardStack.getActiveCardIds();
		RawProcessObject pickedProcess = this.getAvailableProcess(cardIds, node.id);
		return pickedProcess;
	}

	private TypeAdjustingData handleProcessAdjustingCardIds(List<Card> activeCards, RawProcessObject pickedProcess)
	{
		TypeAdjustingData data = new TypeAdjustingData();
		data.init();

		List<Card> processRemovingCard = this.node.processCardStack.getCards(pickedProcess.removingIds.ToList());
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
		return data;
	}

	private IEnumerator sellCard(Card card)
	{
		this.isProccessing = true;
		if (card == null)
		{
			yield break;
		}

		int goldAmount = this.getGoldAmount(card.id);
		List<int> addingGoldCardIds = CardHelpers.generateTypeValueCards(CardsTypes.Gold, goldAmount);

		card.disableInteractiveForATime(this.node.staticVariables.sellTimer, CardDisableType.Process);

		yield return new WaitForSeconds(this.node.staticVariables.sellTimer);

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
			if (this.timeToElectricity(processTime - this.node.staticVariables.bufferProcessingTime) <= currentElectricity)
			{
				return this.timeToElectricity(processTime - this.node.staticVariables.bufferProcessingTime);
			}

			return currentElectricity;
		}
		return 0;
	}

	private int electricityToTime(int electricityValue)
	{
		return electricityValue * this.node.staticVariables.electricityTimeMinus;
	}

	private int timeToElectricity(int timeSeconds)
	{
		return timeSeconds / this.node.staticVariables.electricityTimeMinus;
	}
}
