using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

[CreateAssetMenu(fileName = "StaticVariables", menuName = "ScriptableObjects/StaticVariables", order = 1)]
public class StaticVariables : ScriptableObject
{
	public List<int> foodCardIds;
}
