using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SO_PlayerRuntime", menuName = "ScriptableObjects/SO_PlayerRuntime")]
public class SO_PlayerRuntime : ScriptableObject
{
	public float gameTimeScale;
	private int playerFocusingCardId;

	private bool isOptionMenuEnabled = false;
	public bool isGameFailed = false;
	public bool isGameFinished = false;
	private bool isMuted = false;

	private List<Action> playerFocusAction = new List<Action>();

	public void registerActionToPlayerFocus(Action newAction)
	{
		playerFocusAction.Clear();
		playerFocusAction.Add(newAction);
	}

	public void unRegisterAction(Action targetAction)
	{
		playerFocusAction.Remove(targetAction);
	}

	public void changePlayerFocusingCardId(int playerFocusingCardId)
	{
		this.playerFocusingCardId = playerFocusingCardId;
		foreach (Action singleAction in playerFocusAction)
		{
			singleAction.Invoke();
		}
	}

	public int getPlayerFocusingCardId()
	{
		return playerFocusingCardId;
	}

	public void toggleOptionMenu()
	{
		isOptionMenuEnabled = !isOptionMenuEnabled;
		if (isOptionMenuEnabled)
		{
			gameTimeScale = 0f;
		}
		else
		{
			gameTimeScale = 1f;
		}
	}

	public bool getIsOptionMenuEnabled()
	{
		return isOptionMenuEnabled;
	}

	public void toggleIsMuted()
	{
		isMuted = !isMuted;
	}

	public bool getIsMuted()
	{
		return isMuted;
	}

	void OnEnable()
	{
		isOptionMenuEnabled = false;
		isGameFailed = false;
		isGameFinished = false;
		isMuted = false;
	}
}
