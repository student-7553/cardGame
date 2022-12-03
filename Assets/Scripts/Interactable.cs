using UnityEngine;
using Core;

public class CoreInteractable : MonoBehaviour
{

    public CoreInteractableType interactableType;

    public SpriteRenderer spriteRenderer;

    public bool isDisabled;

    private void Awake()
    {
        isDisabled = false;
        spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
        if (this.tag == CoreInteractableType.Cards.ToString())
        {
            interactableType = CoreInteractableType.Cards;
        }
        else if (this.tag == CoreInteractableType.Nodes.ToString())
        {
            interactableType = CoreInteractableType.Nodes;
        }
    }

    public IStackable getStackable()
    {
        if (interactableType == CoreInteractableType.Cards)
        {
            Card card = gameObject.GetComponent(typeof(Card)) as Card;
            return card;
        }
        else if (interactableType == CoreInteractableType.Nodes)
        {
            Node node = gameObject.GetComponent(typeof(Node)) as Node;
            return node;
        }
        return null;
    }

    public Card getCard()
    {
        if (interactableType == CoreInteractableType.Cards)
        {
            Card card = gameObject.GetComponent(typeof(Card)) as Card;
            return card;
        }
        return null;
    }

    public Node getNode()
    {
        if (interactableType == CoreInteractableType.Nodes)
        {
            Node node = gameObject.GetComponent(typeof(Node)) as Node;
            return node;
        }
        return null;
    }
}
