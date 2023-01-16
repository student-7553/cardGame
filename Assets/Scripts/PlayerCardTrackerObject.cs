using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "PlayerCardTrackerObject", menuName = "ScriptableObjects/SpawnManagerScriptableObject", order = 1)]
public class PlayerCardTrackerObject : ScriptableObject
{
	[SerializeField]
	public Dictionary<int, bool> aquiredCardsInLifetime = new Dictionary<int, bool>();

	[SerializeField]
	private List<int> aquiredOneTimeProcessRewardsList = new List<int>();

	public void ensureCardIdTracked(int cardId)
	{
		if (!aquiredCardsInLifetime.ContainsKey(cardId))
		{
			aquiredCardsInLifetime.Add(cardId, true);
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
		if (aquiredCardsInLifetime.ContainsKey(cardId))
		{
			return true;
		}
		return false;
	}

	public bool didPlayerUnlockCards(int[] cardIds)
	{
		bool isUnlocked = true;
		if (cardIds != null && cardIds.Length > 0)
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
