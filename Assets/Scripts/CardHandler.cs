using UnityEngine;
using Core;
[DefaultExecutionOrder(-100)]
public class CardHandler : MonoBehaviour
{

    public static CardHandler current;

    private Vector3 defaultCardPoint;
    public GameObject cardPrefab;
    public GameObject nodePrefab;
    public GameObject nodePlanePrefab;

    public Sprite[] cardSprites;
    public Sprite[] nodeSprites;

    void Start()
    {
        if (current != null)
        {
            Destroy(gameObject);
            return;
        }
        current = this;

        defaultCardPoint = new Vector3();
    }

    public Card createCard(int cardId, GameObject cardGameObject, Vector3 cardOriginPoint)
    {
        if (!CardDictionary.globalCardDictionary.ContainsKey(cardId))
        {
            return null;
        }
        cardGameObject.name = CardDictionary.globalCardDictionary[cardId].name;
        cardGameObject.tag = "Cards";
        cardGameObject.layer = 6;
        cardGameObject.transform.position = cardOriginPoint;

        Card cardObject = ensureComponent<Card>(cardGameObject);
        cardObject.id = cardId;
        cardObject.init();

        ensureComponent<CoreInteractable>(cardGameObject);

        SpriteRenderer cardSpriteRenderer = ensureComponent<SpriteRenderer>(cardGameObject);
        cardSpriteRenderer.sprite = cardSprites[Random.Range(0, cardSprites.Length)];
        return cardObject;
    }

    private T ensureComponent<T>(GameObject gameObject) where T : Component
    {
        var cardSpriteRenderer = gameObject.GetComponent(typeof(T)) as T;
        if (cardSpriteRenderer == null)
        {
            cardSpriteRenderer = gameObject.AddComponent<T>();
        };
        return cardSpriteRenderer;
    }




    public Card createCard(int cardId, Vector3 cardOriginPoint)
    {

        GameObject newNodePlane = Instantiate(cardPrefab);
        newNodePlane.transform.position = cardOriginPoint;
        newNodePlane.SetActive(true);

        return createCard(cardId, newNodePlane, cardOriginPoint);

    }
    public Card createCard(int cardId)
    {
        return createCard(cardId, defaultCardPoint);
    }

    public Node createNode(int cardId)
    {
        GameObject newNodeGameObject = Instantiate(nodePrefab);
        return this.createNode(cardId, newNodeGameObject);

    }



    public Node createNode(int nodeId, GameObject nodeGameObject)
    {
        nodeGameObject.name = CardDictionary.globalCardDictionary[nodeId].name;
        nodeGameObject.tag = "Nodes";
        nodeGameObject.layer = 6;

        Node newNode = ensureComponent<Node>(nodeGameObject);
        newNode.id = nodeId;
        // newNode.title = $"Sentry {cardId}";

        switch (nodeId)
        {
            case 3000:
                newNode.nodeState = NodeStateTypes.base_1;
                break;
            case 3001:
                newNode.nodeState = NodeStateTypes.base_2;
                break;
            case 3002:
                newNode.nodeState = NodeStateTypes.base_3;
                break;
            case 3003:
                newNode.nodeState = NodeStateTypes.market_1;
                break;
        }

        ensureComponent<CoreInteractable>(nodeGameObject);

        Vector3 spawningPosition = new Vector3(120, 0, 5);
        GameObject newNodePlane = Instantiate(nodePlanePrefab);
        newNodePlane.transform.position = spawningPosition;
        newNodePlane.SetActive(false);
        newNode.nodePlaneManagers = newNodePlane.GetComponent(typeof(NodePlaneHandler)) as NodePlaneHandler;
        newNode.init();
        return newNode;
    }

}
