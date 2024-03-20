using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_HighlightText : MonoBehaviour
{
	public SO_Highlight soHightlight;
	private TextMeshProUGUI textMesh;

	private void Start()
	{
		textMesh = GetComponent<TextMeshProUGUI>();
	}

	void FixedUpdate()
	{
		textMesh.text = soHightlight.highlightText;
	}
}
