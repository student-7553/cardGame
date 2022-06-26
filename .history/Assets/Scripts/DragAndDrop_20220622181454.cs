using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClick;
    private Camera mainCamera;

    private void Awake() {
        mainCamera = Camera.main;
    }



    private void OnEnable() {
        mouseClick.Enable();
        mouseClick.performed += MousePressed;
    }

    private void OnDisable() {
        mouseClick.performed -= MousePressed;
        mouseClick.Disable();
    }

    private void MousePressed(InputAction.CallbackContext context){
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit)){

        }
    }
    
}
