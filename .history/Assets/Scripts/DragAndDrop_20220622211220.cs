using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

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



    private void Awake()
    {
        mainCamera = Camera.main;
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
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("interactable"))
            {
                Vector3 mousePositionInWorld = mainCamera.ScreenToWorldPoint(mousePosition);
                GameObject clickedObject = hit.collider.gameObject;
                Debug.Log(clickedObject.tag);
                // clickedObject.tag
                Vector2 differenceIn2dSpace = new Vector2(mousePositionInWorld.x - clickedObject.transform.position.x, mousePositionInWorld.z - clickedObject.transform.position.z);
                StartCoroutine(DragUpdate(clickedObject, differenceIn2dSpace));

            }
        }
    }

    private IEnumerator DragUpdate(GameObject clickedObject, Vector2 differenceIn2dSpace)
    {
        float initialDistance = Vector3.Distance(clickedObject.transform.position, mainCamera.transform.position);
        // clickedObject.TryGetComponent<Rigidbody>(out Rigidbody clickedRigidBody);
        while (mouseClick.ReadValue<float>() != 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            // if (clickedRigidBody != null)
            // {
            //     Vector3 direction = ray.GetPoint(initialDistance) - clickedObject.transform.position;
            //     clickedRigidBody.velocity = direction * mouseDragPhysicSpeed;
            //     yield return waitForFixedUpdate;
            // }
            // else
            // {
            Vector3 movingToPoint = ray.GetPoint(initialDistance);

            // 
            movingToPoint.y = 1;
            // clickedObject.transform.position
            clickedObject.transform.position = Vector3.SmoothDamp(clickedObject.transform.position, movingToPoint, ref velocity, mouseDragSpeed);
            yield return null;
            // }
        }
    }

}
