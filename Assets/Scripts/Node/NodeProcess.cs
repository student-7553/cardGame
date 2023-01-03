using UnityEngine;
using System.Collections.Generic;
using System.Collections;
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

		this.isProccessing = true;

		if (node.isMarket())
		{
			// Market process
			this.handleMarketProcess();
			return;
		}

		// Node process
		this.handleNodeProcess();
	}

	private void handleMarketProcess()
	{
		// Market process
		List<int> activeCardsIds = node.cardStack.getNonTypeActiveCardIds();
		if (activeCardsIds.Count == 0)
		{
			this.isProccessing = false;
			return;
		}

		StartCoroutine(sellCard(node.cardStack.cards[0]));
	}

	private void handleNodeProcess()
	{
		if (!node.isActive)
		{
			this.handleInActiveNodeProcess();
			return;
		}

		List<int> cardIds = node.cardStack.getActiveCardIds();

		RawProcessObject pickedProcess = this.getAvailableProcess(cardIds);
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
				node.handleCardTypeDeletion(
					CardsTypes.Food,
					foodNeededToBeActive,
					timer,
					() =>
					{
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

	private RawProcessObject getAvailableProcess(List<int> cardIds)
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
				bool isUnlocked = PlayerCardTracker.current.didPlayerUnlockCards(singleProcess.unlockCardIds);

				bool ifRequiredCardsPassed = getIfRequiredCardsPassed(indexedRequiredIds, clonedCardIds);
				if (
					isUnlocked
					&& ifRequiredCardsPassed
					&& node.nodeStats.currentNodeStats.currentGold >= singleProcess.requiredGold
					&& node.nodeStats.currentNodeStats.currentElectricity >= singleProcess.requiredElectricity
				)
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

		List<int> cardIds = node.cardStack.getActiveCardIds();

		this.handleProcessAdjustingCardIds(cardIds, pickedProcess, ref removingCardIds, ref addingCardIds);

		List<Card> removingCards = node.getCards(removingCardIds);
		foreach (Card card in removingCards)
		{
			card.isDisabled = true;
		}

		StartCoroutine(handleProcessCounter(pickedProcess.time));
		yield return new WaitForSeconds(pickedProcess.time);

		if (node.isActive)
		{
			node.hadleRemovingCards(removingCards);
			node.handleCreatingCards(addingCardIds);
		}
		else
		{
			// Todo: handle failed process
		}

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
						bool isOneTimeUnlocked = PlayerCardTracker.current.didPlayerUnlockOneTimeProcess(addingCardObject.id);
						if (isOneTimeUnlocked)
						{
							// Player Already unlocked this oneTimeReward
							return false;
						}
					}

					bool isUnlocked = PlayerCardTracker.current.didPlayerUnlockCards(addingCardObject.extraUnlockCardIds);
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
	}

	private IEnumerator handleProcessCooldown()
	{
		this.isProccessing = false;
		yield return new WaitForSeconds(processCooldown);
		isOnCooldown = false;
	}

	private void handleProcessAdjustingCardIds(
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
			node.handleTypeRemoval(cardIds, CardsTypes.Gold, pickedProcess.requiredGold, ref removingCardIds, ref addingCardIds);
		}

		if (pickedProcess.requiredElectricity > 0)
		{
			node.handleTypeRemoval(
				cardIds,
				CardsTypes.Electricity,
				pickedProcess.requiredElectricity,
				ref removingCardIds,
				ref addingCardIds
			);
		}
		return;
	}

	private IEnumerator handleProcessCounter(float processTime)
	{
		this.proccessingLeft = processTime;
		while (this.proccessingLeft > 0.1f)
		{
			this.proccessingLeft -= 1f;
			yield return new WaitForSeconds(1);
		}
	}

	private IEnumerator sellCard(Card card)
	{
		if (card == null)
		{
			yield break;
		}

		int goldAmount = this.getGoldAmount(card.id);
		List<int> addingGoldCardIds = CardHelpers.generateTypeValueCards(CardsTypes.Gold, goldAmount);

		card.isDisabled = true;

		yield return new WaitForSeconds(sellTimer);

		List<Card> removingCards = new List<Card> { card };

		node.hadleRemovingCards(removingCards);
		node.handleCreatingCards(addingGoldCardIds);

		isProccessing = false;
	}

	private int getGoldAmount(int cardId)
	{
		return CardDictionary.globalCardDictionary[cardId].sellingPrice;
	}
}
