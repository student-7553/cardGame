using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class PlayerInput : MonoBehaviour
{
	public InputActionReference cameraMovement;

	public InputActionReference zoomAxis;
	public InputActionReference zoomIncreaseClick;
	public InputActionReference zoomDecreaseClick;

	public InputActionReference pauseButton;
	public InputActionReference leftMouseClickButton;
	public InputActionReference leftMousePressButton;

	public InputActionReference increaseTimeScale;
	public InputActionReference decreaseTimeScale;

	public CameraController cameraController;

	private Vector3 cachedMousePosition;

	private void OnEnable()
	{
		cameraMovement.action.performed += OnCameraMovement;
		cameraMovement.action.canceled += OnCameraMovementCancel;

		zoomAxis.action.performed += zoomAxisHoldHandler;
		zoomAxis.action.canceled += zoomAxisHoldCancelledHandler;
		zoomIncreaseClick.action.performed += zoomIncreaseClickHandler;
		zoomDecreaseClick.action.performed += zoomDecreaseClickHandler;

		pauseButton.action.performed += pauseButtonHandler;
		leftMouseClickButton.action.performed += leftMouseButtonHandler;
		leftMouseClickButton.action.canceled += leftMouseButtonCanceled;
		leftMousePressButton.action.performed += leftMousePress;

		increaseTimeScale.action.performed += handleIncreaseTimeScaleButtonPress;
		decreaseTimeScale.action.performed += handleDecreaseTimeScaleButtonPress;
	}

	private void OnDisable()
	{
		cameraMovement.action.performed -= OnCameraMovement;
		cameraMovement.action.canceled -= OnCameraMovementCancel;

		zoomAxis.action.performed -= zoomAxisHoldHandler;
		zoomAxis.action.canceled -= zoomAxisHoldCancelledHandler;
		zoomIncreaseClick.action.performed -= zoomIncreaseClickHandler;
		zoomDecreaseClick.action.performed -= zoomDecreaseClickHandler;

		pauseButton.action.performed -= pauseButtonHandler;
		leftMouseClickButton.action.performed -= leftMouseButtonHandler;
		leftMouseClickButton.action.canceled -= leftMouseButtonCanceled;

		leftMousePressButton.action.performed -= leftMousePress;

		increaseTimeScale.action.performed -= handleIncreaseTimeScaleButtonPress;
		decreaseTimeScale.action.performed -= handleDecreaseTimeScaleButtonPress;
	}

	public void handleIncreaseTimeScaleButtonPress(InputAction.CallbackContext context)
	{
		GameManager.current.handleGameTimeScaleIncease();
	}

	public void handleDecreaseTimeScaleButtonPress(InputAction.CallbackContext context)
	{
		GameManager.current.handleGameTimeScaleDecrease();
	}

	public void leftMousePress(InputAction.CallbackContext context)
	{
		cachedMousePosition = Mouse.current.position.ReadValue();
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
			LeftClickHandler.current.handleClickHold(cachedMousePosition);
			return;
		}

		LeftClickHandler.current.handleClick();
	}

	public void pauseButtonHandler(InputAction.CallbackContext context)
	{
		GameManager.current.handleGamePauseAction();
	}

	public void zoomAxisHoldHandler(InputAction.CallbackContext context)
	{
		float zoomvalue = context.ReadValue<float>();

		float adjustedZoomValue = zoomvalue >= 0 ? 1 : -1;
		if (context.interaction is HoldInteraction)
		{
			cameraController.zoomAcceleration(adjustedZoomValue);
			return;
		}
		cameraController.adjustZoom(adjustedZoomValue);
	}

	public void zoomAxisHoldCancelledHandler(InputAction.CallbackContext context)
	{
		cameraController.zoomAcceleration(0);
	}

	public void zoomIncreaseClickHandler(InputAction.CallbackContext context)
	{
		cameraController.adjustZoom(1);
	}

	public void zoomDecreaseClickHandler(InputAction.CallbackContext context)
	{
		cameraController.adjustZoom(-1);
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
