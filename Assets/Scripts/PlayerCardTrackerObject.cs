using UnityEngine;
using System.Collections.Generic;

public class PlayerCardTrackerObject
{
	public HashSet<int> aquiredCardsInLifetime = new HashSet<int>();

	private HashSet<int> aquiredOneTimeProcessRewardsList = new HashSet<int>();

	public void ensureCardIdTracked(int cardId)
	{
		if (aquiredCardsInLifetime.Contains(cardId))
		{
			return;
		}

		aquiredCardsInLifetime.Add(cardId);
	}

	public void ensureOneTimeProcessTracked(int uniqueId)
	{
		if (!aquiredOneTimeProcessRewardsList.Contains(uniqueId))
		{
			aquiredOneTimeProcessRewardsList.Add(uniqueId);
		}
	}

	public bool didPlayerUnlockCard(int cardId)
	{
		return aquiredCardsInLifetime.Contains(cardId);
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
		return aquiredOneTimeProcessRewardsList.Contains(uniqueId);
	}
}
