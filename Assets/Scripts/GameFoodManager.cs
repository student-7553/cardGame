using System;

public class GameFoodManager
{
	public int food;

	public bool isEnabled;

	public void addFood(int foodValue)
	{
		if (foodValue <= 0)
		{
			return;
		}
		food = food + foodValue;
	}

	public void decreaseFood(int foodValue)
	{
		if (!isEnabled)
		{
			return;
		}
		food = Math.Max(0, food - foodValue);
	}
}
