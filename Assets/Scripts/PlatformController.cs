using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    // Variables
    //private Vector2 notActivePosition;
    //public Vector2 activePosition;
    //private Vector2 positionToMove;


    // Platform boolean flags (tags)
    public bool isActive;
    [Header("Platform Exists When")]
    [SerializeField] public bool highSanity = false;
    [SerializeField] public bool mediumSanity = false;
    [SerializeField] public bool lowSanity = false;

    // Components
    private SpriteRenderer sr;


    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Color c = sr.material.color;
        // a -> alpha (transparency: 0=transparent , 1=visible)
        c.a = 1.0f;
        sr.material.color = c;
    }


    // OTHER methods
    public void setActiveState(bool active)
    {
        if (active)
        {
            isActive = true;
            gameObject.SetActive(true);
            startFadingIn();
        }
        else
        {
            isActive = false;
            startFadingOut();
            //gameObject.SetActive(false);
        }
    }


    // Fade platforms (methods called when isSpontaneous == true)
    // Fade in
    IEnumerator fadeIn()
    {
        for (float f = 0.05f; f <= 1.0f; f += 0.05f)
        {
            Color c = sr.material.color;
            c.a = f;
            sr.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
    }
    void startFadingIn()
    {
        //StartCoroutine(fadeIn());
        gameObject.SetActive(true);
    }


    // Fade out
    IEnumerator fadeOut()
    {
        for (float f = 1.0f; f >= -0.05f; f += -0.05f)
        {
            Color c = sr.material.color;
            c.a = f;
            sr.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
        gameObject.SetActive(false);
    }

    void startFadingOut()
    {
        //StartCoroutine(fadeOut());
        gameObject.SetActive(false);
    }




}
