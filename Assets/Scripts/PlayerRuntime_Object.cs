using UnityEngine;

[CreateAssetMenu(fileName = "PlayerRuntime_Object", menuName = "ScriptableObjects/PlayerRuntime_Object")]
public class PlayerRuntime_Object : ScriptableObject
{
	public float timeScale;

	void OnDisable()
	{
		timeScale = 1f;
	}

	void OnEnable()
	{
		timeScale = 1f;
	}
}
