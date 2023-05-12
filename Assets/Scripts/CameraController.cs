using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	private Camera mainCamera;
	private Vector2 currentAcceleration;

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

		float movementX = Mathf.Clamp(this.currentAcceleration.x * speed, -4f, 4f);
		float movementY = Mathf.Clamp(this.currentAcceleration.y * speed, -4f, 4f);

		Vector3 newCameraPosition = mainCamera.gameObject.transform.position + new Vector3(movementX, movementY);
		mainCamera.gameObject.transform.position = newCameraPosition;
	}

	public void moveAcceleration(Vector2 movementVector)
	{
		this.currentAcceleration = movementVector;
	}
}
