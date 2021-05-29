using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mother : MonoBehaviour
{

    public int lives = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLives ()
    {
        lives++;
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
