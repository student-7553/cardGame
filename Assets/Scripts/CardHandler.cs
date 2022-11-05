using UnityEngine;

public class CardHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 defaultCardPoint;
    public GameObject cardPrefab;

    public Sprite[] cardSprites;
    public Sprite[] nodeSprites;

    void Start()
    {
        defaultCardPoint = new Vector3();
    }

    public void createCard(int cardId, GameObject cardGameObject, Vector3 cardOriginPoint)
    {
        if (!CardDictionary.globalCardDictionary.ContainsKey(cardId))
        {
            return;
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




    public void createCard(int cardId, Vector3 cardOriginPoint)
    {

        GameObject newNodePlane = Instantiate(cardPrefab);
        newNodePlane.transform.position = cardOriginPoint;
        newNodePlane.SetActive(true);

        createCard(cardId, newNodePlane, cardOriginPoint);

    }
    public void createCard(int cardId)
    {
        createCard(cardId, defaultCardPoint);
    }

}
