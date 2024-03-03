using System;

public class GameFoodManager
{
	public int food;

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
		food = Math.Max(0, food - foodValue);
	}
}
