using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class CardCollapsedPlaneHandler : MonoBehaviour, IClickable
{
	private CardCollapsed cardCollapsed;

	public void init(CardCollapsed cardCollapsed)
	{
		this.cardCollapsed = cardCollapsed;
	}

	public CardCollapsed getCardCollapsed()
	{
		return cardCollapsed;
	}

	public void OnClick()
	{
		gameObject.SetActive(false);
	}
}
