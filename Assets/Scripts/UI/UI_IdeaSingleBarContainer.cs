using TMPro;
using UnityEngine;

public class UI_IdeaSingleBarContainer : MonoBehaviour
{
	public GameObject textContainer;
	public TextMeshProUGUI textObject;
	private int cardId;

	public void buttonCallBack()
	{
		Debug.Log("UI_IdeaSingleBarContainer clicked");
	}

	public void Start()
	{
		initTextObject();
	}

	private void initTextObject()
	{
		if (textObject != null)
		{
			return;
		}
		textObject = textContainer.GetComponent<TextMeshProUGUI>();
	}

	public void setCardId(int cardId)
	{
		initTextObject();
		this.cardId = cardId;

		textObject.text = $"{this.cardId}";
	}
}
