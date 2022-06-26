using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClick;
    [SerializeField]
    private float mouseDragSpeed = 10;

    private Camera mainCamera;

    

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
        float initialDistance = Vector3.Distance(clickedObject.transform.position, mainCamera.transform.position);

        clickedObject.TryGetComponent<Rigidbody>(out Rigidbody clickedRigidBody);
        while(mouseClick.ReadValue<float>() != 0){
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if(clickedRigidBody != null){
                Vector3 direction = ray.GetPoint(initialDistance) - clickedObject.transform.position;
                clickedRigidBody.velocity = direction * mouseDragSpeed;
                yield return new WaitForFixedUpdate();
            }
        }
    }

}
