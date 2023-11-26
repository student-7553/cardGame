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
			if (textMeshProUGUI.enabled)
			{
				textMeshProUGUI.enabled = false;
			}
			return;
		}

		if (!textMeshProUGUI.enabled)
		{
			textMeshProUGUI.enabled = false;
		}

		textMeshProUGUI.SetText($"{(int)enemySpawnerScriptableObject.timer}");
	}
}
