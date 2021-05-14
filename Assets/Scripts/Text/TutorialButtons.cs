using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialButtons : MonoBehaviour
{
    public GameObject key;

    public void Start()
    {
        key.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
     if(other.name == "Player")
        {
            key.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "Player")
        {
            key.SetActive(false);
        }
    }

}
