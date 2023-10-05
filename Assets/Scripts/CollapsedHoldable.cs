using System.Collections.Generic;
using Core;
using UnityEngine;

public class CollapsedHoldable : MonoBehaviour, IMouseHoldable
{
	public int stackCount;

	private CardCollapsedPlaneHandler cardCollapsedPlaneHandler;
	private SpriteRenderer spriteRenderer;
	private Color defaultColor;

	void Start()
	{
		cardCollapsedPlaneHandler = gameObject.transform.parent.gameObject.GetComponent<CardCollapsedPlaneHandler>();
		spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
		defaultColor = spriteRenderer.color;
	}

	public bool isDisabled()
	{
		CardCollapsed cardCollapsed = cardCollapsedPlaneHandler.getCardCollapsed();
		if (cardCollapsed == null)
		{
			spriteRenderer.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.25f);
			return true;
		}

		if (cardCollapsed.getCards().Count < stackCount)
		{
			spriteRenderer.color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, 0.25f);
			return true;
		}
		spriteRenderer.color = defaultColor;
		return false;
	}

	public Interactable[] getMouseHoldInteractables()
	{
		if (isDisabled())
		{
			return null;
		}

		CardCollapsed cardCollapsed = cardCollapsedPlaneHandler.getCardCollapsed();

		List<BaseCard> cards = new List<BaseCard>(cardCollapsed.getCards().GetRange(0, stackCount));
		cardCollapsed.removeCardsFromStack(cards);

		Interactable[] interactables = cards.ToArray();
		return interactables;
	}
}
