using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{

    public int numDucks;
    public float time;
    private Vector3 position;
    private bool activeInstantiateDucks = true;
    [SerializeField] public Flock flockScript;
    [SerializeField] public GameObject flock;


    //En el inicio se encuentra el script Flock para generar los patitos, se calcula el número de patos del huevo, el tiempo que van a tardar en 
    //aparecer y la posición del huevo
    void Start()
    {
        flock = GameObject.FindGameObjectWithTag("Flock");
        flockScript = flock.GetComponent<Flock>();
        numDucks = Random.Range(1, 4);
        time = Random.Range(2, 14) + Time.fixedTime;
        position = transform.position;
    }


    void Update() //Si ha pasado el tiempo y ya se ha activado el instanciar patos, se llama al método que instancia los patos
    {
        if ((Time.fixedTime > time) && (activeInstantiateDucks))
        {         
            InstantiateDucks();
        }
    }

    //Este método llama al método de generar nuevos patos del script flock y le pasa el número de patos y suys pocisiones
    void InstantiateDucks()
    {
        if (activeInstantiateDucks)
        {
            activeInstantiateDucks = false;
            flockScript.GenerateNewAgents(numDucks, position);
            Destroy(gameObject);
        }
    }
    
}
