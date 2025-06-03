using UnityEngine;
using TMPro;

public class UI_TopRightObjective : MonoBehaviour
{
	public SO_Highlight so_Highlight;
	public TextMeshProUGUI textMeshHighlight;

	void FixedUpdate()
	{
		textMeshHighlight.text = so_Highlight.objectiveText;
	}
}
