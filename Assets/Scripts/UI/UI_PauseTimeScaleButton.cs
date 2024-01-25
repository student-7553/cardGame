using UnityEngine;

public class UI_PauseTimeScaleButton : MonoBehaviour
{
	public void buttonClick()
	{
		GameManager.current.handleGamePauseAction();
	}
}
