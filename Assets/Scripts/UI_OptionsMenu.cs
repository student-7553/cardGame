using UnityEngine;

public class UI_OptionsMenu : MonoBehaviour
{
	public SO_PlayerRuntime playerRuntime;

	[SerializeField]
	private GameObject mainObject;

	[SerializeField]
	private GameObject gameOverObject;

	[SerializeField]
	private GameObject gameFinishedObject;

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
			return;
		}

		if (playerRuntime.isGameFailed && !gameOverObject.activeSelf)
		{
			gameOverObject.SetActive(true);
			return;
		}

		if (playerRuntime.isGameFinished && !gameFinishedObject.activeSelf)
		{
			gameFinishedObject.SetActive(true);
			return;
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
