using UnityEngine;
using System;

public enum SFX_types
{
	groundClick,
	cardClick,
	cardHold,
	cardDrop
}

[CreateAssetMenu(fileName = "SO_Audio", menuName = "ScriptableObjects/SO_Audio")]
public class SO_Audio : ScriptableObject
{
	public Action groundClickAudioAction;
	public Action cardClickAudioAction;
	public Action cardHoldAudioAction;
	public Action cardDropAudioAction;

	public void registerToAction(SFX_types type, Action action)
	{
		switch (type)
		{
			case SFX_types.groundClick:
				groundClickAudioAction = action;
				break;
			case SFX_types.cardClick:
				cardClickAudioAction = action;
				break;
			case SFX_types.cardHold:
				cardHoldAudioAction = action;
				break;
			case SFX_types.cardDrop:
				cardDropAudioAction = action;
				break;
		}
	}

	public void unRegisterToAction(SFX_types type)
	{
		switch (type)
		{
			case SFX_types.groundClick:
				groundClickAudioAction = null;
				break;
			case SFX_types.cardClick:
				cardClickAudioAction = null;
				break;
			case SFX_types.cardHold:
				cardHoldAudioAction = null;
				break;
			case SFX_types.cardDrop:
				cardDropAudioAction = null;
				break;
		}
	}
}
