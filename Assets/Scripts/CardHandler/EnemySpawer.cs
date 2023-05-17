using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawer : MonoBehaviour
{
	public GameObject boardPlaneGameObject;
	private Vector2 boardSize;

	private bool isActive = false;
	private int spawnCardId = 21000;

	private float intervalPerSpawn;
	private float timer;

	private int edgeSpawnPadding = 35;

	void Start()
	{
		BoxCollider2D boxCollider = boardPlaneGameObject.GetComponent(typeof(BoxCollider2D)) as BoxCollider2D;
		if (boxCollider != null)
		{
			boardSize = boxCollider.size;
		}
		else
		{
			Debug.LogError("BOX COLLIDER MSSING [EnemySpawer]");
		}
	}

	public void init(float _intervalPerSpawn)
	{
		intervalPerSpawn = _intervalPerSpawn;
		isActive = true;
	}

	private void FixedUpdate()
	{
		if (!isActive)
		{
			return;
		}

		timer = timer + Time.fixedDeltaTime;
		if (timer > intervalPerSpawn)
		{
			timer = 0;
			spawnTrigger();
		}
	}

	void spawnTrigger()
	{
		EnemyNode createdEnemyNode = CardHandler.current.createEnemyNode(spawnCardId);
		createdEnemyNode.gameObject.transform.position = this.getEnemyNodeSpawnPoint();
	}

	private Vector3 getEnemyNodeSpawnPoint()
	{
		Vector3 spawnPosition = new Vector3(0, 0, 0);
		float widthMinus = Random.Range(0f, edgeSpawnPadding);
		if (Random.Range(-1f, 1f) > 0)
		{
			spawnPosition.x = (boardSize.x / 2) - widthMinus;
		}
		else
		{
			spawnPosition.x = -(boardSize.x / 2) + widthMinus;
		}

		float heightMinus = Random.Range(0f, edgeSpawnPadding);
		if (Random.Range(-1f, 1f) > 0)
		{
			spawnPosition.y = (boardSize.y / 2) - heightMinus;
		}
		else
		{
			spawnPosition.y = -(boardSize.y / 2) + heightMinus;
		}
		return spawnPosition;
	}
}
