using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Audio_Handler : MonoBehaviour
{
	private AudioSource audioSource;
	public List<AudioClip> audioClips;
	public SO_Audio audioGlobal;
	public SO_PlayerRuntime playerRuntime;
	public SFX_types audioType;

	void Start()
	{
		Assert.IsNotNull(playerRuntime);
		Assert.IsNotNull(audioGlobal);
		audioSource = GetComponent<AudioSource>();
		audioGlobal.registerToAction(audioType, triggerSound);

		int pickedIndex = Random.Range(0, audioClips.Count);
		audioSource.clip = audioClips[pickedIndex];
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
		if (playerRuntime.getIsMuted())
		{
			return;
		}

		audioSource.Play();
	}
}
