using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

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
}
