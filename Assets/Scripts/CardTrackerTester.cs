using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardTrackerTester : MonoBehaviour
{
	public List<int> pushedCardIds;

	// Start is called before the first frame update
	void Start()
	{
		foreach (int cardId in pushedCardIds)
		{
			CardHandler.current.playerCardTracker.ensureCardIdTracked(cardId);
		}
	}

	// // Update is called once per frame
	// void Update() { }
}
