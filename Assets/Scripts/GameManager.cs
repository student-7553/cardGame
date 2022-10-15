using UnityEngine;
using Core;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{

    static GameManager current;

    public GameObject nodePlane;

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
        foreach (GameObject singleCard in cards)
        {
            singleCard.AddComponent<Card>();
            singleCard.AddComponent<Interactable>();
        }

        int index = 1;
        foreach (GameObject singleNode in nodes)
        {
            Node createdNode = singleNode.AddComponent<Node>();
            createdNode.title = $"Sentry {index}";
            index++;
            singleNode.AddComponent<Interactable>();
            NodePlaneManagers nodePlaneManagers = singleNode.AddComponent<NodePlaneManagers>();
            nodePlaneManagers.rootNodePlane = nodePlane;
            createdNode.init();
            nodePlaneManagers.init();

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
            nodes = GameObject.FindGameObjectsWithTag("Nodes");
        }

        // foreach (GameObject singleCardGameObject in cards)
        // {
        //     Card hitCard = singleCardGameObject.GetComponent(typeof(Card)) as Card;
        //     if (hitCard == null)
        //     {
        //         singleCardGameObject.AddComponent<Card>();
        //     }
        // }

    }

    private void gameSettings()
    {
        Application.targetFrameRate = 60;
    }

}


