using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CardImageEntry
{
	public List<int> cardIds;
	public Sprite sprite;
}

[CreateAssetMenu(fileName = "SO_CardImage", menuName = "ScriptableObjects/SO_CardImage")]
public class SO_CardImage : ScriptableObject
{
	public List<CardImageEntry> cardImageeEntries;
}
