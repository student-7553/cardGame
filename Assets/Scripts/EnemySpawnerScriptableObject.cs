using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

[CreateAssetMenu(fileName = "EnemySpawnerScriptableObject")]
public class EnemySpawnerScriptableObject : ScriptableObject
{
	public float timer;
	public bool isEnabled;
}
