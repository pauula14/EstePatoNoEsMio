using UnityEngine;
using UnityEngine.Events;

using System;
using System.Collections;
using UnityEngine.InputSystem;

[Serializable]
public class MoveInputEvent : UnityEvent<float, float>{}

public class InputController : MonoBehaviour
{    
    public MoveInputEvent moveInputEvent;
    Controls controls;
    private void Awake() {
        controls = new Controls();
    }
    private void OnEnable() {
        controls.Gameplay.Enable();
        controls.Gameplay.Move.performed += OnMovePerformed;
        controls.Gameplay.Move.canceled += OnMovePerformed; 
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 moveInput =  context.ReadValue<Vector2>();
        moveInputEvent.Invoke(moveInput.x, moveInput.y);
        Debug.Log($"Move Input: {moveInput}");
    }
}
