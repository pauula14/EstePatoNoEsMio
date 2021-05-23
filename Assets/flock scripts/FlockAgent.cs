using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAgent : MonoBehaviour
{
    [SerializeField] private float FOVAngle;
    [SerializeField] private float smoothDamp;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private Vector3[] directionsToCheckWhenAvoidingObstacles; //Direcciones que compruba cuando se choca probablmente no haga falta


    private List<FlockAgent> cohesionNeighbours = new List<FlockAgent>();
    private List<FlockAgent> avoidanceNeighbours = new List<FlockAgent>();
    private List<FlockAgent> aligementNeighbours = new List<FlockAgent>();
    private Flock assignedFlock;
    public Vector3 currentVelocity;
    private Vector3 currentObstacleAvoidanceVector;
    public float speed;

    private float timeObstacle;
    public Vector3 positionObstacle;
    private bool obstacle = false;

    public Transform myTransform { get; set; }

    private void Awake()
    {
        myTransform = transform; //Ahorra complejidad computacional
    }

    private void Update()
    {
        if (obstacle)
        {
            if (/*(Time.fixedTime > timeObstacle + 5) &&*/ (currentVelocity.y < 0.1))
            {
                obstacle = false;
                Debug.Log("me voltio");
                myTransform.rotation = Quaternion.Euler(myTransform.rotation.x, myTransform.rotation.y+80f, myTransform.rotation.z);
            }
        }
        
    }

    public void AssignFlock(Flock flock)
    {
        assignedFlock = flock;
    }

    public void InitializeSpeed(float speed)
    {
        this.speed = speed;
    }

    public void MoveUnit()
    {
        FindNeighbours();
        CalculateSpeed();

        var cohesionVector = CalculateCohesionVector() * assignedFlock.cohesionWeight;
        var avoidanceVector = CalculateAvoidanceVector() * assignedFlock.avoidanceWeight;
        var aligementVector = CalculateAligementVector() * assignedFlock.aligementWeight;
        var boundsVector = CalculateBoundsVector() * assignedFlock.boundsWeight;
        var obstacleVector = CalculateObstacleVector() * assignedFlock.obstacleWeight;

        var moveVector = cohesionVector + avoidanceVector + aligementVector + boundsVector + obstacleVector;
        moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);
        moveVector = moveVector.normalized * speed;
        if (moveVector == Vector3.zero)
            moveVector = transform.forward;

        myTransform.forward = moveVector;
        myTransform.position += moveVector * Time.deltaTime;
    }



    private void FindNeighbours()
    {
        cohesionNeighbours.Clear();
        avoidanceNeighbours.Clear();
        aligementNeighbours.Clear();
        var allUnits = assignedFlock.allAgents;
        for (int i = 0; i < allUnits.Length; i++)
        {
            var currentAgent = allUnits[i];

            if (currentAgent != this)
            {
                float currentNeighbourDistanceSqr = Vector3.SqrMagnitude(currentAgent.myTransform.position - myTransform.position);

                if (currentNeighbourDistanceSqr <= assignedFlock.CohesionDistance * assignedFlock.CohesionDistance)
                {
                    cohesionNeighbours.Add(currentAgent);
                }
                if (currentNeighbourDistanceSqr <= assignedFlock.AvoidanceDistance * assignedFlock.AvoidanceDistance)
                {
                    avoidanceNeighbours.Add(currentAgent);
                }
                if (currentNeighbourDistanceSqr <= assignedFlock.AligementDistance * assignedFlock.AligementDistance)
                {
                    aligementNeighbours.Add(currentAgent);
                }
            }
        }
    }

    private void CalculateSpeed()
    {
        if (cohesionNeighbours.Count == 0)
        {
            return;
        }
            
        speed = 0;

        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            speed += cohesionNeighbours[i].speed;
        }

        speed /= cohesionNeighbours.Count; //Se normaliza la velocidad
        speed = Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed); //Comprueba que la velocidad esté dentro dle rango
    }

    private Vector3 CalculateCohesionVector()
    {
        var cohesionVector = Vector3.zero;
        if (cohesionNeighbours.Count == 0)
            return Vector3.zero;
        int neighboursInFOV = 0;
        for (int i = 0; i < cohesionNeighbours.Count; i++)
        {
            if (IsInFOV(cohesionNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                cohesionVector += cohesionNeighbours[i].myTransform.position;
            }
        }

        cohesionVector /= neighboursInFOV;
        cohesionVector -= myTransform.position;
        cohesionVector = cohesionVector.normalized;
        return new Vector3(cohesionVector.x, 0, cohesionVector.z);
    }

    private Vector3 CalculateAligementVector()
    {
        var aligementVector = myTransform.forward;
        if (aligementNeighbours.Count == 0)
            return myTransform.forward;
        int neighboursInFOV = 0;
        for (int i = 0; i < aligementNeighbours.Count; i++)
        {
            if (IsInFOV(aligementNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                aligementVector += aligementNeighbours[i].myTransform.forward;
            }
        }

        aligementVector /= neighboursInFOV;
        aligementVector = aligementVector.normalized;
        return new Vector3(aligementVector.x, 0, aligementVector.z);
    }

    private Vector3 CalculateAvoidanceVector()
    {
        var avoidanceVector = Vector3.zero;
        if (aligementNeighbours.Count == 0)
            return Vector3.zero;
        int neighboursInFOV = 0;
        for (int i = 0; i < avoidanceNeighbours.Count; i++)
        {
            if (IsInFOV(avoidanceNeighbours[i].myTransform.position))
            {
                neighboursInFOV++;
                avoidanceVector += (myTransform.position - avoidanceNeighbours[i].myTransform.position);
            }
        }

        avoidanceVector /= neighboursInFOV;
        avoidanceVector = avoidanceVector.normalized;
        return new Vector3(avoidanceVector.x, 0, avoidanceVector.z);
    }

    //Si un agente está fuera el radio de acción, lo lleva dentro
    private Vector3 CalculateBoundsVector() 
    {
        var offsetToCenter = assignedFlock.transform.position - myTransform.position;
        bool isNearCenter = (offsetToCenter.magnitude >= assignedFlock.BoundsDistance * 0.9f);
        return isNearCenter ? offsetToCenter.normalized : Vector3.zero;
    }

    private Vector3 CalculateObstacleVector()
    {
        var obstacleVector = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.ObstacleDistance, obstacleMask))
        {
            obstacleVector = FindBestDirectionToAvoidObstacle();
            timeObstacle = Time.fixedTime;
            obstacle = true;
            positionObstacle = myTransform.position;
        }
        else
        {
            currentObstacleAvoidanceVector = Vector3.zero;
        }
        return new Vector3(obstacleVector.x, 0, obstacleVector.z);
    }

    private Vector3 FindBestDirectionToAvoidObstacle()
    {
        if (currentObstacleAvoidanceVector != Vector3.zero)
        {
            RaycastHit hit;
            if (!Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.ObstacleDistance, obstacleMask))
            {
                return currentObstacleAvoidanceVector;
            }
        }

        float maxDistance = int.MinValue;
        var selectedDirection = Vector3.zero;

        for (int i = 0; i < directionsToCheckWhenAvoidingObstacles.Length; i++)
        {

            RaycastHit hit;
            var currentDirection = myTransform.TransformDirection(directionsToCheckWhenAvoidingObstacles[i].normalized);
            if (Physics.Raycast(myTransform.position, currentDirection, out hit, assignedFlock.ObstacleDistance, obstacleMask))
            {

                float currentDistance = (hit.point - myTransform.position).sqrMagnitude;
                if (currentDistance > maxDistance) //Busca el punto más lejano para ir hacia él
                {
                    maxDistance = currentDistance;
                    selectedDirection = currentDirection;
                }
            }
            else
            {
                selectedDirection = currentDirection;
                currentObstacleAvoidanceVector = currentDirection.normalized;
                return selectedDirection.normalized;
            }

            
        }
    
        return selectedDirection.normalized;
    }

    //Comprueba si el agente vecino está en el ángulo de visión del agente
    private bool IsInFOV(Vector3 position)
    {
        return Vector3.Angle(myTransform.forward, position - myTransform.position) <= FOVAngle;
    }
}
