using UnityEngine;

public class UI_NormalTimeScaleButton : MonoBehaviour
{
	public void buttonClick()
	{
		GameManager.current.handleNormalTime();
	}
}
