using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragAndDrop : MonoBehaviour
{
    [SerializeField]
    private InputAction mouseClick;


    private void OnEnable() {
        mouseClick.Enable();
        
    }
    
}
