using UnityEngine;

public class TestForceEnemySpawn : MonoBehaviour
{
	public EnemySpawer enemySpawner;

	public int minEnemySpawnInterval;
	public int maxEnemySpawnInterval;

	void Start()
	{
		enemySpawner.Run(EnemySpawer.EnemySpawner_Tier.tier_1);
	}
}
