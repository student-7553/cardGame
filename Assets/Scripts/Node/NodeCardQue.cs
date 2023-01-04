using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Core;

public class NodeCardQue : MonoBehaviour
{
	public List<Card> queCards;

	public void Awake()
	{
		queCards = new List<Card>();
	}

	public void addCard(Card card)
	{
		queCards.Add(card);
		StartCoroutine(singleCardQue(card));
	}

	private IEnumerator singleCardQue(Card card)
	{
		CardObject cardObject = CardDictionary.globalCardDictionary[card.id];
		card.isInteractiveDisabled = true;
		yield return new WaitForSeconds(cardObject.nodeTransferTimeCost);
		card.isInteractiveDisabled = false;
	}
}
