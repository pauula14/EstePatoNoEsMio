using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FlockAgent : MonoBehaviour
{
    [SerializeField] private float FOVAngle; //ángulo de visión del agente
    [SerializeField] private float smoothDamp;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask motherMask;
    [SerializeField] private Vector3[] directionsToCheckWhenAvoidingObstacles; //Direcciones que comprueba cuando se choca probablmente no haga falta

    //Listas con los vecinos de cada comportamiento
    private List<FlockAgent> cohesionNeighbours = new List<FlockAgent>();
    private List<FlockAgent> avoidanceNeighbours = new List<FlockAgent>();
    private List<FlockAgent> aligementNeighbours = new List<FlockAgent>();

    private float timeFollowingMother;
    private Flock assignedFlock; //Script de flock
    public Vector3 currentVelocity; //Velocidad actual
    private Vector3 currentObstacleAvoidanceVector; //Vector para esquivar al obstáculo
    public float speed; //Velocidad que se aplicará

    [SerializeField]  private NavMeshAgent navMesh;
    private bool motherInFOV = false;
    private GameObject mother;
    private bool obstacle = false; //Si ha chocado contra un obstáculo

    public Transform myTransform { get; set; }

    private void Awake()
    {
        myTransform = transform; //Ahorra complejidad computacional
    }

    private void Update()
    {
        if (obstacle) //Si ha chocado con un obstáculo y está parado o va muy lento, gira para evitar quedarse atascado
        {
            if ((currentVelocity.y < 0.5))
            {
                obstacle = false;
                Debug.Log("me voltio");
                myTransform.rotation = Quaternion.Euler(myTransform.rotation.x, myTransform.rotation.y+80f, myTransform.rotation.z);
            }
        }

        //Comprueba que si no han pasado cinco segundos desde que la madre ha entrado en el ángulo de visión continúe siguiéndola el agente
        if ((Time.realtimeSinceStartup < timeFollowingMother))
        {
            navMesh.SetDestination(mother.transform.position);

        }

    }

    private void Start()
    {
        mother = GameObject.FindGameObjectWithTag("Mother");
    }

    public void AssignFlock(Flock flock) //Asigna el script
    {
        assignedFlock = flock;
    }

    public void InitializeSpeed(float speed) //Inicializa la velocidad
    {
        this.speed = speed; 
    }

    public void MoveUnit() //Mueve al agente con los 3 comportamientos, añadiendo la distancia al centro y los obstáculos
    {
        FindNeighbours();
        CalculateSpeed();
        DetectMother();

        var cohesionVector = CalculateCohesionVector() * assignedFlock.cohesionWeight;
        var avoidanceVector = CalculateAvoidanceVector() * assignedFlock.avoidanceWeight;
        var aligementVector = CalculateAligementVector() * assignedFlock.aligementWeight;
        var boundsVector = CalculateBoundsVector() * assignedFlock.boundsWeight;
        var obstacleVector = CalculateObstacleVector() * assignedFlock.obstacleWeight;
        
        
        if (motherInFOV) //en casio de que la madre esté dentro del ángulo de visión la seguirá
        {
            navMesh.SetDestination(mother.transform.position);

        }
        else //En caso de que no, se realiza el moviumiento flocking
        {
            var moveVector = cohesionVector + avoidanceVector + aligementVector + boundsVector + obstacleVector;


            moveVector = Vector3.SmoothDamp(myTransform.forward, moveVector, ref currentVelocity, smoothDamp);
            moveVector = moveVector.normalized * speed;
            if (moveVector == Vector3.zero)
            {
                moveVector = transform.forward;
            }

            myTransform.forward = moveVector;
            myTransform.position += moveVector * Time.deltaTime;
        }

        
        
    }


    //Busca los vecinos del agente para cada uno de los comportamientos en función de los parámetros establecidos en el agente y los asigna
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

    //Le da velocidad al agente, se adaptan a la velocidad de sus vecinos
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

        if (cohesionNeighbours.Count < 5)
        {
            Debug.Log("menos de cinco");
            speed = Mathf.Clamp(speed, assignedFlock.minSpeed, assignedFlock.maxSpeed-1); //Comprueba que la velocidad esté dentro dle rango
        }
        else if ((cohesionNeighbours.Count > 5) && (cohesionNeighbours.Count < 10))
        {
            Debug.Log("mas de cinco");
            speed = assignedFlock.maxSpeed+1;
        }
        else
        {
            Debug.Log("mas de diez");
            speed = assignedFlock.maxSpeed + 2;
        }
        
    }

    //Calcula el vector de cohesión que se va a utilizar para generar el movimiento del agente
    /* Funciona de forma que lo primero que comprueba es si tiene algún vecino en el comportamiento de cohesión, si es que no no calcula un nuevo vector,
    * sino que devuelve el vector de la posición hacia la que está mirando (el de delante) y sale del método. Sino, pone el número de vecinos en el campo
    * de visión a 0 y entra en el bucle a contar los vecinos que tienen de cohesión y suma uno por cada uno que es vecino y está en su campo de visón
    * y suma el vector de la posición hacia la que miora ese vecino. Una vez se calculan todos, se tiene un vector con la suma de las posiciones hacia
    * las que miran todos los vecinos de cohesión en su campo de visión. Este vector se normaliza, generando el vector hacia el que va a mirar y
    * dirigirse el agente*/
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

    //Calcula el vector de alineación que se va a utilizar para generar el movimiento del agente
    /* Funciona de forma que lo primero que comprueba es si tiene algún vecino en el comportamiento de alineación, si es que no no calcula un nuevo vector,
     * sino que devuelve el vector de la posición hacia la que está mirando (el de delante) y sale del método. Sino, pone el número de vecinos en el campo
     * de visión a 0 y entra en el bucle a contar los vecinos que tienende alineación y suma uno por cada uno que es vecino y está en su campo de visón
     * y suma el vector de la posición hacia la que miora ese vecino. Una vez se calculan todos, se tiene un vector con la suma de las posiciones hacia
     * las que miran todos los vecinos de alineación en su campo de visión. Este vector se normaliza, generando el vector hacia el que va a mirar y
     * dirigirse el agente*/
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

    //Calcula el vector de esquivación que se va a utilizar para generar el movimiento del agente
    /* Funciona de forma que lo primero que comprueba es si tiene algún vecino en el comportamiento de esquivación, si es que no no calcula un nuevo vector,
    * sino que devuelve el vector de la posición hacia la que está mirando (el de delante) y sale del método. Sino, pone el número de vecinos en el campo
    * de visión a 0 y entra en el bucle a contar los vecinos que tienen de esquivación y suma uno por cada uno que es vecino y está en su campo de visón
    * y suma el vector de la posición hacia la que miora ese vecino. Una vez se calculan todos, se tiene un vector con la suma de las posiciones hacia
    * las que miran todos los vecinos de esquivación en su campo de visión. Este vector se normaliza, generando el vector hacia el que va a mirar y
    * dirigirse el agente*/
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

    //Si un agente está fuera el radio de acción, lo lleva dentro. el radio está definido en el proyecto
    private Vector3 CalculateBoundsVector() 
    {
        var offsetToCenter = assignedFlock.transform.position - myTransform.position;
        bool isNearCenter = (offsetToCenter.magnitude >= assignedFlock.BoundsDistance * 0.9f);
        return isNearCenter ? offsetToCenter.normalized : Vector3.zero;
    }

    //Calcula el vector que va a utilizar el agente para esquivar un obstáculo
    /*Llama al método que devolverá el vector que más lejos llega desde su posición actual, se repetirá hasta que se haya alejado del todo del obstáculo*/
    private Vector3 CalculateObstacleVector()
    {
        var obstacleVector = Vector3.zero;
        RaycastHit hit;
        if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.ObstacleDistance, obstacleMask))
        {
            obstacleVector = FindBestDirectionToAvoidObstacle();
            obstacle = true;
        }
        else
        {
            obstacleVector = Vector3.zero;
        }
        return new Vector3(obstacleVector.x, 0, obstacleVector.z);
    }

    //Comprueba si la madre entra deltro del campo de visión, en caso de que si, se añaden 5 segundos para que aunque salga de su campo de visión, la siga durante esos 
    /*segundos, en caso de que no salga, seguirá siguiéndola*/
    private void DetectMother()
    {
        RaycastHit hit;
        if (Physics.Raycast(myTransform.position, myTransform.forward, out hit, assignedFlock.MotherDistance, motherMask))
        {
            timeFollowingMother = Time.realtimeSinceStartup + 5;
            motherInFOV = true;
        }
        else
        {
            motherInFOV = false;
        }
        
    }

    //Busca, con 8 posibles posiciones la mejor para evitar un obstáculo
    /*Este métioido, de los 8 vectores definidos en el inspector de Unity, selecciona el que llega a un punto más lejando respecto del obstáculo, de forma
     * que el agente se va alejando poco a poco del obstáculo. Este método hace la parte de detectar y devolver el vector con la posición más lejana, es decir, a la
     * que se va a desplazar el agente*/
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

        for (int i = 0; i < directionsToCheckWhenAvoidingObstacles.Length; i++) //Va escogiendo la más lejana al obstáculo de las dadas
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
