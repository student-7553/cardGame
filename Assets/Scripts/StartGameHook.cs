using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameHook : MonoBehaviour
{
	void Start()
	{
		GameManager.current.startGame();
	}
}
