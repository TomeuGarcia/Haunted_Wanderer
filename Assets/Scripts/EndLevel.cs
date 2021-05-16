using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    private bool entered = false;
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && entered)
            StartCoroutine(EndLvl());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        entered = other.CompareTag("Player");
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        entered = !other.CompareTag("Player");
    }


    IEnumerator EndLvl()
    {
        for (float f = 1.0f; f >= -0.05f; f += -0.05f)
        {
            yield return new WaitForSeconds(0.05f);
        }
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        // go to main menu
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
