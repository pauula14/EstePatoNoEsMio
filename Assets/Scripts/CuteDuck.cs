using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CuteDuck : MonoBehaviour
{

    [SerializeField] private Mother motherScript; //Para cada vez que se una un patito, aumentar las vidas de la madrre necesitamos acceso a su script
    [SerializeField] public GameObject mother;

    #region Variables meta 
    public NavMeshAgent navMeshAgent; //para que cada pato se tenga a sí mismo como su propio navmeshagent
        public GameObject goalDestination; //Este game object es la meta que perseguirá cada patito bueno
        private bool IsMotherDefined = false; //true o false en función de si encuentra a la madre o no
        private bool LakeDetected = false;
    #endregion

    #region Variables Movimiento Random - Pathfinding
        [Header("AIsettings")]
        public Vector3 nextPosition = new Vector3(0, 0, 0); //posiicón a la que se moverá cuando llegue a la meta anterior
        public float randomXpos = 0;
        public float randomZpos = 0;
    #endregion

    #region Campo de visión - FOV
        public float viewRadius;

        [Range(0, 160)]
        public float viewAngle; //Ángulo de visión del pato, va creciento hasta 160

        public float meshResolution; //Va a buscar los obstaculos
        public MeshFilter viewMeshFilter;
        Mesh viewMesh;

        public LayerMask obstacleMask; //La máscara que interpreta como madre
        public LayerMask motherMask; //La máscara que interpreta como obstáculos

        public List<Transform> visibleObstacles = new List<Transform>(); //Almacena las posiciones de los obstáculos
    #endregion

    void Start()
    {
        mother = GameObject.FindGameObjectWithTag("Mother");
        motherScript = mother.GetComponent<Mother>();

        goalDestination = GameObject.FindGameObjectWithTag("Mother"); //Player es la madre, que es el avatar del jugador

        SetTarget(); //Calcula la primera posición meta
        viewAngle = 40; //ángulo inicial de visión
        viewMesh = new Mesh(); //Adjunta el mesh de la visión
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    void Update()
    {
        viewAngle += 0.01f; //Suma en cada iteración 0.01 hasta llegar a 160 del ángulo de visión
        DrawFieldOfView(); //Dibuja el campo de visión

        if (IsMotherDefined) //Si ya ha encontrado a la madre, le sigue
        {
            navMeshAgent.destination = goalDestination.transform.position;
        }
        else //Realiza movimiento pathfinding si no ha encontrado a la madre
        {
            nextPosition = new Vector3(randomXpos, transform.position.y, randomZpos);
            navMeshAgent.SetDestination(nextPosition);

            if (transform.position == nextPosition) //Si ya ha llegado a la posición meta, calcula la nueva
                {
                    SetTarget(); 
                }
        }
    }

    //Pone como meta a la madre, de forma que ahora le seguirá siempre, ya no hará pathfinding
    void setGoalToMother()
    {
        if (!IsMotherDefined)
        {
            motherScript.SetLives();
        }

        LakeDetected = false;
        IsMotherDefined = true;
    }

    //Calcula una posición aletaoria dentro del terreno
    void SetTarget() 
    {
        randomXpos = Random.Range(50, -40);
        randomZpos = Random.Range(50, -40);
    }
    
    #region Campo de Visión del patito

        //Dibuja el campo de visión del patito
        void DrawFieldOfView()
        {
            int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
            float stepAngleSize = viewAngle / stepCount;
            List<Vector3> viewPoints = new List<Vector3>();

            //Hace una cosa u otra en funcion de si en su campo de visión ha entrado un obstáculo o la madre
            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
                ViewCastInfo newViewCast = ViewCast(angle, 0);
                viewPoints.Add(newViewCast.point);

                if (newViewCast.hit)
                {
                    if (newViewCast.type == 1) //Si ha entrado la madre en el campo de visión
                    {
                        Debug.Log("mother");
                        LakeDetected = false;
                        setGoalToMother();
                    }
                    else if (newViewCast.type == 2) //Si el objeto del campo de visión es un obstácilo
                    {
                        Debug.Log("obstaculo");
                    }
                }
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;

            for (int i = 0; i < vertexCount - 1; i++) //Genera el campo de visión
            {
                vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

                if (i < vertexCount - 2)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }
            }

            viewMesh.Clear();
            viewMesh.vertices = vertices;
            viewMesh.triangles = triangles;
            viewMesh.RecalculateNormals();

        }

        //Devuelve la direccion del angulo
        public Vector3 dirFromAngle(float angleDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleDegrees * Mathf.Deg2Rad));
        }

        //Interpreta si el patito ve o no ve a la madre o al obstáculo
        public ViewCastInfo ViewCast(float globalAngle, int typeObstacle) //1 = madre y 2 = obstaculo y 0 nada
        {
            Vector3 dir = dirFromAngle(globalAngle, true);
            RaycastHit hit;

            //Detecta si algún objeto entra en su campo de visión y en función de eso devuelve una información
            if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle, 2); //Datos del obstáculo (el 2 indica obstáculo)
            }
            else if (Physics.Raycast(transform.position, dir, out hit, viewRadius, motherMask)) //Datos de la madre/player (el 1)
            {
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle, 1);
            }
            else
            {
                return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle, 0); //No ve nada
            }
        }

    public void OnTriggerEnter(Collider collision)//Si pasa por un lago, deja de seguir a la madre y esta pierde una vida
    {
        if (collision.gameObject.tag == "Lake")
        {
            Debug.Log("lagho");

                Debug.Log("Girar");
                transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y + 180f, transform.rotation.z);
                SetTarget();
                LakeDetected = true;
                IsMotherDefined = false;         
        }
    }

    //La estructura utilizada para poder interpretar obstaculos y la madre/player
    public struct ViewCastInfo
        {
            public bool hit; //Si hay choque
            public Vector3 point; //El punto en que se sitúa el objeto visto
            public float dist; //Distancia al objeto
            public float angle; //Ándulo de visión
            public int type; //Tipo de objeto que es: obstáculo o madre

            public ViewCastInfo(bool _hit, Vector3 _point, float _dist, float _angle, int type_)
            {
                hit = _hit;
                point = _point;
                dist = _dist;
                angle = _angle;
                type = type_;
            }
        }

    #endregion
}
