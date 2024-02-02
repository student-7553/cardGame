using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ZoomOutButton : EventTrigger
{
	public override void OnPointerDown(PointerEventData data)
	{
		Debug.Log("OnPointerDown called.");
	}

	public override void OnPointerUp(PointerEventData data)
	{
		Debug.Log("OnPointerUp called.");
	}
}
