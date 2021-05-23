using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggGenerator : MonoBehaviour
{
    public Egg eggPrefab;
    public Vector3 position;
    public float timeToGenerate;
    private bool generateEgg = false;
    private bool timeGenerated = false;
    private float [] xPositions = { -36.5f, 43, 41.2f, -36.8f };
    private float[] yPositions = { 39.1f, 38.8f, -41.3f, -39.8f };

    void Update()
    {
        if (Time.realtimeSinceStartup > timeToGenerate) //Comprueba si ha pasado el tiempo para crear el nuevo huevo
        {
            generateEgg = true;
            GenerateEgg();
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

    //Este método instancia el Huevo del que nacerán los patitos en una de las cuatro esquinas del mapa
    void GenerateEgg()
    {
        var randomPos = Random.Range(0, 3);
        var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

        position = new Vector3(xPositions[randomPos], 0.98f, yPositions[randomPos]); //situia el huevo en una de las esquinas
        Instantiate(eggPrefab, position, rotation);

        generateEgg = false;
        timeGenerated = false;
    }
}
