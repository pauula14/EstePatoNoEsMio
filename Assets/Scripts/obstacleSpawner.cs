using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefab;
    
    private void Start() {
            int rand = Random.Range(0, obstaclePrefab.Length);
            int[] randomRot = {0,90,180,270};
            Instantiate(obstaclePrefab[rand], new Vector3(transform.position.x, 0, transform.position.z), Quaternion.Euler(0,randomRot[Random.Range(0,randomRot.Length)],0));        
    }



}
