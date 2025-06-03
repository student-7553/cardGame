using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System.Linq;

[CreateAssetMenu(fileName = "SO_PlayerRuntime", menuName = "ScriptableObjects/SO_PlayerRuntime")]
public class SO_PlayerRuntime : ScriptableObject
{
	public float gameTimeScale;
	private int playerFocusingCardId;
	public SO_Highlight soHighlight;

	private bool isOptionMenuEnabled = false;
	public bool isIdeaTabOpen = false;
	public bool isGameFailed = false;
	public bool isGameFinished = false;
	private bool isMuted = false;

	private List<Action> playerFocusAction = new List<Action>();

	void Awake()
	{
		Assert.IsNotNull(soHighlight);
	}

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
		if (soHighlight.isHighlightEnabled && soHighlight.cardIds.Count() == 1 && soHighlight.cardIds.Contains(2001))
		{
			isIdeaTabOpen = true;
		}

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

	public void toggleIdeaTab()
	{
		isIdeaTabOpen = !isIdeaTabOpen;
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
