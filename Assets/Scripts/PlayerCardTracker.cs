using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[DefaultExecutionOrder(-100)]
public class PlayerCardTracker : MonoBehaviour
{
	public static PlayerCardTracker current;
	private List<int> aquiredCardsInLifetimeList;

	private List<int> aquiredOneTimeProcessRewardsList;

	void Start()
	{
		if (current != null)
		{
			Destroy(gameObject);
			return;
		}
		current = this;
		initlizeVariable();
	}

	void initlizeVariable()
	{
		// Todo: Read from save file
		aquiredCardsInLifetimeList = new List<int>();
		aquiredOneTimeProcessRewardsList = new List<int>();
	}

	public void ensureCardIdTracked(int cardId)
	{
		bool isAlreadyTracked = aquiredCardsInLifetimeList.Any(libraryCardId => libraryCardId == cardId);
		if (!isAlreadyTracked)
		{
			aquiredCardsInLifetimeList.Add(cardId);
		}
	}

	public void ensureOneTimeProcessTracked(int uniqueId)
	{
		bool isAlreadyTracked = aquiredOneTimeProcessRewardsList.Any(id => id == uniqueId);
		if (!isAlreadyTracked)
		{
			aquiredOneTimeProcessRewardsList.Add(uniqueId);
		}
	}

	public bool didPlayerUnlockCard(int cardId)
	{
		return aquiredCardsInLifetimeList.Any(libraryCardId => libraryCardId == cardId);
	}

	public bool didPlayerUnlockCards(int[] cardIds)
	{
		bool isUnlocked = true;
		if (cardIds.Length > 0)
		{
			foreach (int cardId in cardIds)
			{
				if (!this.didPlayerUnlockCard(cardId))
				{
					isUnlocked = false;
					break;
				}
			}
		}
		return isUnlocked;
	}

	public bool didPlayerUnlockOneTimeProcess(int uniqueId)
	{
		return aquiredOneTimeProcessRewardsList.Any(libraryId => libraryId == uniqueId);
	}

	private T ensureComponent<T>(GameObject gameObject) where T : Component
	{
		var cardSpriteRenderer = gameObject.GetComponent(typeof(T)) as T;
		if (cardSpriteRenderer == null)
		{
			cardSpriteRenderer = gameObject.AddComponent<T>();
		}
		;
		return cardSpriteRenderer;
	}
}
