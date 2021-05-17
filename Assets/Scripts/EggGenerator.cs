using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggGenerator : MonoBehaviour
{

    public Egg eggPrefab;
    public Vector3 position;
    private bool generateEgg;
    private float [] xPositions = { -36.5f, 43, 41.2f, -36.8f };
    private float[] yPositions = { 39.1f, 38.8f, -41.3f, -39.8f };

    void Start()
    {
        
    }


    void Update()
    {
        if (generateEgg)
        {
            GenerateEgg();
        }
    }

    void GenerateEgg()
    {
        var randomPos = Random.Range(0, 3);
        var rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);

        position = new Vector3(xPositions[randomPos], 0.98f, yPositions[randomPos]); //situia el huevo en una de las esquinas
        Instantiate(eggPrefab, position, rotation);

        generateEgg = false;
    }
}
