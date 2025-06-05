using System;
using UnityEngine.Events;

public class GameFoodManager
{
	public int food;
	public bool isEnabled;
	public UnityAction onFoodDecrease;

	public GameFoodManager(UnityAction onFoodDecrease)
	{
		this.onFoodDecrease = onFoodDecrease;
	}


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
		onFoodDecrease.Invoke();
	}
}
