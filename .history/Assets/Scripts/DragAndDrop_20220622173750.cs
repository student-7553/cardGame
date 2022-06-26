using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClick;


    private void OnEnable() {
        mouseClick.Enable();
        mouseClick.performed += MousePressed;
    }

    private void OnDisable() {
        mouseClick.performed -= MousePressed;
        mouseClick.Disable();
    }

    private void MousePressed(InputAction.CallbackContext context){

    }
    
}
