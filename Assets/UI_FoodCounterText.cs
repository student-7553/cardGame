using UnityEngine;
using TMPro;

public class UI_FoodCounterText : MonoBehaviour
{
	private TextMeshProUGUI textMeshProUGUI;

	void Start()
	{
		textMeshProUGUI = GetComponent<TextMeshProUGUI>();
	}

	void FixedUpdate()
	{
		int currentFood = GameManager.current.gameFoodManager.food;
		textMeshProUGUI.SetText($"{currentFood}");
	}
}
