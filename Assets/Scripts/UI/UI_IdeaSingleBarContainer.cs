using TMPro;
using UnityEngine;
using System.Collections;

public class UI_IdeaSingleBarContainer : MonoBehaviour
{
	public TextMeshProUGUI textObject;
	public GameObject dimGameObject;
	public SO_PlayerRuntime playerRuntime;
	public SO_Highlight so_Highlight;

	[SerializeField]
	public int cardId;

	public void buttonCallBack()
	{
		playerRuntime.changePlayerFocusingCardId(cardId);
		if (so_Highlight.isHighlightEnabled && !dimGameObject.activeSelf && cardId == 2001)
		{
			so_Highlight.isHighlightEnabled = true;
			so_Highlight.cardIds = new int[] { };
			so_Highlight.ideaId = -1;
			so_Highlight.topLeftHighlighted = true;
			so_Highlight.highlightText =
				"You can see detailed information about the card on the left side. Specifically the cards required to make this card";
			so_Highlight.triggerRefresh();
			StartCoroutine(stopDim());
		}
	}

	public IEnumerator stopDim()
	{
		yield return new WaitForSeconds(4);
		so_Highlight.isHighlightEnabled = false;
		so_Highlight.cardIds = new int[] { };
		so_Highlight.ideaId = -1;
		so_Highlight.topLeftHighlighted = false;
		so_Highlight.highlightText = "";
		so_Highlight.triggerRefresh();
	}

	public void handleDim()
	{
		dimGameObject.SetActive(true);
	}

	public void handleNonDim()
	{
		dimGameObject.SetActive(false);
	}

	public void setCardId(int cardId)
	{
		this.cardId = cardId;

		textObject.text = $"{CardDictionary.globalCardDictionary[this.cardId].name}";
	}
}
