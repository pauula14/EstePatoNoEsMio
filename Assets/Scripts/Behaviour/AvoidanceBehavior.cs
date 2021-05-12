using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flocking/Behavior/Avoidance")]
public class AvoidanceBehavior : FlockingBehaviour
{
    public override Vector3 CalculateMove(FlockingAgent agent, List<Transform> context, Flocking flocking)
    {
        //Si no hay vecinos, se mantiene la alineación
        if (context.Count == 0)
        {
            return agent.transform.forward; //El usa up
        }

        Vector3 avoidanceMove = Vector3.zero;
        int navoid = 0;

        foreach (Transform item in context)
        {
            if (Vector3.SqrMagnitude(item.position - agent.transform.position) < flocking.SquareAvoidanceRadius)
            {
                navoid++;
                avoidanceMove += (Vector3)(agent.transform.position - item.position); //el up
            }
            
        }

        if (navoid > 0)
        {
            avoidanceMove /= navoid;
        }

        return avoidanceMove;
    }
}
