using UnityEngine;
using UnityEngine.Events;

using Cinemachine;
using System;
using UnityEngine.InputSystem;

[Serializable]
public class MoveInputEvent : UnityEvent<float, float>{}

public class InputController : MonoBehaviour
{    
    public MoveInputEvent moveInputEvent;
    public CinemachineVirtualCamera camDolly;
    public CinemachineFreeLook camCenital;
    public CinemachineFreeLook camThirdPerson;

    private int index = 1;
    private float cooldownCamera = 0.01f;
    Controls controls;
    private void Awake() {
        controls = new Controls();
    }
    private void OnEnable() {
        controls.Gameplay.Enable();
        controls.Gameplay.Move.performed += OnMovePerformed;
        controls.Gameplay.Move.canceled += OnMovePerformed; 
        controls.Gameplay.CameraSwitch.performed += switchCamera;
        controls.Gameplay.CameraSwitch.canceled += switchCamera; 
        camDolly.enabled = true;
        camCenital.enabled=false;
        camThirdPerson.enabled=false;
    }
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        Vector2 moveInput =  context.ReadValue<Vector2>();
        moveInputEvent.Invoke(moveInput.x, moveInput.y);
        //Debug.Log($"Move Input: {moveInput}");
    }

    private void switchCamera(InputAction.CallbackContext context){
          cooldownCamera -= Time.deltaTime;
        if(cooldownCamera < 0){
            cooldownCamera = 0.01f;
            index++;
            if(index>3)index = 1;
            Debug.Log(index);
            switch (index){
                case 1:
                    camDolly.enabled = true;
                    camCenital.enabled=false;
                    camThirdPerson.enabled=false;
                    break;
                case 2:
                    camDolly.enabled = false;
                    camCenital.enabled=true;
                    camThirdPerson.enabled=false;
                    break;
                case 3:
                    camDolly.enabled = false; 
                    camCenital.enabled=false;
                    camThirdPerson.enabled=true;
                    break;
                default:
                    camDolly.enabled = true;
                    camCenital.enabled=false;
                    camThirdPerson.enabled=false;
                    break;
            }
        }
    }
}
