using System.Collections.Generic;
using UnityEngine;

public class Audio_Handler : MonoBehaviour
{
	private AudioSource audioSource;
	public List<AudioClip> audioClips;
	public SO_Audio audioGlobal;
	public SFX_types audioType;

	void Start()
	{
		audioSource = GetComponent<AudioSource>();
		audioGlobal.registerToAction(audioType, triggerSound);
	}

	void OnDestroy()
	{
		audioGlobal.unRegisterToAction(audioType);
	}

	public void triggerSound()
	{
		if (audioClips.Count == 0)
		{
			return;
		}

		audioSource.Stop();

		int pickedIndex = Random.Range(0, audioClips.Count);
		audioSource.clip = audioClips[pickedIndex];

		audioSource.Play();
	}
}
