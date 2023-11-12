using UnityEngine;

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
