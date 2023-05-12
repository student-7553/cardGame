using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
	public InputActionReference cameraMovement;
	public InputActionReference zoomInAndOut;

	public CameraController cameraController;

	private void OnEnable()
	{
		cameraMovement.action.performed += OnCameraMovement;
		cameraMovement.action.canceled += OnCameraMovementCancel;

		zoomInAndOut.action.performed += zoomHandler;
	}

	private void OnDisable()
	{
		cameraMovement.action.performed -= OnCameraMovement;
		cameraMovement.action.canceled -= OnCameraMovementCancel;

		zoomInAndOut.action.performed -= zoomHandler;
	}

	public void zoomHandler(InputAction.CallbackContext context)
	{
		float zoomvalue = context.ReadValue<float>();
		Debug.Log(context.ReadValue<float>());
		this.cameraController.setZoom(zoomvalue);
	}

	public void OnCameraMovement(InputAction.CallbackContext context)
	{
		Vector2 tempo = context.ReadValue<Vector2>();
		cameraController.moveAcceleration(tempo);
	}

	public void OnCameraMovementCancel(InputAction.CallbackContext context)
	{
		cameraController.moveAcceleration(Vector2.zero);
	}
}
