using Helpers;
using UnityEngine;

public class EnemySpawer : MonoBehaviour
{
	public enum EnemySpawner_Tier
	{
		tier_1,
		tier_2,
	}

	public GameObject boardPlaneGameObject;
	private Vector2 boardSize;

	public EnemySpawnerScriptableObject enemySpawnerScriptableObject;
	public bool isEnabled;

	private readonly int spawnCardId = 21000;

	private float minSecTillSpawn;
	private float maxSecTillSpawn;

	public StaticVariables staticVariables;

	public PlayerRuntime_Object playerRuntime;

	private readonly int edgeSpawnPadding = 10;

	private float internalTimer = 0f;

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

		isEnabled = true;
		enemySpawnerScriptableObject.timer = GetSpawnIntervel();
	}

	public void StopRun()
	{
		isEnabled = false;
	}

	private float GetSpawnIntervel()
	{
		return UnityEngine.Random.Range(minSecTillSpawn, maxSecTillSpawn);
	}

	private void FixedUpdate()
	{
		internalTimer = internalTimer + Time.fixedDeltaTime;
		if (internalTimer >= 1f)
		{
			handleSecondTick();
			internalTimer = 0;
		}
	}

	private void handleSecondTick()
	{
		if (!isEnabled)
		{
			return;
		}

		enemySpawnerScriptableObject.timer = enemySpawnerScriptableObject.timer - playerRuntime.timeScale;

		if (enemySpawnerScriptableObject.timer <= 0)
		{
			SpawnTrigger();
			enemySpawnerScriptableObject.timer = GetSpawnIntervel();
		}
	}

	private void SpawnTrigger()
	{
		EnemyNode createdEnemyNode = CardHandler.current.createEnemyNode(spawnCardId);
		createdEnemyNode.gameObject.transform.position = getEnemyNodeSpawnPoint();
	}

	private Vector3 getEnemyNodeSpawnPoint()
	{
		Vector3 spawnPosition = new Vector3(0, 0, HelperData.enemyNodeBaseZ);
		if (UnityEngine.Random.Range(-1f, 1f) > 0)
		{
			spawnPosition.y = UnityEngine.Random.Range(-(boardSize.y / 2) + edgeSpawnPadding, boardSize.y / 2 - edgeSpawnPadding);
			float widthMinus = UnityEngine.Random.Range(0f, edgeSpawnPadding);
			if (UnityEngine.Random.Range(-1f, 1f) > 0)
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
			spawnPosition.x = UnityEngine.Random.Range(-(boardSize.x / 2) + edgeSpawnPadding, (boardSize.x / 2) - edgeSpawnPadding);
			float heightMinus = UnityEngine.Random.Range(0f, edgeSpawnPadding);
			if (UnityEngine.Random.Range(-1f, 1f) > 0)
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
