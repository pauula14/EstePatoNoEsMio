using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoodEggGenerator : MonoBehaviour
{
    public GoodEgg goodEggPrefab;
    public CuteDuck cuteDuckPrefab;
    
    public Vector3 position;
    public float timeToGenerate;
    private bool generateEgg = false;
    private bool timeGenerated = false;


    void Update()
    {
        if (Time.realtimeSinceStartup > timeToGenerate) //Comprueba si ha pasado el tiempo para crear el nuevo huevo
        {
            generateEgg = true;
            GenerateGoodEgg();
        }

        if (!generateEgg) //Después de generarse el huevo anterior, se calcula el tiempo hasta generar el siguinete huevo
        {
            if (!timeGenerated)
            {
                timeToGenerate = Random.Range(8, 20) + Time.realtimeSinceStartup;
                timeGenerated = true;
            }
        }
    }

    //Este método instancia el Huevo del que nacerán los patitos en una parte del mapa
    void GenerateGoodEgg()
    {
        var xPosition = Random.Range(-36.8f, 43f);
        var yPosition = Random.Range(-41.3f, 39f);
        var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

        position = new Vector3(xPosition, 0.98f, yPosition); //situa el huevo en la posición generada aleatoriamente
        Instantiate(goodEggPrefab, position, rotation);

        generateEgg = false;
        timeGenerated = false;
    }
}
