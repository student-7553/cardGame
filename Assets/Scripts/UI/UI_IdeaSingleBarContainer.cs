using TMPro;
using UnityEngine;

public class UI_IdeaSingleBarContainer : MonoBehaviour
{
	public TextMeshProUGUI textObject;
	public SO_PlayerRuntime playerRuntime;
	private int cardId;

	public void buttonCallBack()
	{
		playerRuntime.changePlayerFocusingCardId(cardId);
	}

	public void setCardId(int cardId)
	{
		this.cardId = cardId;

		textObject.text = $"{CardDictionary.globalCardDictionary[this.cardId].name}";
	}
}
