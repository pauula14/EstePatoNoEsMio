using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CuteDuck : MonoBehaviour
{

    [SerializeField]
    private Mother motherScript; //Para cada vez que se una un patito, aumentar las vidas de la madrre necesitamos acceso a su script

    #region Variables meta 
        //para que cada pato se tenga a sí mismo como su propio navmeshagent
        public NavMeshAgent navMeshAgent;
        //Este game object es la meta que perseguirá cada patito bueno
        public GameObject goalDestination;

        private bool IsGoalDefined = false;
    #endregion

    #region Variables Movimiento Random - Pathfinding
        [Header("AIsettings")]
        public Vector3 nextPosition = new Vector3(0, 0, 0);
        public float randomXpos = 0;
        public float randomZpos = 0;
    #endregion

    #region Campo de visión - FOV
        public float viewRadius;

        [Range(0, 160)]
        public float viewAngle;

        public float meshResolution; //Va a buscar los obtsaculos, habra que hacer uno de la mama
        public MeshFilter viewMeshFilter;
        Mesh viewMesh;

        public LayerMask obstacleMask;
        public LayerMask motherMask;

        public List<Transform> visibleObstacles = new List<Transform>();
    #endregion

    void Start()
    {
        SetTarget();
        viewAngle = 40;
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    // Update is called once per frame
    void Update()
    {
        viewAngle += 0.01f;
        DrawFieldOfView();

        if (IsGoalDefined) //Si ya ha encontrado a la madre, le sigue
        {
            navMeshAgent.destination = goalDestination.transform.position;
        }
        else //Realiza movimiento pathfinding
        {
            nextPosition = new Vector3(randomXpos, transform.position.y, randomZpos);
            navMeshAgent.SetDestination(nextPosition);

            if (transform.position == nextPosition)
                {
                    SetTarget();
                }
        }
    }

    void setGoalToMother()
    {
        if (!IsGoalDefined)
        {
            motherScript.SetLives();
        }

        goalDestination = GameObject.FindGameObjectWithTag("Mother"); //Player es la madre, que es el avatar del jugador
        IsGoalDefined = true;
    }

    void SetTarget() //Calcula una posición aletaoria dentro del terreno
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

            for (int i = 0; i <= stepCount; i++)
            {
                float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
                //Debug.DrawLine(transform.position, transform.position + dirFromAngle(angle, true) * viewRadius, Color.red);
                ViewCastInfo newViewCast = ViewCast(angle, 0);
                viewPoints.Add(newViewCast.point);

                if (newViewCast.hit)
                {
                    if (newViewCast.type == 1) //Si ha entrado la madre en el campo de visión
                    {
                        Debug.Log("mother");
                        setGoalToMother();
                    }
                    else if (newViewCast.type == 2) //Si el objeto del campo de visión es un obstácilo
                    {
                        Debug.Log("obstaculo");
                        //SetTarget();
                    }
                }
            }

            int vertexCount = viewPoints.Count + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            int[] triangles = new int[(vertexCount - 2) * 3];

            vertices[0] = Vector3.zero;

            for (int i = 0; i < vertexCount - 1; i++)
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
