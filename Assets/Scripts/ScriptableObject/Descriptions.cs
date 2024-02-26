using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DescriptionEntry
{
	public List<int> cardIds;
	public string description;
}

[CreateAssetMenu(fileName = "Descriptions", menuName = "ScriptableObjects/Descriptions")]
public class Descriptions : ScriptableObject
{
	public List<DescriptionEntry> foodCardIds;
}
