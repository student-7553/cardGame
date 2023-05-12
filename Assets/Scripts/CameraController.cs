using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Camera mainCamera;
	private Vector2 currentAcceleration;
	public float maxAcceleration;

	[System.Serializable]
	public struct CornerPoints
	{
		public float up;
		public float down;
		public float left;
		public float right;
	}

	public CornerPoints cornerPoints;

	public float speed;

	void Start()
	{
		mainCamera = Camera.main;
		mainCamera.enabled = true;
	}

	private void Update()
	{
		if (this.currentAcceleration == Vector2.zero)
		{
			return;
		}

		float movementX = Mathf.Clamp(this.currentAcceleration.x * speed, -this.maxAcceleration, this.maxAcceleration);
		float movementY = Mathf.Clamp(this.currentAcceleration.y * speed, -this.maxAcceleration, this.maxAcceleration);

		Vector3 newCameraPosition = mainCamera.gameObject.transform.position + new Vector3(movementX, movementY);

		newCameraPosition.x = Mathf.Clamp(newCameraPosition.x, this.cornerPoints.left, this.cornerPoints.right);
		newCameraPosition.y = Mathf.Clamp(newCameraPosition.y, this.cornerPoints.down, this.cornerPoints.up);

		mainCamera.gameObject.transform.position = newCameraPosition;
	}

	public void moveAcceleration(Vector2 movementVector)
	{
		this.currentAcceleration = movementVector;
	}
}
