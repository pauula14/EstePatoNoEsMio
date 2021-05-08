using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Collider))]

public class FlockingAgent : MonoBehaviour
{
    Collider agentCollider;
    public Collider AgentCollider { get { return agentCollider; } }


    void Start()
    {
        agentCollider = GetComponent<Collider>();
    }

    void Update()
    {
        
    }

    public void Move(Vector2 velocity)
    {
        transform.forward = new Vector3 (velocity.x, 0, velocity.y);
        transform.position += new Vector3 (velocity.x, 0, velocity.y) * Time.deltaTime;
    }
}
