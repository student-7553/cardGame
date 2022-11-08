using UnityEngine;
[DefaultExecutionOrder(-100)]
public class CardHandler : MonoBehaviour
{

    public static CardHandler current;

    private Vector3 defaultCardPoint;
    public GameObject cardPrefab;

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

    // public void deleteCard(GameObject cardGameObject)
    // {
    //     Destroy(cardGameObject);
    // }

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

}
