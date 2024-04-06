using UnityEngine;

public class UI_OptionsMenu : MonoBehaviour
{
	public SO_PlayerRuntime playerRuntime;

	[SerializeField]
	private GameObject mainObject;

	[SerializeField]
	private GameObject gameOverObject;

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

		if (playerRuntime.isGameFailed && !gameOverObject.activeSelf)
		{
			Debug.Log("GameOver is called in Options");
			gameOverObject.SetActive(true);
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
