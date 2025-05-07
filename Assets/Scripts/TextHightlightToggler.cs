using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class TextHightlightToggler : MonoBehaviour
{
	public SO_Highlight soHightlight;
	private TextMeshPro textObject;
	private Color baseColor;

	public bool isOverriden = false;

	private void Awake()
	{
		textObject = GetComponent<TextMeshPro>();
		Assert.IsNotNull(soHightlight);
		Assert.IsNotNull(textObject);
		baseColor = textObject.color;
		soHightlight.triggerAction.Add(triggerDimRefresh);
		triggerDimRefresh();
	}

	private void OnDestroy()
	{
		soHightlight.triggerAction.Remove(triggerDimRefresh);
	}

	public void triggerDimRefresh()
	{
		if (soHightlight.isHighlightEnabled && !isOverriden)
		{
			textObject.color = baseColor - new Color(0, 0, 0, 0.75f);
			return;
		}
		textObject.color = baseColor;
	}
}
