using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

public class SpriteHightlightToggler : MonoBehaviour
{
	public SO_Highlight soHightlight;
	private SpriteRenderer spriteObject;
	private Color baseColor;

	public bool isOverriden = false;

	private void Awake()
	{
		spriteObject = GetComponent<SpriteRenderer>();
		Assert.IsNotNull(soHightlight);
		Assert.IsNotNull(spriteObject);
		baseColor = spriteObject.color;
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
			spriteObject.color = baseColor - new Color(0, 0, 0, 0.75f);
			return;
		}
		// unless this is overridden by a higher function I think
		spriteObject.color = baseColor;
	}
}
