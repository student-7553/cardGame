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

	public void addCards(List<Card> cards)
	{
		queCards.AddRange(cards);
		foreach (Card card in cards)
		{
			StartCoroutine(singleCardQue(card));
		}
	}

	private IEnumerator singleCardQue(Card card)
	{
		CardObject cardObject = CardDictionary.globalCardDictionary[card.id];
		card.isDisabled = true;
		yield return new WaitForSeconds(cardObject.nodeTransferTimeCost);
		card.isDisabled = false;
	}
}
