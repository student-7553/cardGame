using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_FoodInjector : MonoBehaviour
{
	public int injectFoodValue;

	void Start()
	{
		GameManager.current.gameFoodManager.addFood(injectFoodValue);
	}
}
