using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
	public float maxAcceleration;
	public float maxZoom;
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
		handleCameraMovement();
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
		if (currentZoomAcc == 0)
		{
			return;
		}
		float newZoomvalue = currentZoom + (currentZoomAcc * staticVariables.zoomAccelerationMultiplier);
		currentZoom = Mathf.Clamp(newZoomvalue, initialZoom - maxZoom, initialZoom + maxZoom);
		mainCamera.orthographicSize = currentZoom;
	}

	private void handleCameraMovement()
	{
		Vector2 cameraMovement = new Vector2(
			Mathf.Clamp(currentAcceleration.x * speed, -maxAcceleration, maxAcceleration),
			Mathf.Clamp(currentAcceleration.y * speed, -maxAcceleration, maxAcceleration)
		);

		Vector2 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
		Vector2 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));

		CornerPoints newCameraCornerPoints = new CornerPoints
		{
			down = bottomLeft.y + cameraMovement.y,
			up = topRight.y + cameraMovement.y,
			left = bottomLeft.x + cameraMovement.x,
			right = topRight.x + cameraMovement.x
		};

		if (newCameraCornerPoints.up > staticVariables.cornerPoints.up)
		{
			cameraMovement = cameraMovement + new Vector2(0, staticVariables.cornerPoints.up - newCameraCornerPoints.up);
		}
		if (newCameraCornerPoints.down < staticVariables.cornerPoints.down)
		{
			cameraMovement = cameraMovement + new Vector2(0, staticVariables.cornerPoints.down - newCameraCornerPoints.down);
		}
		if (newCameraCornerPoints.left < staticVariables.cornerPoints.left)
		{
			cameraMovement = cameraMovement + new Vector2(staticVariables.cornerPoints.left - newCameraCornerPoints.left, 0);
		}
		if (newCameraCornerPoints.right > staticVariables.cornerPoints.right)
		{
			cameraMovement = cameraMovement + new Vector2(staticVariables.cornerPoints.right - newCameraCornerPoints.right, 0);
		}

		// if (currentAcceleration == Vector2.zero)
		// {
		// 	return;
		// }

		Vector3 newCameraPosition = mainCamera.gameObject.transform.position + new Vector3(cameraMovement.x, cameraMovement.y);
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
