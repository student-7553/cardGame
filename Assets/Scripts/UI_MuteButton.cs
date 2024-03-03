using UnityEngine;

public class UI_MuteButton : MonoBehaviour
{
	public SO_PlayerRuntime playerRuntime;

	public void cLicked()
	{
		playerRuntime.toggleIsMuted();
	}
}
