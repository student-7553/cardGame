using UnityEditor.Rendering;
using UnityEngine;

public class UI_IdeaSingleBarContainer : MonoBehaviour
{
	public GameObject textContainer;
	private int cardId;

	public void buttonCallBack()
	{
		Debug.Log("UI_IdeaSingleBarContainer clicked");
	}

	public void setCardId(int cardId)
	{
		this.cardId = cardId;
	}
}
