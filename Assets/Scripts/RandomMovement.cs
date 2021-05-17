using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RandomMovement : MonoBehaviour
{

    [Header("AIsettings")]
    public Vector3 Positions = new Vector3(0, 0, 0);
    public float randomXpos = 0;
    public float randomZpos = 0;
    public NavMeshAgent cuteDuck;

    [Header("LiveSettings")]
    public int LivesMin = 100;
    public int LivesMax = 120;
    public int damage;
    public bool dead;


    public bool killed = false;
    //public GameObject radgoll;


    void Start()
    {
        SetTarget();

        
    }


    void Update()
    {
        Positions = new Vector3(randomXpos, transform.position.y ,randomZpos);

        if (dead == false) //poner !dead
        {
            //gameObject.GetComponent<Animation>().Play("walk"); //Usar para la caminacion de los patos
            cuteDuck.SetDestination(Positions);

            if (transform.position == Positions)
            {
                SetTarget();
            }
        }
        else if (dead == true && killed == false)
        {
            killed = true;
            //Instantiate(radgoll, transform.position, transform.rotation); //USAR ESTO CADA X TIEMPO EN POSICIONES X Y Z RANDOM CON UN NUM RANDOM DE PATOS DENTRO (PARA ELLOS TB HABRA QUE USAR INSTANTIATE, LOS PATOS CON LA MISMA POS PERO UN POQUITO AL LADO DEL HUEVO NO JUSTO ENCIMA X-ALGO Y Z-ALGO)
        }

        if (LivesMin <= 0)
        {
            killed = true;
            dead = true;
        }
    }

    void SetTarget()
    {
        randomXpos = Random.Range(50, -40);
        randomZpos = Random.Range(50, -40);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "bullet") //Poner a la mama y a las hordas (ver qué pasaría)
        {
            LivesMin -= damage;
        }
    }
}
