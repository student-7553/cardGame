using UnityEngine;
using Core;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    // ---------------------- REFERENCES ------------------------------
    public GameObject cardHandlerGameObject;
    public GameObject nodePlane;

    static GameManager current;

    private GameObject[] cards;
    private GameObject[] nodes;

    private CardHandler cardHandler;

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
        if (cardHandlerGameObject != null)
        {
            cardHandler = cardHandlerGameObject.GetComponent(typeof(CardHandler)) as CardHandler;
        }

        CardDictionary.init();

    }

    private void awakeGameLogic()
    {
        foreach (GameObject singleCard in cards)
        {
            // CardStaticData cardData = singleCard.GetComponent(typeof(CardStaticData)) as CardStaticData;
            // if (cardData != null)
            // {
            //     Card cardObject = singleCard.AddComponent<Card>();
            //     cardObject.id = cardData.cardId;
            //     singleCard.AddComponent<CoreInteractable>();
            //     // initlize that card
            // }

            Card cardObject = singleCard.AddComponent<Card>();
            singleCard.AddComponent<CoreInteractable>();

        }

        int index = 1;
        foreach (GameObject singleNode in nodes)
        {
            Node createdNode = singleNode.AddComponent<Node>();
            createdNode.title = $"Sentry {index}";
            index++;
            singleNode.AddComponent<CoreInteractable>();
            createdNode.rootNodePlane = nodePlane;

            createdNode.init();

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

    }

    private void gameSettings()
    {
        Application.targetFrameRate = 60;
    }

}


