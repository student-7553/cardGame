using UnityEngine;

namespace Core
{
    public enum InteractableTypes
    {
        card,
        node
    }

    public class Interactable : MonoBehaviour
    {
        public bool isStacked;

        public InteractableTypes _interactableTypes;

        private SpriteRenderer _spriteRenderer;


        private void Awake()
        {
            _spriteRenderer = gameObject.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            _interactableTypes = InteractableTypes.card;
        }
    }
}