using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    // Variables
    private bool appearing;
    private bool disappearing;
    private const float disappearTime = 1f;
    private float disappearTimer = disappearTime; // timer that counts backwards

    private const float appearInc = 0.025f;

    // Platform boolean flags (tags)
    public bool isActive;
    [Header("Platform Exists When")]
    [SerializeField] public bool highSanity = false;
    [SerializeField] public bool mediumSanity = false;
    [SerializeField] public bool lowSanity = false;

    // Components
    public SpriteRenderer sr;


    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Color c = sr.material.color;
        // a -> alpha (transparency: 0=transparent , 1=visible)
        c.a = 1.0f;
        sr.material.color = c;
    }

    private void Update()
    {
        if (disappearing)
        {
            if (disappearTimer >= -appearInc)
            {
                Color c = sr.material.color;
                c.a = disappearTimer;
                sr.material.color = c;
                disappearTimer -= appearInc;
            }
            else
            {
                disappearing = false;
                //disappearTimer = disappearTime;
                //gameObject.SetActive(false);
            }
        }

        if (!disappearing && disappearTimer < 0)
        {
            disappearTimer = disappearTime;
            gameObject.SetActive(false);
        }
    }


    // OTHER methods
    public void setActiveState(bool active)
    {
        if (active)
        {
            gameObject.SetActive(true);
            if (disappearing)
            {
                disappearing = false;
                StopCoroutine(FadeIn());
            }
            
            appearing = true;
            isActive = true;
            StartCoroutine(FadeIn());
        }
        else
        {
            if (appearing)
            {
                appearing = false;
                //StopCoroutine(FadeOut());
            }

            disappearing = true;
            isActive = false;
            //StartCoroutine(FadeOut());
        }
    }


    // Fade platforms (methods called when isSpontaneous == true)
    // Fade in
    IEnumerator FadeIn()
    {
        for (float f = appearInc; f <= 1.0f; f += appearInc)
        {
            Color c = sr.material.color;
            c.a = f;
            sr.material.color = c;
            yield return new WaitForSeconds(0.05f);
        }
        appearing = false;
    }


    // Fade out
    //IEnumerator FadeOut()
    //{
    //    for (float f = 1.0f; f >= -appearInc; f += -appearInc)
    //    {
    //        Color c = sr.material.color;
    //        c.a = f;
    //        sr.material.color = c;
    //        yield return new WaitForSeconds(0.05f);
    //    }
    //    disappearing = false;
    //}






}
