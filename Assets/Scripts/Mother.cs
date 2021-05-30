using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mother : MonoBehaviour
{

    public int lives = 1;



    void Update()
    {
        if (lives == 0)
        {
            Debug.Log("Game Over");
        }
    }

    public void IncrementLives ()
    {
        lives++;
    }

    public void DecrementLives()
    {
        lives--;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "FlockAgent")
        {
            lives--;

            if (lives == 0)
            {
                Debug.Log("Game Over");
            }
        }
    }
}
