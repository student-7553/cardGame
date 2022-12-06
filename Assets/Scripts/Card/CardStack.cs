using System.Collections.Generic;
using UnityEngine;
using Core;

public class CardStack
{
    private static float stackDistance = 5;
    private static float distancePerCards = 0.01f;

    public float cardBaseZ;

    public CardStackType cardStackType;

    public List<Card> cards;

    public CardStack(CardStackType givenStackType)
    {
        cardStackType = givenStackType;
        cardBaseZ = 1;
        cards = new List<Card>();
    }

    public void alignCards(Vector3 originPoint)
    {
        if (cards.Count == 0)
        {
            return;
        }
        Card rootCard = this.getRootCard();
        rootCard.transform.position = new Vector3(originPoint.x, originPoint.y, cardBaseZ);

        // we are not loopting through first card because it's the origin point
        for (int i = 1; i < cards.Count; i++)
        {
            Card cardInSubject = cards[i];
            Vector3 newPostionForCardInSubject = new Vector3(rootCard.transform.position.x,
                rootCard.transform.position.y - (stackDistance * i),
                rootCard.transform.position.z + (i * distancePerCards));
            cardInSubject.transform.position = newPostionForCardInSubject;
            cardInSubject.generateTheCorners();
        }
    }


    public void alignCards()
    {
        if (cards.Count <= 1)
        {
            return;
        }

        Card rootCard = this.getRootCard();
        this.alignCards(rootCard.transform.position);
    }

    public void changeActiveStateOfAllCards(bool isActive)
    {
        foreach (Card singleCard in cards)
        {
            singleCard.gameObject.SetActive(isActive);
        }
    }

    public void removeCardsFromStack(List<Card> removingCards)
    {
        Card rootCard = this.getRootCard();
        Vector3 rootCardPosition = rootCard.transform.position;

        foreach (Card singleCard in removingCards)
        {
            cards.Remove(singleCard);
            singleCard.removeFromCardStack();
        }
        if (cards.Count == 0)
        {
            return;
        }
        this.alignCards(rootCardPosition);

    }

    public Card getRootCard()
    {
        if (cards.Count > 0)
        {
            return cards[0];
        }
        return null;
    }

    public void addCardsToStack(List<Card> addingCards)
    {
        cards.AddRange(addingCards);
        foreach (Card singleCard in addingCards)
        {
            singleCard.addToCardStack(this);
        }
        this.alignCards();
    }

    public void addCardToStack(Card addingCard)
    {
        List<Card> addingCards = new List<Card>(new Card[] { addingCard });
        addCardsToStack(addingCards);
    }

    private void logCards()
    {
        foreach (Card singleCard in cards)
        {
            Debug.Log(singleCard);
        }
    }

    public List<int> getAllCardIds()
    {
        List<int> ids = new List<int>();
        foreach (Card singleCard in cards)
        {
            ids.Add(singleCard.id);
        }
        return ids;
    }

    public List<int> getActiveCardIds()
    {
        List<int> ids = new List<int>();
        foreach (Card singleCard in cards)
        {
            if (!singleCard.isDisabled)
            {

                ids.Add(singleCard.id);
            }
        }
        return ids;
    }

    public List<int> getNonTypeCardIds()
    {
        List<int> ids = new List<int>();
        foreach (Card singleCard in cards)
        {
            if (
                CardDictionary.globalCardDictionary[singleCard.id].type != CardsTypes.Gold &&
                CardDictionary.globalCardDictionary[singleCard.id].type != CardsTypes.Electricity &&
                CardDictionary.globalCardDictionary[singleCard.id].type != CardsTypes.Food
            )
            {
                ids.Add(singleCard.id);
            }

        }
        return ids;
    }

    public void moveRootCardToPosition(float newX, float newY)
    {
        Card rootCard = this.getRootCard();
        if (rootCard == null)
        {
            return;
        }
        rootCard.gameObject.transform.position = new Vector3(
            newX,
            newY,
            cardBaseZ
            );

        alignCards();
    }

}
