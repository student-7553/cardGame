using UnityEngine;

public class CardHandler : MonoBehaviour
{
    // Start is called before the first frame update
    private Vector3 defaultCardPoint;
    void Start()
    {
        defaultCardPoint = new Vector3();
    }

    void createCard(int cardId, Vector3 cardOriginPoint, GameObject cardGameObject)
    {
        if (!CardDictionary.globalCardDictionary.ContainsKey(cardId))
        {
            return;
        }
        cardGameObject.name = CardDictionary.globalCardDictionary[cardId].name;
        cardGameObject.tag = "Cards";
        cardGameObject.layer = 6;

        // cardGameObject.GetComponent(typeof(BoxCollider)) as BoxCollider;
        // if ()
        // {

        // }




    }





    void createCard(int cardId, Vector3 cardOriginPoint)
    {
        GameObject newGameObject = new GameObject();
        createCard(cardId, cardOriginPoint, newGameObject);

    }
    void createCard(int cardId)
    {
        createCard(cardId, defaultCardPoint);
    }

}
