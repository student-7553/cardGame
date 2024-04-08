using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Highlight", menuName = "ScriptableObjects/SO_Highlight")]
public class SO_Highlight : ScriptableObject
{
	public bool isHighlightEnabled = false;
	public int ideaId;
	public bool topLeftHighlighted = false;
	public int[] cardIds;

	public List<Action> triggerAction = new List<Action>();

	public void triggerRefresh()
	{
		foreach (Action singleTriggerAction in triggerAction)
		{
			singleTriggerAction.Invoke();
		}
	}

	public string highlightText;

	public string highlightMainText;
	public string objectiveText;

	void OnEnable()
	{
		isHighlightEnabled = false;
		topLeftHighlighted = false;
		ideaId = -1;
		cardIds = Array.Empty<int>();
		highlightText = null;
		highlightMainText = null;
		objectiveText = null;
	}
}
