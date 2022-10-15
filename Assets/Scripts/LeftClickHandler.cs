using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Core;
using Helpers;

public class LeftClickHandler : MonoBehaviour
{
    private InputAction leftClick;
    // [SerializeField]
    // private float mouseDragPhysicSpeed = 10;
    // [SerializeField]
    // private float mouseDragSpeed = .1f;
    // private Vector3 velocity = Vector3.zero;
    // private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    // private int BASE_SORTING_ORDER_WHILE_DRAGGING = 1000;

    private Camera mainCamera;
    private LayerMask interactableLayerMask;

    private float clickTimer = 0.1f;

    private float baseDraggingPositionY = 10;

    // ################ MonoBehaviour FUNCTION ################

    private void Awake()
    {
        leftClick = new InputAction(binding: "<Mouse>/leftButton");
        mainCamera = Camera.main;
        interactableLayerMask = LayerMask.GetMask("Interactable");
    }

    private void OnEnable()
    {
        leftClick.Enable();
        leftClick.performed += MousePressed;
    }

    private void OnDisable()
    {
        leftClick.performed -= MousePressed;
        leftClick.Disable();
    }

    private void MousePressed(InputAction.CallbackContext context)
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, interactableLayerMask))
        {
            Debug.Log(hit.collider.gameObject);
            handleLeftMouseButtonDown(hit.collider.gameObject);
        }
    }
    // ################ CUSTOM FUNCTION ################

    private void handleLeftMouseButtonDown(GameObject hitGameObject)
    {
        Interactable interactableObject = hitGameObject.GetComponent(typeof(Interactable)) as Interactable;
        IEnumerator task = interactableObject == null ?
            handleClickingOnAGameObject(hitGameObject) : handleClickingOnAInteractable(interactableObject);
        StartCoroutine(task);
    }

    private IEnumerator handleClickingOnAGameObject(GameObject hitGameObject)
    {
        yield return new WaitForSeconds(clickTimer);
        if (leftClick.ReadValue<float>() == 0f)
        {
            // clicked
            hitGameObject.GetComponent<IClickable>()?.OnClick();
        }
    }

    private IEnumerator handleClickingOnAInteractable(Interactable interactableObject)
    {
        Vector3 clickedDifferenceInWorld = findclickedDifferenceInWorld(interactableObject);
        yield return new WaitForSeconds(clickTimer);
        if (leftClick.ReadValue<float>() == 0f)
        {
            // clicked
            interactableObject.gameObject.GetComponent<IClickable>()?.OnClick();
        }
        else
        {
            // dragging
            List<Interactable> draggingObjects = new List<Interactable>();
            interactableObject.gameObject.transform.position = new Vector3(interactableObject.gameObject.transform.position.x,
                baseDraggingPositionY,
                interactableObject.gameObject.transform.position.z);
            draggingObjects.Add(interactableObject);

            StartCoroutine(dragUpdate(draggingObjects, clickedDifferenceInWorld));
        }
    }

    private IEnumerator dragUpdate(List<Interactable> draggingObjects, Vector3 clickedDifferenceInWorld)
    {

        Card baseDragableCardObject = draggingObjects[0].getCard();

        bool isGonnaApplyMiddleLogic = doApplyMiddleLogic();

        float initialDistanceToCamera = Vector3.Distance(draggingObjects[0].transform.position, mainCamera.transform.position);

        const float initialCheckIntervel = 0.01f;
        float checkIntervel = initialCheckIntervel;
        float dragTimer = 0;

        Vector3 initialPostionOfStack = draggingObjects[0].transform.position;

        while (leftClick.ReadValue<float>() != 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Vector3 movingToPoint = ray.GetPoint(initialDistanceToCamera);
            movingToPoint = movingToPoint + clickedDifferenceInWorld;
            DragAndDropHelper.moveDraggingObjects(movingToPoint, draggingObjects);

            dragTimer += Time.deltaTime;
            if (isGonnaApplyMiddleLogic == true)
            {
                if (dragTimer > checkIntervel)
                {
                    dragTimer = 0;
                    if (!this.handleSpecialLogic(baseDragableCardObject, initialPostionOfStack, draggingObjects))
                    {
                        isGonnaApplyMiddleLogic = false;
                    }
                }

            }
            yield return null;
        }

        dragFinishHandler(draggingObjects);

        bool doApplyMiddleLogic()
        {
            if (draggingObjects[0].interactableType != InteractableType.Cards)
            {
                return false;
            }
            bool isGonnaApplyMiddleLogic = false;

            if (baseDragableCardObject != null && baseDragableCardObject.isStacked)
            {
                isGonnaApplyMiddleLogic = true;
            }
            return isGonnaApplyMiddleLogic;
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
                Card singleCard = singleDraggingObject.getCard();
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
                Interactable interactableGameObject = DragAndDropHelper.getInteractableFromGameObject(singleCard.gameObject);
                draggingObjects.Add(interactableGameObject);
                singleCard.gameObject.transform.position = new Vector3(
                    singleCard.gameObject.transform.position.x,
                    baseDraggingPositionY - ((draggingObjects.Count - 1) * 0.01f),
                    singleCard.gameObject.transform.position.z);
            }
        }
    }

    private void dragFinishHandler(List<Interactable> draggingObjects)
    {

        // WE ARE ALWAYS ASSUMING THAT EVERYTHING IN A LIST IS THE SAME CLASSES
        Interactable bottomInteractable = draggingObjects[draggingObjects.Count - 1];

        if (bottomInteractable.interactableType == InteractableType.Cards)
        {
            // is card
            Card bottomCard = bottomInteractable.getCard();
            Stackable stackableObject = findTargetToStack(bottomCard);

            if (stackableObject != null)
            {
                List<Card> stackingCards = new List<Card>();
                for (int i = 0; i < draggingObjects.Count; i++)
                {
                    Card singleCard = draggingObjects[i].getCard();
                    stackingCards.Add(singleCard);
                }
                stackableObject.stackOnThis(stackingCards);
            }
            else
            {
                if (draggingObjects.Count > 1)
                {
                    List<Card> stackingCards = new List<Card>();
                    for (int i = 1; i < draggingObjects.Count; i++)
                    {
                        Card singleCard = draggingObjects[i].getCard();
                        stackingCards.Add(singleCard);
                    }

                    Card baseCard = draggingObjects[0].getCard();
                    baseCard.stackOnThis(stackingCards);
                }
                else
                {
                    for (int i = 0; i < draggingObjects.Count; i++)
                    {
                        draggingObjects[i].gameObject.transform.position = new Vector3(draggingObjects[i].gameObject.transform.position.x,
                            1,
                            draggingObjects[i].gameObject.transform.position.z);
                    }
                }
            }
        }
        else if (bottomInteractable.interactableType == InteractableType.Nodes)
        {
            for (int i = 0; i < draggingObjects.Count; i++)
            {
                draggingObjects[i].gameObject.transform.position = new Vector3(draggingObjects[i].gameObject.transform.position.x,
                    1,
                    draggingObjects[i].gameObject.transform.position.z);
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
            if (Physics.Raycast(corners[i], Vector3.down, out cornerHit, 20, interactableLayerMask))
            {
                Interactable interactableObject = cornerHit.collider.gameObject.GetComponent(typeof(Interactable)) as Interactable;
                return interactableObject.getStackable();
            }
            i++;
        }
        return null;
    }

    private Vector3 findclickedDifferenceInWorld(Interactable interactableObject)
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 mousePositionInWorld = mainCamera.ScreenToWorldPoint(mousePosition);
        Vector3 clickedDifferenceInWorld = new Vector3(interactableObject.gameObject.transform.position.x - mousePositionInWorld.x, 0, interactableObject.transform.position.z - mousePositionInWorld.z);
        return clickedDifferenceInWorld;
    }

}
