using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{

    public int numDucks;
    public int time;
    private Vector3 position;
    private bool activeInstantiateDucks = true;
    [SerializeField] public Flock flockScript;


    void Start()
    {
        numDucks = Random.Range(1, 4);
        time = Random.Range(0, 14);
        position = transform.position;
    }


    void Update()
    {
        if ((Time.fixedTime > time) && (activeInstantiateDucks))
        {         
            InstantiateDucks();
        }
    }

    void InstantiateDucks()
    {
        if (activeInstantiateDucks)
        {
            //Debug.Log("quiero generar");
            activeInstantiateDucks = false;
            flockScript.GenerateNewAgents(numDucks, position);
        }
    }
    
}
