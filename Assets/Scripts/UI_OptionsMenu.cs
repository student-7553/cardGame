using UnityEngine;

public class UI_OptionsMenu : MonoBehaviour
{
	public SO_PlayerRuntime playerRuntime;

	[SerializeField]
	private GameObject mainObject;

	void FixedUpdate()
	{
		bool isOptionEnabled = playerRuntime.getIsOptionMenuEnabled();
		if (mainObject.activeSelf != isOptionEnabled)
		{
			if (isOptionEnabled)
			{
				handleEnable();
			}
			else
			{
				handleDisable();
			}
		}
	}

	private void OnEnable()
	{
		if (playerRuntime.getIsOptionMenuEnabled())
		{
			playerRuntime.toggleOptionMenu();
		}
	}

	private void handleEnable()
	{
		mainObject.SetActive(true);
	}

	private void handleDisable()
	{
		mainObject.SetActive(false);
	}
}
