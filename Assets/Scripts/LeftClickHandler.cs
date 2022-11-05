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

    private float basePlaneZ = 1;

    private Camera mainCamera;
    private LayerMask interactableLayerMask;

    private float clickTimer = 0.08f;

    private readonly float baseDraggingPositionZ = 10;


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
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 40, interactableLayerMask);
        if (hit.collider != null)
        {
            handleLeftMouseButtonDown(hit.collider.gameObject);
        }

    }
    // ################ CUSTOM FUNCTION ################

    private void handleLeftMouseButtonDown(GameObject hitGameObject)
    {
        CoreInteractable interactableObject = hitGameObject.GetComponent(typeof(CoreInteractable)) as CoreInteractable;
        IEnumerator task = interactableObject == null ?
            handleClickingOnAGameObject(hitGameObject) : handleClickingOnACoreInteractable(interactableObject);
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

    private IEnumerator handleClickingOnACoreInteractable(CoreInteractable interactableObject)
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
            List<CoreInteractable> draggingObjects = new List<CoreInteractable>();
            interactableObject.gameObject.transform.position = new Vector3(interactableObject.gameObject.transform.position.x,
                interactableObject.gameObject.transform.position.y,
                baseDraggingPositionZ
            );
            draggingObjects.Add(interactableObject);

            StartCoroutine(dragUpdate(draggingObjects, clickedDifferenceInWorld));
        }
    }



    private IEnumerator dragUpdate(List<CoreInteractable> draggingObjects, Vector3 clickedDifferenceInWorld)
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
                    isGonnaApplyMiddleLogic = this.handleSpecialLogic(baseDragableCardObject, initialPostionOfStack, draggingObjects);
                }
            }
            yield return null;
        }

        if (isGonnaApplyMiddleLogic)
        {
            endingSpecialLogic(baseDragableCardObject, draggingObjects);
        }
        dragFinishHandler(draggingObjects);


        bool doApplyMiddleLogic()
        {
            if (draggingObjects[0].interactableType != CoreInteractableType.Cards)
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

    private bool handleSpecialLogic(Card hitCard, Vector3 initialPostionOfStack, List<CoreInteractable> draggingObjects)
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
            endingSpecialLogic(hitCard, draggingObjects);
            return false;
        }

    }

    private void endingSpecialLogic(Card hitCard, List<CoreInteractable> draggingObjects)
    {
        List<Card> draggingCards = new List<Card>();
        foreach (CoreInteractable singleDraggingObject in draggingObjects)
        {
            Card singleCard = singleDraggingObject.getCard();
            if (singleCard != null)
            {
                draggingCards.Add(singleCard);
            }
        }
        hitCard.joinedStack.removeCardsFromStack(draggingCards);
    }

    private void applyDownDragLogic(Card hitCard, Vector3 initialPostionOfCard, List<CoreInteractable> draggingObjects)
    {
        List<Card> qualifiedCards = new List<Card>(hitCard.joinedStack.cards.Where(stacksSingleCard =>
            {
                bool found = draggingObjects.Find(singleDraggingObject => singleDraggingObject.gameObject.GetInstanceID() == stacksSingleCard.gameObject.GetInstanceID());
                return !found;
            }));
        foreach (Card singleCard in qualifiedCards)
        {
            if (hitCard.gameObject.transform.position.y < singleCard.gameObject.transform.position.y && initialPostionOfCard.y > singleCard.gameObject.transform.position.y)
            {
                CoreInteractable interactableGameObject = DragAndDropHelper.getInteractableFromGameObject(singleCard.gameObject);
                draggingObjects.Add(interactableGameObject);
                singleCard.gameObject.transform.position = new Vector3(
                    singleCard.gameObject.transform.position.x,
                    singleCard.gameObject.transform.position.y,
                    baseDraggingPositionZ - ((draggingObjects.Count - 1) * 0.01f));
            }
        }
    }

    private void dragFinishHandler(List<CoreInteractable> draggingObjects)
    {

        // WE ARE ALWAYS ASSUMING THAT EVERYTHING IN A LIST IS THE SAME CLASSES
        CoreInteractable bottomInteractable = draggingObjects[draggingObjects.Count - 1];

        if (bottomInteractable.interactableType == CoreInteractableType.Cards)
        {
            // is card
            Card bottomCard = bottomInteractable.getCard();
            Stackable stackableObject = findTargetToStack(bottomCard);
            if (stackableObject != null)
            {
                // stacking on a coreInteractable
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
                // stacking on a plane
                if (draggingObjects.Count > 1)
                {
                    // stacking on the top card of dragging
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
                    GameObject bottomGameObject = this.findInteractableGameObject(bottomCard);
                    if (bottomGameObject != null)
                    {
                        draggingObjects[0].gameObject.transform.position = new Vector3(draggingObjects[0].gameObject.transform.position.x,
                            draggingObjects[0].gameObject.transform.position.y,
                            bottomGameObject.transform.position.z + 1f);
                    }
                    else
                    {
                        draggingObjects[0].gameObject.transform.position = new Vector3(draggingObjects[0].gameObject.transform.position.x,
                            draggingObjects[0].gameObject.transform.position.y,
                            basePlaneZ);
                    }

                }
            }
        }
        else if (bottomInteractable.interactableType == CoreInteractableType.Nodes)
        {
            for (int i = 0; i < draggingObjects.Count; i++)
            {
                draggingObjects[i].gameObject.transform.position = new Vector3(draggingObjects[i].gameObject.transform.position.x,
                    draggingObjects[i].gameObject.transform.position.y,
                    basePlaneZ
               );
            }
        }
    }

    private Stackable findTargetToStack(Card hitCard)
    {
        hitCard.generateTheCorners();
        Vector3[] corners = { hitCard.leftTopCorner, hitCard.rightTopCorner, hitCard.leftBottomCorner, hitCard.rightBottomCorner };
        int i = 0;
        while (i < 4)
        {
            // Physics.Raycast(corners[i], Vector3.down, out cornerHit, 20, interactableLayerMask)
            // RaycastHit2D cornerHit = Physics2D.Raycast(corners[i], Vector2.down, 20, interactableLayerMask);
            Ray ray = new Ray(corners[i], Vector3.back);
            RaycastHit2D cornerHit = Physics2D.GetRayIntersection(ray, 20, interactableLayerMask);
            if (cornerHit.collider != null)
            {
                Stackable stackableObject = cornerHit.collider.gameObject.GetComponent(typeof(Stackable)) as Stackable;
                if (stackableObject != null)
                {
                    return stackableObject;
                }
            }
            i++;
        }
        return null;
    }

    private GameObject findInteractableGameObject(Card hitCard)
    {
        hitCard.generateTheCorners();
        Vector3[] corners = { hitCard.leftTopCorner, hitCard.rightTopCorner, hitCard.leftBottomCorner, hitCard.rightBottomCorner };
        int i = 0;
        while (i < corners.Length)
        {
            Ray ray = new Ray(corners[i], Vector3.back);
            RaycastHit2D cornerHit = Physics2D.GetRayIntersection(ray, 20, interactableLayerMask);
            // RaycastHit2D cornerHit = Physics2D.Raycast(corners[i], Vector2.down, 20, interactableLayerMask);
            if (cornerHit.collider != null)
            {
                return cornerHit.collider.gameObject;
            }
            i++;
        }
        return null;
    }

    private Vector3 findclickedDifferenceInWorld(CoreInteractable interactableObject)
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Vector3 mousePositionInWorld = mainCamera.ScreenToWorldPoint(mousePosition);
        Vector3 clickedDifferenceInWorld = new Vector3(interactableObject.gameObject.transform.position.x - mousePositionInWorld.x, interactableObject.transform.position.y - mousePositionInWorld.y, 0);
        return clickedDifferenceInWorld;
    }

}
