using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodEgg : MonoBehaviour
{
    public CuteDuck cuteDuckPref;
    public GoodEggGenerator eggGeneratorScript;
    public GameObject ground;

    public float timeToGen;
    private Vector3 positionEgg;
    private bool activeInstantiateDucks = true;
   

    void Start()
    {
        ground = GameObject.FindGameObjectWithTag("Ground");
        eggGeneratorScript = ground.GetComponent<GoodEggGenerator>();
        cuteDuckPref = eggGeneratorScript.cuteDuckPrefab;

        timeToGen = Random.Range(2, 14) + Time.fixedTime;
        positionEgg = transform.position;
    }


    void Update()
    {
        if ((Time.fixedTime > timeToGen) && (activeInstantiateDucks)) //Si ha pasado el tiempo, se llama a instanciar el pato
        {
            InstantiateDucks();
        }
    }

    //Intsnacia el patito y destruye el huevo del que ha salido. El pato se instancia en el mismo lugar donde ha aparecido el huevo
    void InstantiateDucks()
    {
        if (activeInstantiateDucks)
        {
            activeInstantiateDucks = false;
            var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
            Instantiate(cuteDuckPref, positionEgg, rotation);
            Destroy(gameObject);
        }
    }
}
