using UnityEngine;

public class UI_TabButtonn : MonoBehaviour
{
	public SO_PlayerRuntime playerRuntime;

	public void clicked()
	{
		playerRuntime.toggleIdeaTab();
	}
}
