using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public const float speed = -8.0f;
    public int damagePoints = 10;

    private Rigidbody2D rb2;
    private Vector2 screenBounds;

    [Header("Audio Elements")]
    [SerializeField] public AudioSource audio;
    [SerializeField] public AudioClip bulletSplash;

    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        rb2 = GetComponent<Rigidbody2D>();
        //rb2.velocity = new Vector2(0, speed);
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy"))
        {
            rb2.bodyType = RigidbodyType2D.Static;
            StartCoroutine(Explode());
        }
    }

    IEnumerator Explode()
    {
        audio.PlayOneShot(bulletSplash, 0.1f);
        animator.SetBool("Explode", true);
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
}
