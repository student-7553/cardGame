using UnityEngine;
using TMPro;
using System;

public class UI_TimeScaleText : MonoBehaviour
{
	public SO_PlayerRuntime playerRuntime;
	private TextMeshProUGUI textMeshProUGUI;

	void Start()
	{
		textMeshProUGUI = GetComponent<TextMeshProUGUI>();
	}

	void FixedUpdate()
	{
		textMeshProUGUI.SetText($"{Math.Round(playerRuntime.gameTimeScale, 1)}");
	}
}
