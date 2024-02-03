using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ZoomInButton : EventTrigger
{
	public CameraController cameraController;

	private void OnEnable()
	{
		init();
	}

	private void init()
	{
		// We could make this better but we won't :p
		cameraController = (CameraController)FindObjectOfType(typeof(CameraController));
	}

	public override void OnPointerDown(PointerEventData data)
	{
		if (cameraController == null)
		{
			return;
		}
		cameraController.zoomAcceleration(1);
	}

	public override void OnPointerUp(PointerEventData data)
	{
		if (cameraController == null)
		{
			return;
		}
		cameraController.zoomAcceleration(0);
	}
}
