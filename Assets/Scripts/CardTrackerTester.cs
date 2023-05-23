using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTrackerTester : MonoBehaviour
{
	public List<int> pushedCardIds;

	void Start()
	{
		foreach (int cardId in pushedCardIds)
		{
			CardHandler.current.playerCardTracker.ensureCardIdTracked(cardId);
		}
	}
}
