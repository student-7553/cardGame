using TMPro;

public class UI_TopLeftEntry_2 : TopLeftEntry
{
	public TextMeshProUGUI textField;
	public TextMeshProUGUI textField2;
	public TextMeshProUGUI textField3;

	public override void Show(int currentCardId)
	{
		textField.text = $"{CardDictionary.globalCardDictionary[currentCardId].resourceInventoryCount}";

		textField2.text = $"{CardDictionary.globalCardDictionary[currentCardId].infraInventoryCount}";

		if (CardDictionary.globalCardDictionary[currentCardId].isSellable)
		{
			textField3.text = $"{CardDictionary.globalCardDictionary[currentCardId].sellingPrice}";
		}
		else
		{
			textField3.text = "";
		}

		gameObject?.SetActive(true);
	}

	public override void Hide()
	{
		gameObject?.SetActive(false);
	}
}
