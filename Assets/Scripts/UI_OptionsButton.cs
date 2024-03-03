using UnityEngine;

public class UI_OptionsButton : MonoBehaviour
{
	public SO_PlayerRuntime playerRuntime;

	public void cLicked()
	{
		playerRuntime.toggleOptionMenu();
	}
}
