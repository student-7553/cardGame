using UnityEngine;
using UnityEngine.UI;

public class UI_PauseTimeScaleButton : MonoBehaviour
{
	[SerializeField] private Text buttonText;
	[SerializeField] private Image buttonIcon;
	[SerializeField] private Sprite pauseIcon;
	[SerializeField] private Sprite playIcon;
	

	private void Start()
	{
		// If components are not assigned in inspector, try to find them
		if (buttonText == null)
			buttonText = GetComponentInChildren<Text>();
		if (buttonIcon == null)
			buttonIcon = GetComponentInChildren<Image>();
			
		UpdateButtonState();
	}

	public void buttonClick()
	{
		GameManager.current.handleGamePauseAction();
		UpdateButtonState();
	}

	private void UpdateButtonState()
	{
		bool isPaused = GameManager.current.playerRuntime.gameTimeScale == 0;

		if (buttonText != null)
			buttonText.text = isPaused ? "Play" : "Pause";
			
		if (buttonIcon != null && pauseIcon != null && playIcon != null)
			buttonIcon.sprite = isPaused ? playIcon : pauseIcon;
	}
}
