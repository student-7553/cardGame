using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Highlight", menuName = "ScriptableObjects/SO_Highlight")]
public class SO_Highlight : ScriptableObject
{
	public bool isHighlightEnabled = false;

	public int ideaId;

	public int cardId;

	public string highlightText;

	void OnDisable()
	{
		isHighlightEnabled = false;
		ideaId = -1;
		cardId = -1;
		highlightText = null;
	}

	void OnEnable()
	{
		isHighlightEnabled = false;
		ideaId = -1;
		cardId = -1;
		highlightText = null;
	}
}
