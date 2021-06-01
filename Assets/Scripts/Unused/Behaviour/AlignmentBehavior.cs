using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flocking/Behavior/Aligment")]
public class AlignmentBehavior : FlockingBehaviour
{
    public override Vector3 CalculateMove(FlockingAgent agent, List<Transform> context, Flocking flocking)
    {
        //Si no hay vecinos, se mantiene la alineación
        if (context.Count == 0)
        {
            return agent.transform.forward; //El usa up
        }

        Vector3 alignmentMove = Vector3.zero;

        foreach (Transform item in context)
        {
            alignmentMove += (Vector3)item.transform.forward; //el up
        }

        alignmentMove /= context.Count;

        return alignmentMove;
    }
}
