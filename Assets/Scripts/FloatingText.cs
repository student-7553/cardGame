using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class FloatingText : MonoBehaviour
{
	public StaticVariables staticVariables;

	private readonly float positonY = 15;

	private TextMeshPro textMesh;

	public void Run(string floatingText)
	{
		textMesh = GetComponent<TextMeshPro>();
		textMesh.text = floatingText;

		Sequence textSeq = DOTween.Sequence();
		textSeq.Append(
			gameObject.transform
				.DOMoveY(gameObject.transform.position.y + positonY, staticVariables.floatingTextDurationSec)
				.SetEase(Ease.Linear)
		);
		textSeq.Join(
			textMesh
				.DOColor(new Color(1f, 1f, 1f, 0f), staticVariables.floatingTextDurationSec)
				.SetDelay(staticVariables.floatingTextDurationSec * 0.4f)
				.SetEase(Ease.OutExpo)
		);

		textSeq.OnComplete(() =>
		{
			Destroy(gameObject);
		});
	}

}
