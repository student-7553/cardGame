using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Core;
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

    private int baseDraggingSortingOrder = 1000;


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
            Interactable interactableObject = hit.collider.gameObject.GetComponent(typeof(Interactable)) as Interactable;

            Vector3 findclickedDifferenceInWorld()
            {
                Vector3 mousePosition = Mouse.current.position.ReadValue();
                Vector3 mousePositionInWorld = mainCamera.ScreenToWorldPoint(mousePosition);
                Vector3 clickedDifferenceInWorld = new Vector3(interactableObject.gameObject.transform.position.x - mousePositionInWorld.x, 0, interactableObject.transform.position.z - mousePositionInWorld.z);
                return clickedDifferenceInWorld;
            }
            Vector3 clickedDifferenceInWorld = findclickedDifferenceInWorld();

            // This will probably change to render Order
            List<Interactable> draggingObjects = new List<Interactable>();
            hit.collider.gameObject.transform.position = new Vector3(hit.collider.gameObject.transform.position.x,
                baseDraggingPositionY,
                hit.collider.gameObject.transform.position.z);
            draggingObjects.Add(interactableObject);

            StartCoroutine(dragUpdate(draggingObjects, clickedDifferenceInWorld));
        }
    }

    private IEnumerator dragUpdate(List<Interactable> draggingObjects, Vector3 clickedDifferenceInWorld)
    {

        Card dragableCardObject = DragAndDropHelper.getCardFromGameObject(draggingObjects[0].gameObject);

        bool applyMiddleLogic = doApplyMiddleLogic();

        float initialStackDistanceToCamera = Vector3.Distance(draggingObjects[0].transform.position, mainCamera.transform.position);

        const float initialCheckIntervel = 0.05f;
        float checkIntervel = initialCheckIntervel;
        float dragTimer = 0;
        bool intervalChecked = false;

        Vector3 initialPostionOfStack = draggingObjects[0].transform.position;

        while (mouseClick.ReadValue<float>() != 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Vector3 movingToPoint = ray.GetPoint(initialStackDistanceToCamera);
            movingToPoint = movingToPoint + clickedDifferenceInWorld;
            DragAndDropHelper.moveDraggingObjects(movingToPoint, draggingObjects);


            dragTimer += Time.deltaTime;
            if (applyMiddleLogic == true)
            {
                handleMiddleLogic();

            }
            yield return null;
        }

        dragFinishHandler(draggingObjects);

        void handleMiddleLogic()
        {
            if (!intervalChecked && dragTimer > checkIntervel)
            {
                intervalChecked = true;
                if (this.handleSpecialLogic(dragableCardObject, initialPostionOfStack, draggingObjects))
                {
                    intervalChecked = false;
                    checkIntervel = checkIntervel + initialCheckIntervel;
                }
                else
                {
                    applyMiddleLogic = false;
                }
            }
        }

        bool doApplyMiddleLogic()
        {
            if (draggingObjects[0]._interactableTypes != InteractableTypes.card)
            {

                return false;

            }
            bool applyMiddleLogic = false;

            if (dragableCardObject != null && dragableCardObject.isStacked)
            {
                applyMiddleLogic = true;
            }
            return applyMiddleLogic;
        }
    }

    private bool handleSpecialLogic(Card hitCard, Vector3 initialPostionOfStack, List<Interactable> draggingObjects)
    {
        Vector3 currentPositionOfCard = hitCard.gameObject.transform.position;
        bool isDraggingMoreCardsFromStack = DragAndDropHelper.getDraggingCardsAngle(initialPostionOfStack, currentPositionOfCard);
        if (isDraggingMoreCardsFromStack)
        {
            this.applyDownDragLogic(hitCard, initialPostionOfStack, draggingObjects);
            return true;
        }
        else
        {
            List<Card> draggingCards = new List<Card>();
            foreach (Interactable singleDraggingObject in draggingObjects)
            {
                Card singleCard = DragAndDropHelper.getCardFromGameObject(singleDraggingObject.gameObject);
                if (singleCard != null)
                {
                    draggingCards.Add(singleCard);
                }
            }
            hitCard.joinedStack.removeCardsFromStack(draggingCards);
            return false;
        }

    }

    private void applyDownDragLogic(Card hitCard, Vector3 initialPostionOfCard, List<Interactable> draggingObjects)
    {
        List<Card> qualifiedCards = new List<Card>(hitCard.joinedStack.cards.Where(stacksSingleCard =>
        {
            bool found = draggingObjects.Find(singleDraggingObject => singleDraggingObject.gameObject.GetInstanceID() == stacksSingleCard.gameObject.GetInstanceID());
            return !found;
        }));
        foreach (Card singleCard in qualifiedCards)
        {
            if (hitCard.gameObject.transform.position.z < singleCard.gameObject.transform.position.z && initialPostionOfCard.z > singleCard.gameObject.transform.position.z)
            {
                singleCard.gameObject.transform.position = new Vector3(
                    singleCard.gameObject.transform.position.x,
                    baseDraggingPositionY,
                    singleCard.gameObject.transform.position.z);
                draggingObjects.Add(DragAndDropHelper.getInteractableFromGameObject(singleCard.gameObject));
            }
        }
    }

    private void dragFinishHandler(List<Interactable> draggingObjects)
    {

        // WE ARE ALWAYS ASSUMING THAT EVERYTHING IN A LIST IS THE SAME CLASSES
        Interactable bottomInteractable = draggingObjects[draggingObjects.Count - 1];


        // if (bottomCard != null)
        if (bottomInteractable._interactableTypes == InteractableTypes.card)
        {
            Card bottomCard = DragAndDropHelper.getCardFromGameObject(bottomInteractable.gameObject);
            //  every single one in array is a card
            List<Card> draggingCards = new List<Card>();
            foreach (Interactable singleDraggingObject in draggingObjects)
            {
                Card singleCard = DragAndDropHelper.getCardFromGameObject(singleDraggingObject.gameObject);
                draggingCards.Add(singleCard);
            }
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
        else
        {
            // is not a card
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
