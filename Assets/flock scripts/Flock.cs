using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    [Header("Spawn Setup")]
    [SerializeField] private FlockAgent flockUnitPrefab;
    [SerializeField] private int flockSize;
    //[SerializeField] private Vector3 spawnBounds;
    private Vector3 spawnBounds = new Vector3(50f, 0, 50f);

    [Header("Speed Setup")]
    [Range(0, 10)]
    [SerializeField] private float _minSpeed;
    public float minSpeed { get { return _minSpeed; } }
    [Range(0, 10)]
    [SerializeField] private float _maxSpeed;
    public float maxSpeed { get { return _maxSpeed; } }


    [Header("Detection Distances")] //distancias a las que se detectan entre ellos en los distintos comportamientos

    [Range(0, 10)]
    [SerializeField] private float _cohesionDistance;
    public float CohesionDistance { get { return _cohesionDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidanceDistance; //Cuanto mayor, más lejos estarán uno de otros
    public float AvoidanceDistance { get { return _avoidanceDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _aligementDistance; //Cuanto mayor, más formación "ejército" será
    public float AligementDistance { get { return _aligementDistance; } }

    [Range(0, 10)]
    [SerializeField] private float _obstacleDistance;
    public float ObstacleDistance { get { return _obstacleDistance; } }

    [Range(0, 100)]
    [SerializeField] private float _boundsDistance; //Distancia al centro - radio en el que se moverán
    public float BoundsDistance { get { return _boundsDistance; } }


    [Header("Behaviour Weights")] //"peso" de cada comortamiento, a mayor peso, mayor prioridad

    [Range(0, 10)]
    [SerializeField] private float _cohesionWeight;
    public float cohesionWeight { get { return _cohesionWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _avoidanceWeight; //Cuanto mayor más lejos iran entre sí
    public float avoidanceWeight { get { return _avoidanceWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _aligementWeight; //Cuanto mayor menos habrá por horda
    public float aligementWeight { get { return _aligementWeight; } }

    [Range(0, 10)]
    [SerializeField] private float _boundsWeight;
    public float boundsWeight { get { return _boundsWeight; } }

    [Range(0, 100)]
    [SerializeField] private float _obstacleWeight;
    public float obstacleWeight { get { return _obstacleWeight; } }

    public FlockAgent[] allAgents { get; set; }
    public FlockAgent[] newAgents { get; set; }
    public bool generatedNewAgents = false;


    private void Start()
    {
        GenerateAgents(); //Genera los agentes del comoprtamiento por defecto
    }

    private void Update()
    {
        for (int i = 0; i < allAgents.Length; i++)
        {
            allAgents[i].MoveUnit();
        }

        if (generatedNewAgents)
        {
            for (int i = 0; i < CountNewAgents(); i++)
            {
                newAgents[i].MoveUnit();
            }
        }
        
    }

    private void GenerateAgents()
    {
        allAgents = new FlockAgent[flockSize];
        for (int i = 0; i < flockSize; i++)
        {
            var randomVector = UnityEngine.Random.insideUnitSphere;
            randomVector = new Vector3(randomVector.x * spawnBounds.x, randomVector.y * spawnBounds.y, randomVector.z * spawnBounds.z);
            var spawnPosition = new Vector3(transform.position.x + randomVector.x, 1, transform.position.z + randomVector.z);
            var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

            allAgents[i] = Instantiate(flockUnitPrefab, spawnPosition, rotation);
            allAgents[i].AssignFlock(this);
            allAgents[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
        }
    }

    public void GenerateNewAgents(int number, Vector3 position)
    {

        if (!generatedNewAgents)
        {
            newAgents = new FlockAgent[100];
        }

        //Debug.Log("Hola");
        //newAgents = new FlockAgent[number];
        var nuevosAgentes = CountNewAgents();
        generatedNewAgents = true;

        Debug.Log("numero Agentes: " + nuevosAgentes);
        Debug.Log("nuevos Agentes: " + number);
        //Debug.Log("Length: " + newAgents.Length);
        //var actualSize = newAgents.Length;
        //newAgents.Add(new FlockAgent[number]);
        //Debug.Log("generando");

                 
         for (int i = nuevosAgentes; i < nuevosAgentes+number; i++)
         {
             //Debug.Log("nuevo Agente: " + i);

             var randomPosition = Random.Range(0, 3);
             var spawnPosition = new Vector3(position.x + randomPosition, 1, position.z + randomPosition);
             var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

            //FlockAgent newAgent = new FlockAgent();
            newAgents[i] = Instantiate(flockUnitPrefab, spawnPosition, rotation);
            newAgents[i].AssignFlock(this);
            newAgents[i].InitializeSpeed(UnityEngine.Random.Range(minSpeed, maxSpeed));
            
            //agentitos.Add(newAgent);

            //newAgents.Add(newAgent);

            Debug.Log("nuevo Agente: " + i);
        }

         
    }

    public int CountNewAgents()
    {
        var numberNewAgents = 0;

        if (generatedNewAgents)
        {
            for (int i = 0; i < newAgents.Length; i++)
            {
                if (newAgents[i] != null)
                {
                    numberNewAgents++;
                }
            }
        }
        

        return numberNewAgents;
    }
}
