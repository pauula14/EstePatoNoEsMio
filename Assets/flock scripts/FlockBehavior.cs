using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockBehavior : MonoBehaviour
{/*
    [SerializeField] private float FOVangle; //angulo de campo de vision
    [SerializeField] private float smoothDamp;

    private List<FlockBehavior> cohesionNeighbors = new List<FlockBehavior>();
    private Flock assignedFlock;
    private Vector3 currentVelocity;
    private float speed;


    public void AssignFlock(Flock flock)
    {
        assignedFlock = flock;
    }

    public void InitializeSpeed (float speed)
    {
        this.speed = speed;
    }

    public void MoveBehavior()
    {
        FindNeighbors();
        CalculateSpeed();

        var cohesionVector = CalculateCohesionVector();

        var moveVector = Vector3.SmoothDamp(transform.forward, cohesionVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;

        if (moveVector == Vector3.zero)
        {
            moveVector = transform.forward;
        }
            
        transform.forward = moveVector;
        transform.position += moveVector * Time.deltaTime;
    }

    private void FindNeighbors()
    {
        cohesionNeighbors.Clear();

        var agents = assignedFlock.agents;

        for(int i = 0; i < agents.Length; i++)
        {
            var currentAgent = agents[i];
            if (currentAgent != this)
            {
                float currentNeighborDistanceSqr = Vector3.SqrMagnitude(currentAgent.transform.position - transform.position);
                if (currentNeighborDistanceSqr <= assignedFlock.CohesionDistance * assignedFlock.CohesionDistance)
                {
                    cohesionNeighbors.Add(currentAgent);
                }
            }
        }
    }

    private Vector3 CalculateCohesionVector()
    {
        var cohesionVector = Vector3.zero;

        if (cohesionNeighbors.Count == 0)
        {
            return cohesionVector;
        }

        int neighborsInFOV = 0; //Vecinos en el campo de vision

        for(int i = 0; i < cohesionNeighbors.Count; i++)
        {
            if (IsInFOV(cohesionNeighbors[i].transform.position))
            {
                neighborsInFOV++;
                cohesionVector += cohesionNeighbors[i].transform.position;
            }
        }

        if (neighborsInFOV == 0)
        {
            return cohesionVector;
        }

        cohesionVector /= neighborsInFOV;
        cohesionVector -= transform.position;
        cohesionVector = cohesionVector.normalized;

        return cohesionVector;
    }

    private void CalculateSpeed()
    {
        speed = 0;

        for (int i = 0; i < cohesionNeighbors.Count; i++)
        {
            speed += cohesionNeighbors[i].speed;
        }

        speed /= cohesionNeighbors.Count;
        speed = Mathf.Clamp(speed, assignedFlock.MinSpeed, assignedFlock.MaxSpeed);
    }

    private bool IsInFOV(Vector3 position)
    {
        return Vector3.Angle(transform.forward, position - transform.position) <= FOVangle;
    }*/
}
