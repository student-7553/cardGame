using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StaticVariables", menuName = "ScriptableObjects/StaticVariables", order = 1)]
public class StaticVariables : ScriptableObject
{
	public List<int> foodCardIds;

	public int comboTimeFlatMinus = 5;

	public int comboTimeElectricityMinus = 2;

	public int electricityTimeMinus = 10;

	public float processCooldown = 2f;

	public float sellTimer = 1f;

	public int bufferProcessingTime = 1;

	public int processingTimeMin = 1;

	public float floatingTextDurationSec = 1f;

	public float magnetizedIntervel = 5f;

	public Vector2 defaultNodePlanePositon = new Vector2(-75, 0);

	public int endingCardId;

	public List<Vector2Int> enemySpawnIntervals;

	public float magnetizeMaxRange = 10;

	public float cardReachSmoothTime = 0.15f;
}
