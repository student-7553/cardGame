using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using Core;
using System.Linq;
using Helpers;
using Unity.VisualScripting;

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
		TypeAdjustingData data = CardHelpers.handleTypeAdjusting(node.processCardStack.getActiveBaseCards(), cardType, typeValue);

		if (timer > 0)
		{
			foreach (Card card in data.removingCards)
			{
				card.disableInteractiveForATime(timer, CardDisableType.Process);
			}
			yield return new WaitForSeconds(timer);
		}

		// node.hadleRemovingCards(data.removingCards);
		foreach (Card singleRemovingCard in data.removingCards)
		{
			if (singleRemovingCard != null)
			{
				singleRemovingCard.destroyCard();
			}
		}

		List<Card> addingCards = CardHandler.current.handleCreatingCards(data.addingCardIds);

		List<BaseCard> addingBaseCards = new List<BaseCard>(addingCards);

		node.processCardStack.addCardsToStack(addingBaseCards);

		if (addingCards != null && addingCards.Count > 0)
		{
			List<BaseCard> ejectingBaseCards = addingBaseCards
				.Where(
					(card) =>
					{
						return CardHelpers.isNonValueTypeCard(CardDictionary.globalCardDictionary[card.id].type);
					}
				)
				.ToList();
			if (ejectingBaseCards.Count > 0)
			{
				node.ejectCards(ejectingBaseCards);
			}
		}

		if (callback != null)
		{
			callback.Invoke();
		}
	}

	private void handleMarketProcess()
	{
		BaseCard sellingBaseCard = node.processCardStack.cards.Find(
			(card) => CardDictionary.globalCardDictionary[card.id].type != CardsTypes.Gold
		);

		if (sellingBaseCard == null)
		{
			return;
		}
		StartCoroutine(sellCard(sellingBaseCard));
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
		RawProcessObject pickedProcess = getNextAvailableProcess();

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

	private List<RawProcessObject> getAvailableProcesses(List<int> cardIds, int nodeId)
	{
		List<RawProcessObject> possibleProcesses = new List<RawProcessObject>();

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
					possibleProcesses.Add(singleProcess);
					break;
				}
			}
		}
		return possibleProcesses;
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

		List<BaseCard> activeBaseCards = node.processCardStack.getActiveBaseCards();

		TypeAdjustingData adjData = handleProcessAdjustingCardIds(activeBaseCards, pickedProcess);

		addingCardIds.AddRange(adjData.addingCardIds);

		List<Card> removingCards = adjData.removingCards;

		List<int> restNonInteractiveCardIds = getRestNonInteractiveCardIds();

		List<BaseCard> restNonInteractiveCards = node.processCardStack.getBaseCardsFromIds(restNonInteractiveCardIds);

		foreach (BaseCard card in restNonInteractiveCards)
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

		// Fillers start
		if (pickedAddingCardObject.updateCurrentNode != 0 && node.id < pickedAddingCardObject.updateCurrentNode)
		{
			node.id = pickedAddingCardObject.updateCurrentNode;
		}

		GameManager.current.gameFoodManager.addFood(pickedAddingCardObject.addingFood);

		if (pickedAddingCardObject.id == 5176)
		{
			List<int> cardIds = node.processCardStack.getAllCardIds();
			int foodInfraId = 1001;
			int infraCount = cardIds.Where((cardId) => cardId == foodInfraId).Count();
			int totalUnityValue = getUnityValue();

			int plusFoodCount = infraCount;

			if (totalUnityValue > 0)
			{
				plusFoodCount = plusFoodCount * 2;
			}
			GameManager.current.gameFoodManager.addFood(plusFoodCount - pickedAddingCardObject.addingFood);
		}

		// Fillers end

		List<BaseCard> ejectingBaseCards = new List<BaseCard>();
		if (node.isActive)
		{
			foreach (Card singleRemovingCard in removingCards)
			{
				if (singleRemovingCard == null)
				{
					continue;
				}
				singleRemovingCard?.destroyCard();
			}

			List<BaseCard> addingBaseCards = new List<BaseCard>(CardHandler.current.handleCreatingCards(addingCardIds));

			if (pickedProcess.id == 34 || pickedProcess.id == 35 || pickedProcess.id == 28)
			{
				ejectingBaseCards.AddRange(addingBaseCards);
			}
			else
			{
				ejectingBaseCards.AddRange(
					addingBaseCards.Where(
						(card) =>
						{
							return CardHelpers.isNonValueTypeCard(CardDictionary.globalCardDictionary[card.id].type)
								|| CardDictionary.globalCardDictionary[card.id].type == CardsTypes.Electricity;
						}
					)
				);
			}

			addingBaseCards = addingBaseCards
				.Where(
					(baseCard) =>
					{
						return !ejectingBaseCards.Contains(baseCard);
					}
				)
				.ToList();

			node.processCardStack.addCardsToStack(addingBaseCards);
		}
		else
		{
			// Todo: handle failed process
			foreach (BaseCard card in removingCards)
			{
				if (card == null)
				{
					continue;
				}
				card.isInteractiveDisabled = false;
			}
		}

		foreach (BaseCard card in restNonInteractiveCards)
		{
			if (card == null)
			{
				continue;
			}
			card.isInteractiveDisabled = false;
		}

		foreach (int newCardId in addingCardsFromProcess)
		{
			GameManager.current.SpawnFloatingText("[" + CardDictionary.globalCardDictionary[newCardId].name + "]", transform.position);
		}

		ejectingBaseCards.AddRange(
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

		node.ejectCards(ejectingBaseCards);

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

			// CardCollapsed card will be uninetactive
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

	private int getUnityValue()
	{
		List<int> unityModuleCount = node.processCardStack.getAllCardIdsOfUnityModules();
		int totalUnityValue = unityModuleCount.Aggregate(
			0,
			(total, cardId) =>
			{
				return total + CardDictionary.globalCardDictionary[cardId].module.unityCount;
			}
		);
		return totalUnityValue;
	}

	private int getProcessTime(RawProcessObject pickedProcess, bool isCombo)
	{
		int processTime = pickedProcess.time;

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
		List<int> cardIds = node.processCardStack.getAllActiveCardIds();

		int totalUnityValue = getUnityValue();

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
		List<int> cardIds = node.processCardStack.getAllActiveCardIds();

		List<RawProcessObject> possibleProcess = getAvailableProcesses(cardIds, node.id);
		if (possibleProcess.Count == 0)
		{
			return null;
		}
		IEnumerable<RawProcessObject> processEnumerable = possibleProcess.OrderByDescending((process) => process.priority);
		return processEnumerable.First();
	}

	private TypeAdjustingData handleProcessAdjustingCardIds(List<BaseCard> baseCards, RawProcessObject pickedProcess)
	{
		TypeAdjustingData data = new TypeAdjustingData();
		data.init();

		List<Card> processRemovingCard = node.processCardStack.getCardsFromIds(pickedProcess.removingIds.ToList());

		data.removingCards.AddRange(processRemovingCard);

		if (pickedProcess.requiredGold > 0)
		{
			TypeAdjustingData goldData = CardHelpers.handleTypeAdjusting(baseCards, CardsTypes.Gold, pickedProcess.requiredGold);
			data.addingCardIds.AddRange(goldData.addingCardIds);
			data.removingCards.AddRange(goldData.removingCards);
		}

		if (pickedProcess.requiredElectricity > 0)
		{
			TypeAdjustingData elcData = CardHelpers.handleTypeAdjusting(
				baseCards,
				CardsTypes.Electricity,
				pickedProcess.requiredElectricity
			);
			data.addingCardIds.AddRange(elcData.addingCardIds);
			data.removingCards.AddRange(elcData.removingCards);
		}

		if (pickedProcess.requiredWill > 0)
		{
			TypeAdjustingData valueData = CardHelpers.handleTypeAdjusting(baseCards, CardsTypes.Will, pickedProcess.requiredWill);
			data.addingCardIds.AddRange(valueData.addingCardIds);
			data.removingCards.AddRange(valueData.removingCards);
		}
		return data;
	}

	private IEnumerator sellCard(BaseCard card)
	{
		isProccessing = true;
		if (card == null)
		{
			yield break;
		}

		int goldAmount = 0;
		if (card.interactableType == CoreInteractableType.CollapsedCards)
		{
			List<int> cardIds = card.getCollapsedCard().getCards().Select((card) => card.id).ToList();
			foreach (int cardId in cardIds)
			{
				goldAmount = goldAmount + getGoldAmount(cardId);
			}
		}
		else
		{
			// Card
			goldAmount = getGoldAmount(card.id);
		}

		List<int> addingGoldCardIds = CardHelpers.generateTypeValueCards(CardsTypes.Gold, goldAmount);

		card.disableInteractiveForATime(node.staticVariables.sellTimer, CardDisableType.Process);

		yield return new WaitForSeconds(node.staticVariables.sellTimer);

		List<BaseCard> removingCards = new List<BaseCard> { card };

		foreach (BaseCard singleRemovingCard in removingCards)
		{
			singleRemovingCard.destroyCard();
		}

		List<BaseCard> addingCards = new List<BaseCard>(CardHandler.current.handleCreatingCards(addingGoldCardIds));
		node.processCardStack.addCardsToStack(addingCards);
		// node.processCardStack.consolidateTypeCards();

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
