using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Highlight", menuName = "ScriptableObjects/SO_Highlight")]
public class SO_Highlight : ScriptableObject
{
	public bool isHighlightEnabled = false;
	public int ideaId;
	public bool topLeftHighlighted = false;
	public bool highlightDisabledForce = false;

	public int[] cardIds;

	public string highlightText;
	public string highlightMainText;

	public bool isFoodFlashing;
	public bool isMovementTutorialDone;

	public string objectiveText;
	public bool bottomBarFoodHightlighted;

	public List<Action> triggerAction = new List<Action>();

	public void triggerRefresh()
	{
		if (highlightDisabledForce)
		{
			isHighlightEnabled = false;
			return;
		}

		foreach (Action singleTriggerAction in triggerAction)
		{
			singleTriggerAction.Invoke();
		}
	}

	void OnEnable()
	{
		isHighlightEnabled = false;
		topLeftHighlighted = false;
		bottomBarFoodHightlighted = false;
		isFoodFlashing = false;
		isMovementTutorialDone = false;
		ideaId = -1;
		cardIds = Array.Empty<int>();
		highlightText = null;
		highlightMainText = null;
		objectiveText = null;
	}
}
