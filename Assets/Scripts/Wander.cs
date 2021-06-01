using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour
{
    private float moveSpeed = 40f;
    private float rotSpeed = 200f;

    private bool isWandering = false;
    private bool isRotatingLeft = false;
    private bool isRotatingRight = false;
    private bool isWalking = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isWandering)
        {
            StartCoroutine(Wandering());
        }

        if (isRotatingRight)
        {
            transform.Rotate(transform.up * Time.deltaTime * rotSpeed);
        }

        if (isRotatingLeft)
        {
            transform.Rotate(transform.up * Time.deltaTime * (-rotSpeed));
        }

        if (isWalking)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    //Este método se va a reproducir completo cada vez que se llame en el update. 
    /* Consiste en un comoprtamiento en el que el agente primero espera unos segundos aleatorios, pueden ser 1, 2 o 3, para que no sea excesivo y no saque del juego,
     * una vez han pasado esos segundos, comienza a andar durante el tiempo que el random del intérvalo diga, es un número muy variable, puede ser bastante grande, de
     * forma que pueda cruzar el escenario, o muy pequeño, de esta forma se simula el movimiento distinto para cada agente. Una vez ha terminado de andar, el agente
     * esperará unos segundos hasta girar hacia derecha o izquierda, según salga el random. Una vez haya girado, el enumerator termina, se pone a false y el update lo vuelve a llamar.
     * */
    IEnumerator Wandering()
    {
        int rotTime = Random.Range(1, 3);
        int rotateWait = Random.Range(0, 2);
        int rotateLoR = Random.Range(0, 2);
        int walkWait = Random.Range(0, 2);
        int walkTime = Random.Range(1, 3);

        isWandering = true;

        yield return new WaitForSeconds(rotateWait);
        if (rotateLoR == 0)
        {
            isRotatingRight = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingRight = false;
        }
        else
        {
            isRotatingLeft = true;
            yield return new WaitForSeconds(rotTime);
            isRotatingLeft = false;
        }
        yield return new WaitForSeconds(walkWait); //Espera los segundos que devuelva el random de walkwait
        isWalking = true;
        yield return new WaitForSeconds(walkTime);
        isWalking = false;
        
        isWandering = false;
    }
}
