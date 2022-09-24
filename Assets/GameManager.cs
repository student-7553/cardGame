using UnityEngine;
using Core;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{

    static GameManager current;

    private GameObject[] cards;
    private GameObject[] nodes;

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
        findTempLogic();

        awakeGameLogic();

    }

    private void awakeGameLogic()
    {
        foreach(GameObject singleCard in cards){
            singleCard.AddComponent<Card>();
            singleCard.AddComponent<Interactable>();
        }


        foreach(GameObject singleNode in nodes){
            singleNode.AddComponent<Node>();
            singleNode.AddComponent<Interactable>();
        }
    }

    public void findTempLogic()
    {
        if (cards == null)
        {
            cards = GameObject.FindGameObjectsWithTag("Cards");
        }

        if (nodes == null)
        {
            cards = GameObject.FindGameObjectsWithTag("Nodes");
        }

    }

    private void gameSettings()
    {
        Application.targetFrameRate = 60;
    }

}


