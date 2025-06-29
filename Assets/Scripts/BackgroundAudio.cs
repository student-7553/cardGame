using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AudioClipData
{
	public AudioClip clip;
	public int repeatCount;
}

struct SavedIndex
{
	public bool isActive;
	public int index;
}

public class BackgroundAudio : MonoBehaviour
{
	private AudioSource audioSource;
	public StaticVariables staticVariables;
	public List<AudioClipData> backgroundClips;
	private float nextPlayTimer;
	private bool isPlayingTrack;
	private int currentRepeatLimit;
	private int currentRepeatIndex;

	private SavedIndex lastIndex;
	private SavedIndex lastSecondIndex;

	void Start()
	{
		audioSource = GetComponent<AudioSource>();
		nextPlayTimer = Random.Range(staticVariables.backgroundMusicPlayIntervals.x, staticVariables.backgroundMusicPlayIntervals.y);
		currentRepeatLimit = 0;
		currentRepeatIndex = 0;
		isPlayingTrack = false;
	}

	private void FixedUpdate()
	{
		if (isPlayingTrack)
		{
			if (!audioSource.isPlaying)
			{
				currentRepeatIndex++;
				if (currentRepeatIndex < currentRepeatLimit)
				{
					audioSource.Play();
				}
				else
				{
					isPlayingTrack = false;
				}
			}
			return;

		}
		nextPlayTimer = nextPlayTimer - Time.fixedDeltaTime;
		if (nextPlayTimer <= 0)
		{
			playBackgroundAudio();
			isPlayingTrack = true;
			nextPlayTimer = Random.Range(staticVariables.backgroundMusicPlayIntervals.x, staticVariables.backgroundMusicPlayIntervals.y);
		}
	}

	private void playBackgroundAudio()
	{
		audioSource.Stop();

		AudioClipData randomAudioClipData = getRandomClip();
		audioSource.clip = randomAudioClipData.clip;
		currentRepeatLimit = randomAudioClipData.repeatCount;
		currentRepeatIndex = 0;

		audioSource.Play();
	}


	private AudioClipData getRandomClip()
	{
		int maxIndex = backgroundClips.Count;
		if (lastIndex.isActive)
		{
			maxIndex = maxIndex - 1;
		}

		if (lastSecondIndex.isActive)
		{
			maxIndex = maxIndex - 1;
		}

		int pickedIndex = Random.Range(0, maxIndex);
		int trueTargetIndex = 0;

		for (int clipIndex = 0; clipIndex < backgroundClips.Count; clipIndex++)
		{
			if (lastIndex.isActive && lastIndex.index == clipIndex)
			{
				continue;
			}
			if (lastSecondIndex.isActive && lastSecondIndex.index == clipIndex)
			{
				continue;
			}
			if (trueTargetIndex == pickedIndex)
			{
				lastSecondIndex = lastIndex;
				lastIndex = new SavedIndex { index = clipIndex, isActive = true };

				return backgroundClips[clipIndex];
			}
			trueTargetIndex++;
		}

		//Failsafe
		return backgroundClips[pickedIndex];
	}
}
