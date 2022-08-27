using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using CardGlobal;


// Z = Y

public class DragAndDrop : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClick;
    [SerializeField]
    private float mouseDragPhysicSpeed = 10;
    [SerializeField]
    private float mouseDragSpeed = .1f;


    private Vector3 velocity = Vector3.zero;

    private Camera mainCamera;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private LayerMask cardLayerMask;

    private void Awake()
    {
        mainCamera = Camera.main;
        cardLayerMask = LayerMask.GetMask("Cards");
    }



    private void OnEnable()
    {
        mouseClick.Enable();
        mouseClick.performed += MousePressed;
    }

    private void OnDisable()
    {
        mouseClick.performed -= MousePressed;
        mouseClick.Disable();
    }

    private void MousePressed(InputAction.CallbackContext context)
    {
        handleClickingOnACard();

    }

    private void handleClickingOnACard()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, cardLayerMask))
        {
            // && hit.collider.gameObject.CompareTag("interactable")
            if (hit.collider != null)
            {
                Card hitCard = hit.collider.gameObject.GetComponent(typeof(Card)) as Card;
                if (hitCard == null)
                {
                    hitCard = hit.collider.gameObject.AddComponent<Card>();
                }

                hitCard.gameObject.transform.position = new Vector3(hitCard.gameObject.transform.position.x, hitCard.gameObject.transform.position.y + 1, hitCard.gameObject.transform.position.z);

                Vector3 mousePositionInWorld = mainCamera.ScreenToWorldPoint(mousePosition);
                Vector3 differenceIn2dSpace = new Vector3(hitCard.gameObject.transform.position.x - mousePositionInWorld.x, 0, hitCard.gameObject.transform.position.z - mousePositionInWorld.z);

                StartCoroutine(dragUpdate(hitCard, differenceIn2dSpace));

            }
        }
    }

    private void dragFinishHandler(Card hitCard)
    {
        Card stackCard = findCardToStack(hitCard);
        if (stackCard != null)
        {
            stackThatCard(hitCard, stackCard);
        }

        Debug.Log(hitCard.isStacked);
    }



    private void stackThatCard(Card movingCard, Card originCard)
    {
        Card[] stackingCards = { movingCard, originCard };
        CardStack newStack = new CardStack(stackingCards);

    }

    private IEnumerator dragUpdate(Card hitCard, Vector3 clickedDifferenceInWorld)
    {
        float initialDistance = Vector3.Distance(hitCard.gameObject.transform.position, mainCamera.transform.position);
        while (mouseClick.ReadValue<float>() != 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Vector3 movingToPoint = ray.GetPoint(initialDistance);
            movingToPoint.y = hitCard.gameObject.transform.position.y;
            movingToPoint = movingToPoint + clickedDifferenceInWorld;
            hitCard.gameObject.transform.position = Vector3.SmoothDamp(hitCard.gameObject.transform.position, movingToPoint, ref velocity, mouseDragSpeed);
            yield return null;
        }
        dragFinishHandler(hitCard);
        hitCard.gameObject.transform.position = new Vector3(hitCard.gameObject.transform.position.x, hitCard.gameObject.transform.position.y - 1, hitCard.gameObject.transform.position.z);
    }

    private Card findCardToStack(Card hitCard)
    {
        hitCard.generateTheCorners();
        RaycastHit cornerHit;
        if (
            Physics.Raycast(hitCard.leftTopCorner, transform.TransformDirection(Vector3.down), out cornerHit, 20, cardLayerMask) ||
            Physics.Raycast(hitCard.rightTopCorner, transform.TransformDirection(Vector3.down), out cornerHit, 20, cardLayerMask) ||
            Physics.Raycast(hitCard.leftBottomCorner, transform.TransformDirection(Vector3.down), out cornerHit, 20, cardLayerMask) ||
            Physics.Raycast(hitCard.rightBottomCorner, transform.TransformDirection(Vector3.down), out cornerHit, 20, cardLayerMask)
        )
        {
            Card stackingCard = cornerHit.collider.gameObject.AddComponent<Card>();
            return stackingCard;

        }
        return null;
    }

}
