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
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null)
            {
                StartCoroutine(DragUpdate(hit.collider.gameObject));

            }
        }
    }

    private IEnumerator DragUpdate(GameObject clickedObject)
    {
        yield return null;
        float initialDistance = Vector3.Distance(clickedObject.transform.position, mainCamera.transform.position);
        // Debug.Log(initialDistance);
        clickedObject.TryGetComponent<Rigidbody>(out Rigidbody clickedRigidBody);
        while (mouseClick.ReadValue<float>() != 0)
        {
            Debug.Log('MOving RG');
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (clickedRigidBody != null)
            {
                Vector3 direction = ray.GetPoint(initialDistance) - clickedObject.transform.position;
                clickedRigidBody.velocity = direction * mouseDragPhysicSpeed;
                yield return waitForFixedUpdate;
            }
            else
            {
                Vector3 movingToPoint = ray.GetPoint(initialDistance);
                movingToPoint.y = 0;
                // clickedObject.transform.position
                clickedObject.transform.position = Vector3.SmoothDamp(clickedObject.transform.position, movingToPoint, ref velocity, mouseDragSpeed);
                yield return null;
            }
        }
    }

}
