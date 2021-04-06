using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenAppleController : MonoBehaviour
{
    private int healingPoints;

    // Start is called before the first frame update
    void Start()
    {
        healingPoints = 40;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if Collided with player
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().gainSanity(healingPoints);
            Destroy(gameObject);
        }
    }

}