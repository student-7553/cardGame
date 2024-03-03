using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_QuitButton : MonoBehaviour
{
	public void Clicked()
	{
		SceneManager.LoadScene(0, LoadSceneMode.Single);
	}
}
