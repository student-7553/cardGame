using UnityEngine;
using Core;

[DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    // ---------------------- REFERENCES ------------------------------
    public GameObject cardHandlerGameObject;
    public GameObject nodePlane;

    // public Sprite[] cardSprites;
    // public Sprite[] nodeSprites;

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

        CardDictionary.init();


        if (cardHandlerGameObject != null)
        {
            cardHandler = cardHandlerGameObject.GetComponent(typeof(CardHandler)) as CardHandler;
        }

        DontDestroyOnLoad(gameObject);
        gameSettings();
        findTempLogic();
        awakeGameLogic();



    }

    private void awakeGameLogic()
    {
        foreach (GameObject singleCard in cards)
        {

            CardStaticData cardData = singleCard.GetComponent(typeof(CardStaticData)) as CardStaticData;
            if (cardData != null)
            {
                cardHandler.createCard(cardData.cardId, singleCard, singleCard.transform.position);
            }

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


