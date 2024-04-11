using UnityEngine;
using TMPro;

public class UI_TopRightObjective : MonoBehaviour
{
	public SO_Highlight so_Highlight;
	public TextMeshProUGUI textMeshHighlight;
	public GameObject containerDimObject;

	private void Awake()
	{
		so_Highlight.triggerAction.Add(triggerDimRefresh);
	}

	private void OnDestroy()
	{
		so_Highlight.triggerAction.Remove(triggerDimRefresh);
	}

	private void triggerDimRefresh()
	{
		if (so_Highlight.isHighlightEnabled)
		{
			containerDimObject.SetActive(true);
		}
		else
		{
			containerDimObject.SetActive(false);
		}
	}

	void FixedUpdate()
	{
		textMeshHighlight.text = so_Highlight.objectiveText;
	}
}
