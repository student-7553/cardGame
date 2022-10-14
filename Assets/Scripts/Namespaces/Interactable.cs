using UnityEngine;

namespace Core
{
    public class Interactable : MonoBehaviour
    {

        public InteractableType interactableType;

        public SpriteRenderer spriteRenderer;


        private void Awake()
        {
            spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            if (this.tag == InteractableType.Cards.ToString())
            {
                interactableType = InteractableType.Cards;
            }
            else if (this.tag == InteractableType.Nodes.ToString())
            {
                interactableType = InteractableType.Nodes;
            }
        }

        public Stackable getStackable()
        {
            if (interactableType == InteractableType.Cards)
            {
                Card card = gameObject.GetComponent(typeof(Card)) as Card;
                return card;
            }
            else if (interactableType == InteractableType.Nodes)
            {
                Node node = gameObject.GetComponent(typeof(Node)) as Node;
                return node;
            }
            return null;
        }

        public Card getCard()
        {
            if (interactableType == InteractableType.Cards)
            {
                Card card = gameObject.GetComponent(typeof(Card)) as Card;
                return card;
            }
            return null;
        }

        public Node getNode()
        {
            if (interactableType == InteractableType.Nodes)
            {
                Node node = gameObject.GetComponent(typeof(Node)) as Node;
                return node;
            }
            return null;
        }
    }
}