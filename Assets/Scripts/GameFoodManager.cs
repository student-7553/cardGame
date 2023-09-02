using System;
using UnityEngine;

public class GameFoodManager
{
	public int food;

	public void addFood(int foodValue)
	{
		food = food + foodValue;
	}

	public void decreaseFood(int foodValue)
	{
		food = Math.Max(0, food - foodValue);
	}
}
