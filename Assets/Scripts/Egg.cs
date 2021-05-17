using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{

    public int numDucks;
    private Vector3 position;
    //private bool activeInstantiateDucks = true;
    [SerializeField] private Flock flockScript;


    public FlockAgent[] newAgents { get; set; }

    void Start()
    {
        Debug.Log("quiero generar");
        numDucks = Random.Range(1, 4);
        position = transform.position;

        flockScript.GenerateNewAgents(numDucks, position);
    }


    void Update()
    {
        
    }

    
}
