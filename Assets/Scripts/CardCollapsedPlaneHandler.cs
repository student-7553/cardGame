using Core;
using UnityEngine;

public class CardCollapsedPlaneHandler : MonoBehaviour, IClickable, IStackable
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

	public void stackOnThis(BaseCard draggingCard, Node prevNode)
	{
		cardCollapsed.stackOnThis(draggingCard, prevNode);
	}
}
