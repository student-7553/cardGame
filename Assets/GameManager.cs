using UnityEngine;
// using System.Collections;
// using UnityEngine.SceneManagement;
// using System.Collections.Generic;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{

    static GameManager current;

    void Start()
    {
        if (current != null)
        {
            Destroy(gameObject);
            return;
        }
        current = this;
        DontDestroyOnLoad(gameObject);

        gameSettings();
        awakeGameLogic();

    }

    public static void awakeGameLogic()
    {

    }


    private void gameSettings()
    {
        Application.targetFrameRate = 60;
    }

    // void Update()
    // {

    // }




}


