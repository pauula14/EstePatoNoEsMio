using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FlockingBehaviour : ScriptableObject
{

    public abstract Vector3 CalculateMove(FlockingAgent agent, List<Transform> context, Flocking flocking);

}
