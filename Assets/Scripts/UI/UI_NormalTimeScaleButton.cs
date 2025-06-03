using UnityEngine;

public class UI_NormalTimeScaleButton : MonoBehaviour
{
	//Todo
	public void buttonClick()
	{
		GameManager.current.handleNormalTime();
	}
}
