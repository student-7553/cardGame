using UnityEngine;
using TMPro;
using System;

public class UI_TimeScaleText : MonoBehaviour
{
	public PlayerRuntime_Object playerRuntime;
	private TextMeshProUGUI textMeshProUGUI;

	void Start()
	{
		textMeshProUGUI = GetComponent<TextMeshProUGUI>();
	}

	void FixedUpdate()
	{
		textMeshProUGUI.SetText($"{Math.Round(playerRuntime.timeScale, 1)}");
	}
}
