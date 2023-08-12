// using System.Collections;
// using System.Collections.Generic;
using Helpers;
using UnityEngine;

public class EnemySpawer : MonoBehaviour
{
	public GameObject boardPlaneGameObject;
	private Vector2 boardSize;

	private bool isActive = false;
	private readonly int spawnCardId = 21000;

	private float minSecTillSpawn;
	private float maxSecTillSpawn;

	private float timer;

	private readonly int edgeSpawnPadding = 10;

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

	public void Run(float _minSecTillSpawn, float _maxSecTillSpawn)
	{
		minSecTillSpawn = _minSecTillSpawn;
		maxSecTillSpawn = _maxSecTillSpawn;
		isActive = true;
		timer = GetSpawnIntervel();
	}

	public void StopRun()
	{
		isActive = false;
	}

	private float GetSpawnIntervel()
	{
		return Random.Range(this.minSecTillSpawn, this.maxSecTillSpawn);
	}

	private void FixedUpdate()
	{
		if (!isActive)
		{
			return;
		}

		this.timer = this.timer - Time.fixedDeltaTime;
		// Debug.Log("[EnemySpawn] " + this.timer);
		if (timer <= 0)
		{
			SpawnTrigger();
			this.timer = this.GetSpawnIntervel();
		}
	}

	private void SpawnTrigger()
	{
		EnemyNode createdEnemyNode = CardHandler.current.createEnemyNode(spawnCardId);
		createdEnemyNode.gameObject.transform.position = this.getEnemyNodeSpawnPoint();
	}

	private Vector3 getEnemyNodeSpawnPoint()
	{
		Vector3 spawnPosition = new Vector3(0, 0, HelperData.enemyNodeBaseZ);
		if (Random.Range(-1f, 1f) > 0)
		{
			spawnPosition.y = Random.Range(-(boardSize.y / 2) + edgeSpawnPadding, boardSize.y / 2 - edgeSpawnPadding);
			float widthMinus = Random.Range(0f, edgeSpawnPadding);
			if (Random.Range(-1f, 1f) > 0)
			{
				spawnPosition.x = (boardSize.x / 2) - widthMinus;
			}
			else
			{
				spawnPosition.x = -(boardSize.x / 2) + widthMinus;
			}
		}
		else
		{
			spawnPosition.x = Random.Range(-(boardSize.x / 2) + edgeSpawnPadding, (boardSize.x / 2) - edgeSpawnPadding);
			float heightMinus = Random.Range(0f, edgeSpawnPadding);
			if (Random.Range(-1f, 1f) > 0)
			{
				spawnPosition.y = (boardSize.y / 2) - heightMinus;
			}
			else
			{
				spawnPosition.y = -(boardSize.y / 2) + heightMinus;
			}
		}

		return spawnPosition;
	}
}
