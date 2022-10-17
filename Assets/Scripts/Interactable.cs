using UnityEngine;

namespace Core
{
    public class CoreInteractable : MonoBehaviour
    {

        public CoreInteractableType interactableType;

        public SpriteRenderer spriteRenderer;


        private void Awake()
        {
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

        public Stackable getStackable()
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
}