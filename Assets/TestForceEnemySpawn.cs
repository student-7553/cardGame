using UnityEngine;

public class TestForceEnemySpawn : MonoBehaviour
{
	public EnemySpawer enemySpawner;
	public Vector2Int enemySpawnInterval;

	void Start()
	{
		this.enemySpawner.start(enemySpawnInterval.x, enemySpawnInterval.y);
	}
}
