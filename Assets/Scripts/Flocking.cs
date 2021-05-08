using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{

    public FlockingAgent agentPrefab;
    List<FlockingAgent> agents = new List<FlockingAgent>();
    public FlockingBehaviour behaviour;

    [Range(10, 500)]
    public int startingCount = 250; //agentes iniciales
    const float agentDensity = 0.08f;

    [Range(1f, 100f)]
    public float driveFactor = 10f;

    [Range(1f, 100f)]
    public float maxSpeed = 5f;

    [Range(1f, 10f)]
    public float neighborsRadius = 1.5f;

    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    float squareMaxSpeed;
    float squareNeighborRadius;
    float squareAvoidanceRadius;
    public float SquareAvoidanceRadius { get { return squareAvoidanceRadius; } }


    void Start()
    {
        squareMaxSpeed = maxSpeed * maxSpeed;
        squareNeighborRadius = neighborsRadius * neighborsRadius;
        squareAvoidanceRadius = squareNeighborRadius * avoidanceRadiusMultiplier * avoidanceRadiusMultiplier;

        for (int i = 0; i < startingCount; i++){
            FlockingAgent newAgent = Instantiate(
                agentPrefab,
                //Random.insideUnitSphere * startingCount * agentDensity,
                new Vector3 (Random.Range(50f, -50f), 1.4f, Random.Range(50f, -50f)),
                Quaternion.Euler(new Vector3(0f, Vector3.forward.y * Random.Range(0f, 360f), 0f)),
                transform
                );

            newAgent.name = "Agent" + i;
            agents.Add(newAgent);
        }
    }

    void Update()
    {
        
    }
}
