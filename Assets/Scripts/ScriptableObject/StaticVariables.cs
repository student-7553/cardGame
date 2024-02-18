using System.Collections.Generic;
using Core;
using UnityEngine;

[System.Serializable]
public struct CornerPoints
{
	public float up;
	public float down;
	public float left;
	public float right;
}

[System.Serializable]
public struct TypeToColorMap
{
	public CardsTypes cardType;
	public Color color;
}

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

	public int endingCardId;

	public List<Vector2Int> enemySpawnIntervals;

	public float magnetizeMaxRange = 10;

	public float cardReachSmoothTime = 0.15f;

	public float nodeEjectDistance = 10;

	public float nodeEjectSlideDistance = 6;

	public float nodeEjectSlideTime = 0.5f;

	public float zoomAccelerationMultiplier = 0.3f;

	public float screenMouseEdgeThreshhold = 0.05f;

	public CornerPoints cornerPoints;

	public List<TypeToColorMap> cardColors;

	public List<TypeToColorMap> cardTextColors;

	public List<TypeToColorMap> cardBackgroundColors;

	public Vector3 hoveringShadowAdjustment;
}
