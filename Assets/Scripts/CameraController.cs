using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public struct CornerPoints
{
	public float up;
	public float down;
	public float left;
	public float right;
}

public class CameraController : MonoBehaviour
{
	public float maxAcceleration;
	public float maxZoom;
	public CornerPoints cornerPoints;
	public float speed;

	private Camera mainCamera;
	private Vector2 currentAcceleration;
	public StaticVariables staticVariables;
	public bool isMouseAccelerationLocked;

	private float currentZoomAcc;

	private float currentZoom;
	private float initialZoom;

	public void adjustZoom(float zoomValue)
	{
		float newZoomvalue = currentZoom + zoomValue;
		currentZoom = Mathf.Clamp(newZoomvalue, initialZoom - maxZoom, initialZoom + maxZoom);

		mainCamera.orthographicSize = currentZoom;
	}

	private void Start()
	{
		mainCamera = Camera.main;
		mainCamera.enabled = true;

		initialZoom = mainCamera.orthographicSize;
		currentZoom = initialZoom;
	}

	private void FixedUpdate()
	{
		handleFixedCameraAcceleration();
		handleFixedZoomAcceleration();
		handleMouseScreenEdge();
	}

	private void handleMouseScreenEdge()
	{
		if (isMouseAccelerationLocked)
		{
			return;
		}
		Vector2 mousePosition = Mouse.current.position.ReadValue();
		bool isRightScreenEdgeReached = mousePosition.x >= Screen.width * (1 - staticVariables.screenMouseEdgeThreshhold);
		bool isLeftScreenEdgeReached = mousePosition.x <= Screen.width * staticVariables.screenMouseEdgeThreshhold;
		bool isTopScreenEdgeReached = mousePosition.y >= Screen.height * (1 - staticVariables.screenMouseEdgeThreshhold);
		bool isBottomScreenEdgeReached = mousePosition.y <= Screen.height * staticVariables.screenMouseEdgeThreshhold;
		float moveCameraX = 0;
		float moveCameraY = 0;
		if (isRightScreenEdgeReached)
		{
			moveCameraX = 1;
		}
		else if (isLeftScreenEdgeReached)
		{
			moveCameraX = -1;
		}

		if (isTopScreenEdgeReached)
		{
			moveCameraY = 1;
		}
		else if (isBottomScreenEdgeReached)
		{
			moveCameraY = -1;
		}

		if (moveCameraX == 0 && moveCameraY == 0)
		{
			moveAcceleration(Vector2.zero);
			return;
		}

		moveAcceleration(new Vector2(moveCameraX, moveCameraY));
	}

	private void handleFixedZoomAcceleration()
	{
		float newZoomvalue = currentZoom + (currentZoomAcc * staticVariables.zoomAccelerationMultiplier);
		currentZoom = Mathf.Clamp(newZoomvalue, initialZoom - maxZoom, initialZoom + maxZoom);

		mainCamera.orthographicSize = currentZoom;
	}

	private void handleFixedCameraAcceleration()
	{
		if (currentAcceleration == Vector2.zero)
		{
			return;
		}

		float movementX = Mathf.Clamp(currentAcceleration.x * speed, -maxAcceleration, maxAcceleration);
		float movementY = Mathf.Clamp(currentAcceleration.y * speed, -maxAcceleration, maxAcceleration);

		Vector3 newCameraPosition = mainCamera.gameObject.transform.position + new Vector3(movementX, movementY);

		newCameraPosition.x = Mathf.Clamp(newCameraPosition.x, cornerPoints.left, cornerPoints.right);
		newCameraPosition.y = Mathf.Clamp(newCameraPosition.y, cornerPoints.down, cornerPoints.up);

		mainCamera.gameObject.transform.position = newCameraPosition;
	}

	public void moveAcceleration(Vector2 movementVector)
	{
		currentAcceleration = movementVector;
	}

	public void zoomAcceleration(float zoomAccDirection)
	{
		currentZoomAcc = zoomAccDirection;
	}
}
