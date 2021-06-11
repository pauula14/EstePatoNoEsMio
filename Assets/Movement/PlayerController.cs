using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;    
    public float rotationSpeed = 280.0f;
    float horizontal;
    float vertical;
    private void Update() {
        Vector3 moveDirection = Vector3.forward * vertical + Vector3.right * horizontal;
        
        Vector3 projectedCamerafoward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Quaternion rotationToCamera = Quaternion.LookRotation(projectedCamerafoward, Vector3.up);
        
        moveDirection = rotationToCamera * moveDirection;
        
        //Move camera follow player
        //transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationToCamera, rotationSpeed * Time.deltaTime);
        
        //
        //Move camara not follow player
        Quaternion rotationToMoveDirection = Quaternion.LookRotation(moveDirection, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotationToMoveDirection, rotationSpeed * Time.deltaTime);
        //
        
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

    }
    public void OnMoveInput(float horizontal, float vertical){
        this.vertical = vertical;
        this.horizontal = horizontal;
    }

    
}
