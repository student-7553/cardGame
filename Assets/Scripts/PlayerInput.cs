using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerInput : MonoBehaviour
{
	public InputActionReference cameraMovement;
	public InputActionReference zoomInAndOut;
	public InputActionReference pauseButton;
	public InputActionReference leftMouseClickButton;
	public InputActionReference leftMousePressButton;

	private Vector3 cachedMousePosition;

	public CameraController cameraController;

	private void OnEnable()
	{
		cameraMovement.action.performed += OnCameraMovement;
		cameraMovement.action.canceled += OnCameraMovementCancel;

		zoomInAndOut.action.performed += zoomHandler;
		pauseButton.action.performed += pauseButtonHandler;
		leftMouseClickButton.action.performed += leftMouseButtonHandler;
		leftMouseClickButton.action.canceled += leftMouseButtonCanceled;

		leftMousePressButton.action.performed += leftMousePress;
	}

	private void OnDisable()
	{
		cameraMovement.action.performed -= OnCameraMovement;
		cameraMovement.action.canceled -= OnCameraMovementCancel;

		zoomInAndOut.action.performed -= zoomHandler;
		pauseButton.action.performed -= pauseButtonHandler;
		leftMouseClickButton.action.performed -= leftMouseButtonHandler;
		leftMouseClickButton.action.canceled -= leftMouseButtonCanceled;

		leftMousePressButton.action.performed -= leftMousePress;
	}

	public void leftMousePress(InputAction.CallbackContext context)
	{
		this.cachedMousePosition = Mouse.current.position.ReadValue();
	}

	public void leftMouseButtonCanceled(InputAction.CallbackContext context)
	{
		if (context.interaction is HoldInteraction)
		{
			LeftClickHandler.current.handleClickHoldEnd();
		}
	}

	public void leftMouseButtonHandler(InputAction.CallbackContext context)
	{
		if (LeftClickHandler.current == null)
		{
			return;
		}

		if (context.interaction is HoldInteraction)
		{
			LeftClickHandler.current.handleClickHold(this.cachedMousePosition);
		}

		LeftClickHandler.current.handleClick();
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
