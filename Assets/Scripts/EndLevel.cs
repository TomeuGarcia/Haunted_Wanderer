using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndLevel : MonoBehaviour
{
    private bool entered = false;
    public Image myImage;


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
        for (float f = 0.0f; f <= 1.05f; f += 0.05f)
        {
            Color c = myImage.color;
            c.a = f;
            myImage.color = c;
            yield return new WaitForSeconds(0.05f);
        }
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        // go to main menu
        if (SceneManager.GetActiveScene().buildIndex < 2)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene(0);
    }
}
