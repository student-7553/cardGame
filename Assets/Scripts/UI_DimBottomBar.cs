using UnityEngine;

public class UI_DimBottomBar : MonoBehaviour
{
	public SO_Highlight soHighlight;
	public GameObject dimObject;

	private void Awake()
	{
		soHighlight.triggerAction.Add(triggerDimRefresh);
	}

	private void OnDestroy()
	{
		soHighlight.triggerAction.Remove(triggerDimRefresh);
	}

	private void triggerDimRefresh()
	{
		if (soHighlight.isHighlightEnabled)
		{
			dimObject.SetActive(true);
			return;
		}
		dimObject.SetActive(false);
	}
}
