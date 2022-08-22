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
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, cardLayerMask))
        {
            // && hit.collider.gameObject.CompareTag("interactable")
            if (hit.collider != null)
            {
                Card hitCard = new Card(hit.collider.gameObject);
                hitCard.cardGameObject.transform.position = new Vector3(hitCard.cardGameObject.transform.position.x, hitCard.cardGameObject.transform.position.y + 1, hitCard.cardGameObject.transform.position.z);

                Vector3 mousePositionInWorld = mainCamera.ScreenToWorldPoint(mousePosition);
                Vector3 differenceIn2dSpace = new Vector3(hitCard.cardGameObject.transform.position.x - mousePositionInWorld.x, 0, hitCard.cardGameObject.transform.position.z - mousePositionInWorld.z);

                StartCoroutine(DragUpdate(hitCard, differenceIn2dSpace));

            }
        }
    }

    private IEnumerator DragUpdate(Card hitCard, Vector3 clickedDifferenceInWorld)
    {
        float initialDistance = Vector3.Distance(hitCard.cardGameObject.transform.position, mainCamera.transform.position);
        while (mouseClick.ReadValue<float>() != 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Vector3 movingToPoint = ray.GetPoint(initialDistance);
            movingToPoint.y = hitCard.cardGameObject.transform.position.y;
            movingToPoint = movingToPoint + clickedDifferenceInWorld;
            hitCard.cardGameObject.transform.position = Vector3.SmoothDamp(hitCard.cardGameObject.transform.position, movingToPoint, ref velocity, mouseDragSpeed);
            yield return null;
        }
        DragFinishHandler(hitCard);
        hitCard.cardGameObject.transform.position = new Vector3(hitCard.cardGameObject.transform.position.x, hitCard.cardGameObject.transform.position.y - 1, hitCard.cardGameObject.transform.position.z);
    }

    private void DragFinishHandler(Card hitCard)
    {
        GameObject stackCard = findCardToStack(hitCard);
        if(stackCard){
            // stack with that card
            // stackCard.transform.position.x, stackCard.transform.position.z


        }
    }

    private GameObject findCardToStack(Card hitCard)
    {

        Vector3 leftTopCornerPoint = new Vector3(clickedObject.transform.position.x - (clickedObject.transform.localScale.x / 2),
        clickedObject.transform.position.y, clickedObject.transform.position.z + (clickedObject.transform.localScale.z / 2));

        Vector3 rightTopCornerPoint = new Vector3(clickedObject.transform.position.x + (clickedObject.transform.localScale.x / 2),
        clickedObject.transform.position.y, clickedObject.transform.position.z + (clickedObject.transform.localScale.z / 2));

        Vector3 leftBottomCornerPoint = new Vector3(clickedObject.transform.position.x - (clickedObject.transform.localScale.x / 2),
        clickedObject.transform.position.y, clickedObject.transform.position.z - (clickedObject.transform.localScale.z / 2));

        Vector3 rightBottomCornerPoint = new Vector3(clickedObject.transform.position.x + (clickedObject.transform.localScale.x / 2),
        clickedObject.transform.position.y, clickedObject.transform.position.z - (clickedObject.transform.localScale.z / 2));

        RaycastHit cornerHit;
        if (
            Physics.Raycast(leftTopCornerPoint, transform.TransformDirection(Vector3.down), out cornerHit, 20, cardLayerMask) ||
            Physics.Raycast(rightTopCornerPoint, transform.TransformDirection(Vector3.down), out cornerHit, 20, cardLayerMask) ||
            Physics.Raycast(leftBottomCornerPoint, transform.TransformDirection(Vector3.down), out cornerHit, 20, cardLayerMask) || 
            Physics.Raycast(rightBottomCornerPoint, transform.TransformDirection(Vector3.down), out cornerHit, 20, cardLayerMask)
        )
        {
            GameObject stackingCardObject = cornerHit.collider.gameObject;
            Debug.Log("Did hit some corner");
            return stackingCardObject;

        }
        return null;
    }

}
