using UnityEngine;

public class UI_FastTimeScaleButton : MonoBehaviour
{
	public void buttonClick()
	{
		GameManager.current.handleFastTime();
	}
}
