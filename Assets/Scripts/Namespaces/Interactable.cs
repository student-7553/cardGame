using UnityEngine;

namespace Core
{
    public enum InteractableTypes
    {
        Cards,
        Nodes
    }

    public class Interactable : MonoBehaviour
    {

        public InteractableTypes _interactableTypes;

        public SpriteRenderer spriteRenderer;


        private void Awake()
        {
            spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            if (this.tag == InteractableTypes.Cards.ToString())
            {
                _interactableTypes = InteractableTypes.Cards;
            }
            else if (this.tag == InteractableTypes.Nodes.ToString())
            {
                _interactableTypes = InteractableTypes.Nodes;
            }
        }

        public Stackable getStackable()
        {
            if (_interactableTypes == InteractableTypes.Cards)
            {
                Card card = gameObject.GetComponent(typeof(Card)) as Card;
                return card;
            }
            else if (_interactableTypes == InteractableTypes.Nodes)
            {
                Node node = gameObject.GetComponent(typeof(Node)) as Node;
                return node;
            }
            return null;
        }
    }
}