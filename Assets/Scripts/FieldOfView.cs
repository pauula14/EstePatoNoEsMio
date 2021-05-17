using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfView : MonoBehaviour
{


    public float viewRadius;

    [Range(0, 160)]
    public float viewAngle;

    public float meshResolution; //Va a buscar los obtsaculos, habra que hacer uno de la mama
    public MeshFilter viewMeshFilter;
    Mesh viewMesh;

    public LayerMask obstacleMask;
    public LayerMask motherMask;

    public List<Transform> visibleObstacles = new List<Transform>();

    /*public void FindObjects() //Detecta esferas
    {

        Collider[] obstaclesInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, obstacleMask);

        for (int i = 0; i < obstaclesInViewRadius.Length; i++)
        {

            Transform obstacle = obstaclesInViewRadius[i].transform;
            Vector3 dirToObstacle = (obstacle.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToObstacle) < viewAngle / 2)
            {
                float distToObstacle = Vector3.Distance(transform.position, obstacle.position);

                if (Physics.Raycast(transform.position, dirToObstacle, distToObstacle, obstacleMask))
                {
                    Debug.Log("obstaculo!!");
                    visibleObstacles.Add(obstacle);
                }
            }
        }
    }*/


    //Detecta los obtaculos y pinta la vista, hacer para la mdre y en lugar de para obstaculos usar este para las hordas (detectaria a cada patete de la horda)
    public void DrawFieldOfView()
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
                if (newViewCast.type == 1)
                { 
                    //Aqui detecta los obstaculos
                    Debug.Log("mother");
                }
                else if (newViewCast.type == 2)
                { 
                    //Aqui detecta los obstaculos
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

    //Esto es para interpretar la estructura de los obstaculos (creo que se puede usar el mismo para madre)
    public ViewCastInfo ViewCast(float globalAngle, int typeObstacle) //1 = madre y 2 = obstaculo y 0 nada
    {
        Vector3 dir = dirFromAngle(globalAngle, true);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle, 2);
        }
        else if (Physics.Raycast(transform.position, dir, out hit, viewRadius, motherMask))
        {
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle, 1);
        }
        else
        {
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle, 0);
        }
    }

    //La estructura utilizada para poder interpretar obstaculos -- patos feos
    public struct ViewCastInfo{
        
        public bool hit;
        public Vector3 point;
        public float dist;
        public float angle;
        public int type;

        public ViewCastInfo (bool _hit, Vector3 _point, float _dist, float _angle, int type_)
        {
            hit = _hit;
            point = _point;
            dist = _dist;
            angle = _angle;
            type = type_;
        }
    }


    void Start()
    {
        viewAngle = 40;
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }


    void LateUpdate()
    {
        viewAngle += 0.01f;
        DrawFieldOfView();
    }
}
