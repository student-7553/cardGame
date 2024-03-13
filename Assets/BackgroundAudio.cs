using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct SavedIndex
{
	public bool isActive;
	public int index;
}

public class BackgroundAudio : MonoBehaviour
{
	private AudioSource audioSource;
	public StaticVariables staticVariables;
	public List<AudioClip> backgroundClips;
	private float nextPlayTimer;

	private SavedIndex lastIndex;
	private SavedIndex lastSecondIndex;

	void Start()
	{
		audioSource = GetComponent<AudioSource>();
		nextPlayTimer = Random.Range(staticVariables.backgroundMusicPlayIntervals.x, staticVariables.backgroundMusicPlayIntervals.y);
	}

	private void FixedUpdate()
	{
		nextPlayTimer = nextPlayTimer - Time.fixedDeltaTime;

		if (nextPlayTimer <= 0)
		{
			playBackgroundAudio();
			nextPlayTimer = Random.Range(staticVariables.backgroundMusicPlayIntervals.x, staticVariables.backgroundMusicPlayIntervals.y);
		}
	}

	private void playBackgroundAudio()
	{
		audioSource.Stop();

		AudioClip randomAudioClip = getRandomClip();

		audioSource.clip = randomAudioClip;

		audioSource.Play();
	}

	private AudioClip getRandomClip()
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
