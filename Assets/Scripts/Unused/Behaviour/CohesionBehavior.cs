using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flocking/Behavior/Cohesion")]
public class CohesionBehavior : FlockingBehaviour
{
    public override Vector3 CalculateMove(FlockingAgent agent, List<Transform> context, Flocking flocking)
    {
        //Si no hay vecinos, no se devuelve un ajuste de la posición
        if (context.Count == 0)
        {
            return Vector3.zero;
        }

        Vector3 cohesionMove = Vector3.zero;

        foreach (Transform item in context)
        {
            cohesionMove += (Vector3)item.position;
        }

        cohesionMove /= context.Count;

        //crear iffset de la posicion del agente
        cohesionMove -= (Vector3)agent.transform.position;
        return cohesionMove;
    }


}
