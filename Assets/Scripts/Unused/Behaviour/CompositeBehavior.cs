using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Flocking/Behavior/Composite")]

public class CompositeBehavior : FlockingBehaviour
{

    public FlockingBehaviour[] behaviors;
    public float[] weights;

    public override Vector3 CalculateMove(FlockingAgent agent, List<Transform> context, Flocking flocking)
    {
        //ghandle data missmathc
        if (weights.Length != behaviors.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero;
        }

        //Configurar movimiento
        Vector3 move = Vector3.zero;

        //iterar los comportamientos
        for (int i = 0; i < behaviors.Length; i++)
        {
            Vector3 partialMove = behaviors[i].CalculateMove(agent, context, flocking) * weights[i];

            if (partialMove != Vector3.zero)
            {
                if (partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= weights[i];
                }

                move += partialMove;
            }
        }

        return move;

    }
}
