using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using CardGlobal;
using Helpers;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClick;
    [SerializeField]
    private float mouseDragPhysicSpeed = 10;
    [SerializeField]
    private float mouseDragSpeed = .1f;

    private float baseDraggingPositionY = 2;


    private Vector3 velocity = Vector3.zero;

    private Camera mainCamera;
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    private LayerMask cardLayerMask;


    // ################ MonoBehaviour FUNCTION ################

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


    // ################ CUSTOM FUNCTION ################

    private void handleClickingOnACard()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, cardLayerMask))
        {
            Card hitCard = DragAndDropHelper.getCardFromGameObject(hit.collider.gameObject);
            StartCoroutine(dragUpdate(hitCard));
        }
    }

    private IEnumerator dragUpdate(Card hitCard)
    {

        Vector3 findclickedDifferenceInWorld()
        {
            Vector3 mousePosition = Mouse.current.position.ReadValue();
            Vector3 mousePositionInWorld = mainCamera.ScreenToWorldPoint(mousePosition);
            Vector3 clickedDifferenceInWorld = new Vector3(hitCard.gameObject.transform.position.x - mousePositionInWorld.x, 0, hitCard.gameObject.transform.position.z - mousePositionInWorld.z);
            return clickedDifferenceInWorld;
        }

        Vector3 clickedDifferenceInWorld = findclickedDifferenceInWorld();

        List<Card> draggingCards = new List<Card>();
        hitCard.gameObject.transform.position = new Vector3(hitCard.gameObject.transform.position.x, baseDraggingPositionY, hitCard.gameObject.transform.position.z);
        draggingCards.Add(hitCard);

        float initialDistance = Vector3.Distance(hitCard.gameObject.transform.position, mainCamera.transform.position);
        float timer = 0;
        float initialCheckIntervel = 0.05f;
        float checkIntervel = 1;
        bool intervalChecked = false;
        Vector3 initialPostionOfCard = hitCard.gameObject.transform.position;
        while (mouseClick.ReadValue<float>() != 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Vector3 movingToPoint = ray.GetPoint(initialDistance);
            movingToPoint = movingToPoint + clickedDifferenceInWorld;
            DragAndDropHelper.moveDraggingCard(movingToPoint, draggingCards);

            timer += Time.deltaTime;
            float timerCheck = checkIntervel * initialCheckIntervel;

            if (hitCard.isStacked)
            {
                if (timer > timerCheck && !intervalChecked)
                {
                    intervalChecked = true;
                    bool didAppliedLogic = this.handleDownLogic(hitCard, initialPostionOfCard, draggingCards);
                    if (didAppliedLogic)
                    {
                        intervalChecked = false;
                        checkIntervel++;
                    }
                }
            }
            yield return null;
        }
        dragFinishHandler(draggingCards);
    }

    private bool handleDownLogic(Card hitCard, Vector3 initialPostionOfCard, List<Card> draggingCards)
    {
        Vector3 currentPositionOfCard = hitCard.gameObject.transform.position;
        bool isDraggingMoreCardsFromStack = DragAndDropHelper.getDraggingCardsAngle(initialPostionOfCard, currentPositionOfCard);
        if (isDraggingMoreCardsFromStack)
        {
            this.applyDownDragLogic(hitCard, initialPostionOfCard, draggingCards);
            return true;
        }
        else
        {
            draggingCards[0].joinedStack.removeCardsFromStack(draggingCards);
            return false;
        }

    }

    private void applyDownDragLogic(Card hitCard, Vector3 initialPostionOfCard, List<Card> draggingCards)
    {
        List<Card> qualifiedCards = new List<Card>(hitCard.joinedStack.cards.Where(stacksSingleCard =>
        {
            bool found = draggingCards.Find(singleDraggingCard => singleDraggingCard.gameObject.GetInstanceID() == stacksSingleCard.gameObject.GetInstanceID());
            return !found;
        }));
        foreach (Card singleCard in qualifiedCards)
        {
            if (hitCard.gameObject.transform.position.z < singleCard.gameObject.transform.position.z && initialPostionOfCard.z > singleCard.gameObject.transform.position.z)
            {
                singleCard.gameObject.transform.position = new Vector3(
                    singleCard.gameObject.transform.position.x,
                    baseDraggingPositionY - (draggingCards.Count * CardStack.distancePerCards),
                    singleCard.gameObject.transform.position.z);
                draggingCards.Add(singleCard);
            }
        }
    }

    private void dragFinishHandler(List<Card> draggingCards)
    {

        Card bottomCard = draggingCards[draggingCards.Count - 1];
        Stackable stackableObject = findTargetToStack(bottomCard);
        if (stackableObject != null)
        {
            stackableObject.stackOnThis(draggingCards);
        }
        else
        {
            int i = 1;
            foreach (Card singleDraggingCard in draggingCards)
            {
                singleDraggingCard.gameObject.transform.position = new Vector3(singleDraggingCard.gameObject.transform.position.x,
                    Card.cardBaseY + ((draggingCards.Count - i) * CardStack.distancePerCards),
                    singleDraggingCard.gameObject.transform.position.z);
                i++;
            }
        }
    }

    private Stackable findTargetToStack(Card hitCard)
    {
        hitCard.generateTheCorners();
        RaycastHit cornerHit;
        Vector3[] corners = { hitCard.leftTopCorner, hitCard.rightTopCorner, hitCard.leftBottomCorner, hitCard.rightBottomCorner };
        int i = 0;
        while (i < 4)
        {
            if (Physics.Raycast(corners[i], Vector3.down, out cornerHit, 20, cardLayerMask))
            {
                hitCard.gameObject.transform.position = new Vector3(hitCard.gameObject.transform.position.x, hitCard.gameObject.transform.position.y - 1, hitCard.gameObject.transform.position.z);
                Card stackingCard = DragAndDropHelper.getCardFromGameObject(cornerHit.collider.gameObject);
                return stackingCard;
            }
            i++;
        }
        return null;
    }


}
