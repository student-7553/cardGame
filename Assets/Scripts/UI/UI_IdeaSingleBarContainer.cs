using TMPro;
using UnityEngine;

public class UI_IdeaSingleBarContainer : MonoBehaviour
{
	public TextMeshProUGUI textObject;
	public SO_PlayerRuntime playerRuntime;
	private int cardId;

	public void buttonCallBack()
	{
		Debug.Log("UI_IdeaSingleBarContainer clicked");
		Debug.Log(playerRuntime.playerFocusingCardId);
	}

	public void setCardId(int cardId)
	{
		this.cardId = cardId;

		textObject.text = $"{this.cardId}";
	}
}
