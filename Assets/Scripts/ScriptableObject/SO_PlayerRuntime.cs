using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SO_PlayerRuntime", menuName = "ScriptableObjects/SO_PlayerRuntime")]
public class SO_PlayerRuntime : ScriptableObject
{
	public float gameTimeScale;

	public int playerFocusingCardId;
	private List<Action> playerFocusAction = new List<Action>();

	public void registerActionToPlayerFocus(Action newAction)
	{
		playerFocusAction.Add(newAction);
	}
}
