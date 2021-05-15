using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MoveInputEvent : UnityEvent<float, float>{}

public class InputController : MonoBehaviour
{    

    //FORMA 1
    /*
    Animator animator;
    int isWalkingHash;
    MamaInputController input;
    Vector2 currentMovement;
    bool movementPressed;

    private void Awake() {
        input = new MamaInputController();

        input.MamaMovement.Movement.performed += ctx =>  {
            currentMovement = ctx.ReadValue<Vector2>();    
            movementPressed = currentMovement.x != 0 || currentMovement.y !=0;
        };

    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void handleMovement(){
        bool isWalking = animator.GetBool(isWalkingHash);

        if(movementPressed && !isWalking){
            animator.SetBool(isWalkingHash, true);
        }
        if(!movementPressed && !isWalking){
            animator.SetBool(isWalkingHash, false);
        }
    }

    private void OnEnable() {
        input.MamaMovement.Enable();
    }
    private void OnDisable() {
        input.MamaMovement.Disable();
    }

    */

    //FORMA 2

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
