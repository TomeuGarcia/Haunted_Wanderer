using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Player Checkpoint respawn coords:")]
    [SerializeField] public PlayerController player;
    [SerializeField] public float playerX;
    [SerializeField] public float playerY;

    [Header("Camera Checkpoint respawn coords:")]
    [SerializeField] public CameraController cam;
    [SerializeField] public float camX;
    [SerializeField] public float camY;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.respawnPosition = new Vector2(playerX, playerY);
            cam.respawnPosition = new Vector3(camX, camY, cam.transform.position.z);
        }
    }
}
