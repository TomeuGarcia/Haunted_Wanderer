using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // Singleton
    private static GameController Instance = null;

    // Player
    public PlayerController myPlayer;

    // Enemies
    private GameObject[] highSanityEnemies;
    private GameObject[] mediumSanityEnemies;
    private GameObject[] lowSanityEnemies;


    // Platforms
    public GameObject[] highSanityPlatforms;
    public GameObject[] mediumSanityPlatforms;
    public GameObject[] lowSanityPlatforms;


    // Flags
    private int playerSanityState;
    // 1 = SanityState changed to HIGH
    // 2 = SanityState changed to MEDIUM
    // 3 = SanityState changed to LOW

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
    }


    void Start()
    {
        // SET ENEMIES
        foreach (GameObject e in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (e.GetComponent<EnemyController>().spawnsHighSanity)

        }

        // Store enemies that appear when Player's sanity is LOW
        lowSanityEnemies = GameObject.FindGameObjectsWithTag("LowSanityEnemies");
        foreach (GameObject e in lowSanityEnemies)
        {
            e.GetComponent<EnemyController>().setActiveState(false);
        }
        // Store enemies that appear when Player's sanity is MEDIUM
        mediumSanityEnemies = GameObject.FindGameObjectsWithTag("MediumSanityEnemies");
        foreach (GameObject e in mediumSanityEnemies)
        {
            e.GetComponent<EnemyController>().setActiveState(false);
        }

        // SET PLATFORMS
        // Store platforms that appear when Player's sanity is LOW
        lowSanityPlatforms = GameObject.FindGameObjectsWithTag("LowSanityPlatforms");
        foreach (GameObject p in lowSanityPlatforms)
        {
            p.GetComponent<PlatformController>().setActiveState(false);
        }
        // Store platforms that appear when Player's sanity is MEDIUM
        mediumSanityPlatforms = GameObject.FindGameObjectsWithTag("MediumSanityPlatforms");
        foreach (GameObject p in mediumSanityPlatforms)
        {
            p.GetComponent<PlatformController>().setActiveState(false);
        }

        // FLAGS
        // 1 = SanityState changed to HIGH
        playerSanityState = 1;
    }

    void Update()
    {
        playerSanityCycle();
    }



    // Function that executes the player Sanity cycle
    private void playerSanityCycle()
    {
        if (myPlayer.getCurrentSanity() == 0)
        {
            reactivateScene();
            return;
        }

        // player can't die from losing sanity over time (stays at 10%)
        else if (myPlayer.getCurrentSanity() > (myPlayer.getMaxSanity() / 10))
        {
            myPlayer.loseSanityViaTime();
            myPlayer.updateSanityState();
            myPlayer.updateMovementSpeed();
            updateScenary(myPlayer.getSanityState());
        }
    }


    // Function that updates the scene based on player's SanityState
    private void updateScenary(PlayerController.SanityState sanity)
    {
        // HIGH SANITY
        if (playerSanityState != 1 && sanity == PlayerController.SanityState.HIGH)
        {
            /*
            // ENEMIES
            foreach (GameObject e in mediumSanityEnemies)
            {
                e.GetComponent<EnemyController>().setActiveState(false);
            }
            if (playerSanityState == 3) 
            {
                foreach (GameObject e in lowSanityEnemies)
                {
                    e.GetComponent<EnemyController>().setActiveState(false);
                }
            }    
            // PLATFORMS
            foreach (GameObject p in mediumSanityPlatforms)
            {
                p.GetComponent<PlatformController>().setActiveState(false);
            }
            if (playerSanityState == 3)
            {
                foreach (GameObject p in lowSanityPlatforms)
                {
                    p.GetComponent<PlatformController>().setActiveState(false);
                }
            }
            */
            foreach (GameObject e in mediumSanityEnemies)
            {
                e.GetComponent<EnemyController>().setActiveState(false);
            }
            foreach (GameObject e in lowSanityEnemies)
            {
                e.GetComponent<EnemyController>().setActiveState(false);
            }
            foreach (GameObject p in mediumSanityPlatforms)
            {
                p.GetComponent<PlatformController>().setActiveState(false);
            }
            foreach (GameObject p in lowSanityPlatforms)
            {
                p.GetComponent<PlatformController>().setActiveState(false);
            }
            playerSanityState = 1;
        }
        // MEDIUM SANITY
        else if (playerSanityState != 2 && sanity == PlayerController.SanityState.MEDIUM)
        {
            /*
            // ENEMIES
            if (playerSanityState == 1)
            {
                foreach (GameObject e in mediumSanityEnemies)
                {
                    e.GetComponent<EnemyController>().setActiveState(true);
                }
            }
            if (playerSanityState == 3)
            {
                foreach (GameObject e in lowSanityEnemies)
                {
                    e.GetComponent<EnemyController>().setActiveState(false);
                }
            }
            // PLATFORMS
            foreach (GameObject p in mediumSanityPlatforms)
            {
                p.GetComponent<PlatformController>().setActiveState(true);
            }
            if (playerSanityState == 3)
            {
                foreach (GameObject p in lowSanityPlatforms)
                {
                    p.GetComponent<PlatformController>().setActiveState(false);
                }
            }
            */
            foreach (GameObject e in mediumSanityEnemies)
            {
                e.GetComponent<EnemyController>().setActiveState(true);
            }
            foreach (GameObject e in lowSanityEnemies)
            {
                e.GetComponent<EnemyController>().setActiveState(false);
            }
            foreach (GameObject p in mediumSanityPlatforms)
            {
                p.GetComponent<PlatformController>().setActiveState(true);
            }
            foreach (GameObject p in lowSanityPlatforms)
            {
                p.GetComponent<PlatformController>().setActiveState(false);
            }
            playerSanityState = 2;
        }
        // LOW SANITY
        else if (playerSanityState != 3 && sanity == PlayerController.SanityState.LOW)
        {
            /*
            // ENEMIES
            if (playerSanityState == 1)
            {
                foreach (GameObject e in mediumSanityEnemies)
                {
                    e.GetComponent<EnemyController>().setActiveState(true);
                }
            }
            foreach (GameObject e in lowSanityEnemies)
            {
                e.GetComponent<EnemyController>().setActiveState(true);
            }
            // PLATFORMS
            if (playerSanityState == 2)
            {
                foreach (GameObject p in mediumSanityPlatforms)
                {
                    p.GetComponent<PlatformController>().setActiveState(false);
                }
            }
            foreach (GameObject p in lowSanityPlatforms)
            {
                p.GetComponent<PlatformController>().setActiveState(true);
            }
            */
            foreach (GameObject e in lowSanityEnemies)
            {
                e.GetComponent<EnemyController>().setActiveState(true);
            }
            foreach (GameObject p in mediumSanityPlatforms)
            {
                p.GetComponent<PlatformController>().setActiveState(false);
            }
            foreach (GameObject p in lowSanityPlatforms)
            {
                p.GetComponent<PlatformController>().setActiveState(true);
            }
            playerSanityState = 3;
        }

    }


    // Function that reactivates scene 
    public void reactivateScene()
    {
        // Having added: using UnityEngine.SceneManagement;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

}
