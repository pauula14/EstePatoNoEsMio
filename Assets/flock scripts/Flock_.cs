using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock_ : MonoBehaviour
{
   /* [Header ("Spawn Setup")]
    [SerializeField] private FlockBehavior flockAgentPrefab;
    [SerializeField] private int flockSize;
    [SerializeField] private Vector3 spawnBounds;

    [Header("Speed Setup")]
    [Range(0, 10)]
    [SerializeField] private float minSpeed;
    [Range(0, 10)]
    [SerializeField] private float maxSpeed;

    [Header("Detection distances")]
    [Range(0, 10)]
    [SerializeField] private float cohesionDistance;

    public float CohesionDistance { get { return cohesionDistance; } }
    public float MaxSpeed { get { return maxSpeed; } }
    public float MinSpeed { get { return minSpeed; } }


    public FlockBehavior[] agents { get; set; }

    void Start()
    {
        GenerateUnits();
    }

    void Update()
    {
        for (int i = 0; i < agents.Length; i++)
        {
            agents[i].MoveBehavior();
        }
    }

    private void GenerateUnits()
    {

        agents = new FlockBehavior[flockSize];

        for (int i = 0; i < flockSize; i++)
        {
            var randomVector = UnityEngine.Random.insideUnitSphere;
            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            //var randomVector = new Vector3(Random.Range(50f, -50f), 1.4f, Random.Range(50f, -50f));
            //var spawnPosition = transform.position + randomVector;
            var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            agents[i] = Instantiate(flockAgentPrefab, randomVector, rotation);
            agents[i].AssignFlock(this);
            agents[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
        }
    }*/
}
