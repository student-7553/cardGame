using UnityEngine;
// using Core;

// [DefaultExecutionOrder(-100)]
public class GameManager : MonoBehaviour
{
    // ---------------------- REFERENCES ------------------------------
    public GameObject cardHandlerGameObject;
    public GameObject nodePlane;

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

        CardDictionary.init();


        if (cardHandlerGameObject != null)
        {
            // cardHandler = cardHandlerGameObject.GetComponent(typeof(CardHandler)) as CardHandler;
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

            StaticData cardData = singleCard.GetComponent(typeof(StaticData)) as StaticData;
            if (cardData != null)
            {
                CardHandler.current.createCard(cardData.id, singleCard, singleCard.transform.position);
            }

        }

        foreach (GameObject singleNode in nodes)
        {
            StaticData nodeData = singleNode.GetComponent(typeof(StaticData)) as StaticData;
            if (nodeData != null)
            {
                CardHandler.current.createNode(nodeData.id, singleNode);
            }

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


