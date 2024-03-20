using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Highlight", menuName = "ScriptableObjects/SO_Highlight")]
public class SO_Highlight : ScriptableObject
{
	public bool isHighlightEnabled = false;

	public string ideaId;

	public string cardId;

	public string highlightText;

	void OnDisable()
	{
		isHighlightEnabled = false;
		ideaId = null;
		cardId = null;
		highlightText = null;
	}

	void OnEnable()
	{
		isHighlightEnabled = false;
		ideaId = null;
		cardId = null;
		highlightText = null;
	}
}
