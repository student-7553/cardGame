using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_HighlightText : MonoBehaviour
{
	public SO_Highlight soHightlight;
	public TextMeshProUGUI textMeshUnderline;
	public TextMeshProUGUI textMeshHighlight;

	void FixedUpdate()
	{
		textMeshUnderline.text = soHightlight.highlightText;
		textMeshHighlight.text = soHightlight.highlightMainText;
	}
}
