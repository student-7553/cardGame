using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_HighlightText : MonoBehaviour
{
	public SO_Highlight soHightlight;
	public TextMeshProUGUI textMeshUnderline;
	public TextMeshProUGUI textMeshHighlight;
	public Color highLightEnabledColor;
	public Color highLightDisabledColor;

	public Color highLightEnabledUnderLineColor;
	public Color highLightDisabledUnderLineColor;

	private void Awake()
	{
		soHightlight.triggerAction.Add(triggerDimRefresh);
		triggerDimRefresh();
	}

	private void OnDestroy()
	{
		soHightlight.triggerAction.Remove(triggerDimRefresh);
	}

	private void triggerDimRefresh()
	{
		if (soHightlight.isHighlightEnabled)
		{
			textMeshHighlight.color = highLightEnabledColor;
			textMeshUnderline.color = highLightEnabledUnderLineColor;
		}
		else
		{
			textMeshHighlight.color = highLightDisabledColor;
			textMeshUnderline.color = highLightDisabledUnderLineColor;
		}

		textMeshUnderline.text = soHightlight.highlightText;
		textMeshHighlight.text = soHightlight.highlightMainText;
	}
}
