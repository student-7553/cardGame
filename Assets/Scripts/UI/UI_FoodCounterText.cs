using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_FoodCounterText : MonoBehaviour
{
	private TextMeshProUGUI textMeshProUGUI;
	private Image iconImage;
	private Text labelText;

	public SO_Highlight soHighlight;
	private Color originalTextColor;
	private Color originalLabelColor;
	private Color originalIconColor;

	public float flashSpeed = 0.5f;
	public float flashIntensity = 0.2f;
	private bool isFoodFlashing = false;

	void Start()
	{
		textMeshProUGUI = GetComponent<TextMeshProUGUI>();
		iconImage = GetComponentInChildren<Image>();
		labelText = GetComponentInChildren<Text>();
		
		originalTextColor = textMeshProUGUI.color;
		originalLabelColor = labelText.color;
		originalIconColor = iconImage.color;
		
		soHighlight.triggerAction.Add(OnHighlightChanged);
	}

	void OnDestroy()
	{
		soHighlight.triggerAction.Remove(OnHighlightChanged);
	}

	void OnHighlightChanged()
	{
		isFoodFlashing = soHighlight.isFoodFlashing;
		if (!isFoodFlashing)
		{
			textMeshProUGUI.color = originalTextColor;
			labelText.color = originalLabelColor;
			iconImage.color = originalIconColor;
		}
	}

	void FixedUpdate()
	{
		int currentFood = GameManager.current.gameFoodManager.food;
		textMeshProUGUI.SetText($"{currentFood}");

		if (isFoodFlashing)
		{
			float flashValue = Mathf.PingPong(Time.time * flashSpeed, flashIntensity);
			Color flashColor = new Color(1f, flashValue, flashValue, 1f);
			
			textMeshProUGUI.color = flashColor;
			labelText.color = flashColor;
			iconImage.color = flashColor;
		}
	}
}
