using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public const float speed = -8.0f;
    private const int damagePoints = 10;

    private Rigidbody2D rb2;
    private Vector2 screenBounds;

    // Start is called before the first frame update
    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        //rb2.velocity = new Vector2(0, speed);
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    // Update is called once per frame
    void Update()
    {
        // Delete bullet if outside screen boundries
        /*
        if (transform.position.x  > screenBounds.x * -2 || transform.position.y > screenBounds.y * -2)
        {
            Destroy(gameObject);
        }
        */

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().loseSanity(damagePoints);
        }
        if (!other.CompareTag("Enemy"))
            Destroy(gameObject);
    }
}
