using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : MonoBehaviour
{
    // Variables
    public bool isSpontaneous = false;

    private float moveSpeed = 12.0f;
    private bool isMoving;
    private Vector2 notActivePosition;
    public Vector2 activePosition;
    private Vector2 positionToMove;


    // Enemy boolean flags (tags)
    [Header("Platform Spawn Tags")]
    [SerializeField] public bool spawnsHighSanity;
    [SerializeField] public bool spawnsMediumSanity;
    [SerializeField] public bool spawnsLowSanity;


    // Components
    private SpriteRenderer sr;


    private void Start()
    {
        isMoving = false;
        if (isSpontaneous)
        {
            sr = GetComponent<SpriteRenderer>();
            Color c = sr.material.color;
            // a -> alpha (transparency: 0=transparent , 1=visible)
            c.a = 1.0f;
            sr.material.color = c;
        }
        else
        {
            notActivePosition = transform.position;
        }
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            movePlatform(positionToMove);
            if ((Vector2)transform.position == positionToMove)
            {
                isMoving = false;
                if (positionToMove == notActivePosition)
                    gameObject.SetActive(false);
            }
        }
    }


    // OTHER methods
    public void setActiveState(bool isActive)
    {
        if (isActive)
        {
            gameObject.SetActive(true);
            if (isSpontaneous)
            {
                startFadingIn();
            }
            else
            {
                isMoving = true;
                positionToMove = activePosition;
            }
        }
        else
        {
            if (isSpontaneous)
            {
                startFadingOut();
                gameObject.SetActive(false);
            }
            else
            {
                isMoving = true;
                positionToMove = notActivePosition;
            }
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
        StartCoroutine(fadeIn());
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
    }
    void startFadingOut()
    {
        StartCoroutine(fadeOut());
    }


    // Move platforms (methods called when isSpontaneous == false)
    private void movePlatform(Vector2 positionToMove)
    {
        //positionToMove = new Vector2(25.25f, -1.5f);
        //transform.position = Vector2.MoveTowards(transform.position, positionToMove, moveSpeed * Time.deltaTime);
        //transform.Translate((positionToMove - (Vector2)transform.position) * moveSpeed * Time.deltaTime);
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(25f, 10f), moveSpeed * Time.deltaTime);
    }


}
