using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


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
                GameObject clickedObject = hit.collider.gameObject;
                clickedObject.transform.position = new Vector3(clickedObject.transform.position.x, clickedObject.transform.position.y + 1, clickedObject.transform.position.z);

                Vector3 mousePositionInWorld = mainCamera.ScreenToWorldPoint(mousePosition);
                Vector3 differenceIn2dSpace = new Vector3(clickedObject.transform.position.x - mousePositionInWorld.x, 0, clickedObject.transform.position.z - mousePositionInWorld.z);

                StartCoroutine(DragUpdate(clickedObject, differenceIn2dSpace));

            }
        }
    }

    private IEnumerator DragUpdate(GameObject clickedObject, Vector3 clickedDifferenceInWorld)
    {
        float initialDistance = Vector3.Distance(clickedObject.transform.position, mainCamera.transform.position);
        while (mouseClick.ReadValue<float>() != 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            Vector3 movingToPoint = ray.GetPoint(initialDistance);
            movingToPoint.y = clickedObject.transform.position.y;
            movingToPoint = movingToPoint + clickedDifferenceInWorld;
            clickedObject.transform.position = Vector3.SmoothDamp(clickedObject.transform.position, movingToPoint, ref velocity, mouseDragSpeed);
            yield return null;
        }
        DragFinishHandler(clickedObject);
    }

    private void DragFinishHandler(GameObject clickedObject)
    {


        bool willCardStack = willStackWithAnotherCard(clickedObject);

        clickedObject.transform.position = new Vector3(clickedObject.transform.position.x, clickedObject.transform.position.y - 1, clickedObject.transform.position.z);



        // We will stack here.

        // int layerMask = 1 << 8;

        // // This would cast rays only against colliders in layer 8.
        // // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        // layerMask = ~layerMask;


        // // Does the ray intersect any objects excluding the player layer
        // if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, layerMask))
        // {
        //     Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        //     Debug.Log("Did Hit");
        // }
        // else
        // {
        //     Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        //     Debug.Log("Did not Hit");
        // }


    }

    private bool willStackWithAnotherCard(GameObject clickedObject)
    {

        Vector3 leftTopCornerPoint = new Vector3(clickedObject.transform.position.x - (clickedObject.transform.localScale.x / 2),
        clickedObject.transform.position.y, clickedObject.transform.position.z + (clickedObject.transform.localScale.z / 2));

        Vector3 rightTopCornerPoint = new Vector3(clickedObject.transform.position.x + (clickedObject.transform.localScale.x / 2),
        clickedObject.transform.position.y, clickedObject.transform.position.z + (clickedObject.transform.localScale.z / 2));

        Vector3 leftBottomCornerPoint = new Vector3(clickedObject.transform.position.x - (clickedObject.transform.localScale.x / 2),
        clickedObject.transform.position.y, clickedObject.transform.position.z - (clickedObject.transform.localScale.z / 2));

        Vector3 rightBottomCornerPoint = new Vector3(clickedObject.transform.position.x + (clickedObject.transform.localScale.x / 2),
        clickedObject.transform.position.y, clickedObject.transform.position.z - (clickedObject.transform.localScale.z / 2));

        RaycastHit leftTopCornerHit;
        if (Physics.Raycast(leftTopCornerPoint, transform.TransformDirection(Vector3.down), out leftTopCornerHit, 20, cardLayerMask))
        {
            GameObject stackingCardObject = leftTopCornerHit.collider.gameObject;
            Debug.Log("Did hit leftTopCornerHit");
        }
        RaycastHit rightTopCornerHit;
        if (Physics.Raycast(rightTopCornerPoint, transform.TransformDirection(Vector3.down), out rightTopCornerHit, 20, cardLayerMask))
        {
            Debug.Log("Did hit rightTopCornerHit");
        }
        RaycastHit leftBottomCornerHit;
        if (Physics.Raycast(leftBottomCornerPoint, transform.TransformDirection(Vector3.down), out leftBottomCornerHit, 20, cardLayerMask))
        {
            Debug.Log("Did hit leftBottomCornerHit");
        }
        RaycastHit rightBotttomCornerHit;
        if (Physics.Raycast(rightBottomCornerPoint, transform.TransformDirection(Vector3.down), out rightBotttomCornerHit, 20, cardLayerMask))
        {
            Debug.Log("Did hit rightBotttomCornerHit");
        }

        return true;
    }

}
