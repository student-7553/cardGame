using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class FloatingText : MonoBehaviour
{
	public StaticVariables staticVariables;

	// private readonly float fadeDuration = 0.5f;
	private readonly float positonY = 15;

	private TextMeshPro textMesh;

	public void Run(string floatingText)
	{
		textMesh = GetComponent<TextMeshPro>();
		textMesh.text = floatingText;
		gameObject.transform
			.DOMoveY(gameObject.transform.position.y + positonY, staticVariables.floatingTextDurationSec)
			.SetEase(Ease.Linear);

		StartCoroutine(HandleFade());
	}

	IEnumerator HandleFade()
	{
		// yield return new WaitForSeconds(staticVariables.floatingTextDurationSec - fadeDuration);
		// yield return new WaitForSeconds(fadeDuration);
		yield return new WaitForSeconds(staticVariables.floatingTextDurationSec);
		Destroy(gameObject);
	}
}
