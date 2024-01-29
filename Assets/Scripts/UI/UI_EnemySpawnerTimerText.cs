using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_EnemySpawnerTimerText : MonoBehaviour
{
	public EnemySpawnerScriptableObject enemySpawnerScriptableObject;
	private TextMeshProUGUI textMeshProUGUI;

	void Start()
	{
		textMeshProUGUI = GetComponent<TextMeshProUGUI>();
	}

	void FixedUpdate()
	{
		if (!enemySpawnerScriptableObject.isEnabled)
		{
			textMeshProUGUI.SetText("0");
			return;
		}

		textMeshProUGUI.SetText($"{(int)enemySpawnerScriptableObject.timer}");
	}
}
