using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_StartButton : MonoBehaviour
{
	public void Clicked()
	{
		SceneManager.LoadScene("main", LoadSceneMode.Single);
	}
}
