using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Core;

public class NodeCardQue : MonoBehaviour
{
	public List<Card> queCards = new List<Card>();
	private bool isProccessing = false;

	private int overrideTimeCost = 0;

	private void FixedUpdate()
	{
		if (!isProccessing && queCards.Count > 0)
		{
			StartCoroutine(singleCardQue());
		}
	}

	public void addCard(Card card)
	{
		CardObject cardObject = CardDictionary.globalCardDictionary[card.id];
		card.disableInteractiveForATime(cardObject.nodeTransferTimeCost, CardDisableType.Que);
		queCards.Add(card);
	}

	private IEnumerator singleCardQue()
	{
		Card card = queCards[0];
		isProccessing = true;
		CardObject cardObject = CardDictionary.globalCardDictionary[card.id];
		if (overrideTimeCost > 0)
		{
			yield return new WaitForSeconds(overrideTimeCost);
		}
		else
		{
			yield return new WaitForSeconds(cardObject.nodeTransferTimeCost);
		}

		card.isInteractiveDisabled = false;
		isProccessing = false;
		queCards.RemoveAt(0);
	}
}
