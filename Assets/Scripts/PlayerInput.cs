using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
	public InputActionReference cameraMovement;
	public InputActionReference zoomInAndOut;
	public InputActionReference pauseButton;

	public CameraController cameraController;

	private void OnEnable()
	{
		cameraMovement.action.performed += OnCameraMovement;
		cameraMovement.action.canceled += OnCameraMovementCancel;

		zoomInAndOut.action.performed += zoomHandler;
		pauseButton.action.performed += pauseButtonHandler;
	}

	private void OnDisable()
	{
		cameraMovement.action.performed -= OnCameraMovement;
		cameraMovement.action.canceled -= OnCameraMovementCancel;

		zoomInAndOut.action.performed -= zoomHandler;
		pauseButton.action.performed -= pauseButtonHandler;
	}

	public void pauseButtonHandler(InputAction.CallbackContext context)
	{
		GameManager.current.handleGamePauseAction();
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
