using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_QuitButton : MonoBehaviour
{
	public void Clicked()
	{
		SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
	}
}
