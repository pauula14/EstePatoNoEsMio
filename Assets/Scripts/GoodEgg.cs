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

    // Update is called once per frame
    void Update()
    {
        if ((Time.fixedTime > timeToGen) && (activeInstantiateDucks))
        {
            InstantiateDucks();
        }
    }

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
