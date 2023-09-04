// using System.Collections;
// using System.Collections.Generic;
using Helpers;
using UnityEditor.Rendering;
using UnityEngine;

public class EnemySpawer : MonoBehaviour
{
	public enum EnemySpawner_Tier
	{
		tier_1,
		tier_2,
	}

	private EnemySpawner_Tier current_tier;
	public GameObject boardPlaneGameObject;
	private Vector2 boardSize;

	public EnemySpawnerScriptableObject enemySpawnerScriptableObject;
	public bool isEnabled;

	private readonly int spawnCardId = 21000;

	private float minSecTillSpawn;
	private float maxSecTillSpawn;

	public StaticVariables staticVariables;

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

	public void Run(EnemySpawner_Tier tier)
	{
		current_tier = tier;
		switch (tier)
		{
			case EnemySpawner_Tier.tier_1:
				minSecTillSpawn = staticVariables.enemySpawnIntervals[0].x;
				maxSecTillSpawn = staticVariables.enemySpawnIntervals[0].y;
				break;
			case EnemySpawner_Tier.tier_2:
				minSecTillSpawn = staticVariables.enemySpawnIntervals[1].x;
				maxSecTillSpawn = staticVariables.enemySpawnIntervals[1].y;
				break;
		}

		// staticVariables.enemySpawnIntervals[1].x,
		// staticVariables.enemySpawnIntervals[1].y,

		isEnabled = true;
		enemySpawnerScriptableObject.timer = GetSpawnIntervel();
	}

	public void StopRun()
	{
		isEnabled = false;
	}

	private float GetSpawnIntervel()
	{
		return Random.Range(this.minSecTillSpawn, this.maxSecTillSpawn);
	}

	private void FixedUpdate()
	{
		if (!isEnabled)
		{
			return;
		}

		this.enemySpawnerScriptableObject.timer = this.enemySpawnerScriptableObject.timer - Time.fixedDeltaTime;
		// Debug.Log("[EnemySpawn] " + this.enemySpawnerScriptableObject.timer);
		if (enemySpawnerScriptableObject.timer <= 0)
		{
			SpawnTrigger();
			this.enemySpawnerScriptableObject.timer = this.GetSpawnIntervel();
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
