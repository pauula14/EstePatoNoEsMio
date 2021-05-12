using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerScript : MonoBehaviour
{
    //para que cada pato se tenga a sí mismo como su propio navmeshagent
    public NavMeshAgent navMeshAgent;

    //Este game object es la meta que perseguirá cada patito malo, por lo que hay que añadir una parte de codigo en la que decida si su meta es una horda de patos o la madre
    public GameObject goalDestination;

    void Start()
    {

        //aquí se añadiria el codigo dinamico para obtener según el tag o nombre la meta (horda o madre)
        //goalDestination = GameObject.FindGameObjectWithTag("Goal"); //en lugar de goal mother y como denominemos las hordas

        //aqui se pone al agente el destino seleccionado
        //navMeshAgent.destination = goalDestination.transform.position; //Esto en el start solo vale si la meta NO se mueve
    }


    void Update()
    {/*
        //aqui se pone al agente el destino seleccionado
        navMeshAgent.destination = goalDestination.transform.position; //Esto en el start vale si la meta se mueve

        //Por si nos interesa: esto hace que el bicho vaya a donde pulsamos

        if (Input.GetMouseButtonDown(0)) //Todo esto con el sistema antiguo de inputs, si lo queremos usar atuyalizar al nuevo sistema
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                navMeshAgent.destination = hit.point;
            }
        }*/

        goalDestination = GameObject.FindGameObjectWithTag("Goal");
        navMeshAgent.destination = goalDestination.transform.position;
    }
}
