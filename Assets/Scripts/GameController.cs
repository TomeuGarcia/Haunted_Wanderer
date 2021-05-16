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

    //health controller
    public HPGained hpGained;
    public HealthBar healthBar;

    // Enemies
    private GameObject[] sceneEnemies;
    // Platforms
    private GameObject[] scenePlatforms;
    // Hazards
    private GameObject[] sceneHazards;

    // Flags
    private int playerSanityState;
    // 1 = SanityState changed to HIGH
    // 2 = SanityState changed to MEDIUM
    // 3 = SanityState changed to LOW

    // SINGLETON
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
        sceneEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject e in sceneEnemies)
        {
            e.GetComponent<EnemyController>().setActiveState(false);
        }

        // SET PLATFORMS
        scenePlatforms = GameObject.FindGameObjectsWithTag("Platform");
        foreach (GameObject p in scenePlatforms)
        {
            p.GetComponent<PlatformController>().setActiveState(false);
        }

        // SET HAZARDS
        sceneHazards = GameObject.FindGameObjectsWithTag("Hazard");
        foreach (GameObject h in sceneHazards)
        {
            h.GetComponent<HazardController>().setActiveState(false);
        }

        // FLAGS
        // 0 = NONE
        playerSanityState = 0;
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
            StartCoroutine(waitReactiveScene());
            return;
        }

        // player can't die from losing sanity over time (stays at 10%)
        else if (myPlayer.getCurrentSanity() > (myPlayer.getLimit()))
        {
            myPlayer.loseSanityViaTime();
            myPlayer.updateSanityState();
            myPlayer.updateMovementSpeed();
            if (myPlayer.canUpdateSanity)
                updateScenary();
        }
    }


    // Function that updates the scene based on player's SanityState
    private void updateScenary()
    {
        PlayerController.SanityState sanity = myPlayer.getSanityState();

        // HIGH SANITY
        if (playerSanityState != 1 && sanity == PlayerController.SanityState.HIGH)
        {
            playerSanityState = 1;

            // ENEMIES
            foreach (GameObject e in sceneEnemies)
            {
                EnemyController ec = e.GetComponent<EnemyController>();
                if (ec.canSpawn && ec.spawnsHighSanity)
                    ec.setActiveState(true);    
                else if (!ec.highSanity)
                    ec.setActiveState(false);
            }

            // PLATFORMS
            foreach (GameObject p in scenePlatforms)
            {
                PlatformController pc = p.GetComponent<PlatformController>();
                if (!pc.isActive && pc.highSanity)
                {
                    pc.setActiveState(true);
                }
                else if (!pc.highSanity)
                {
                    pc.setActiveState(false);
                }
            }

            // HAZARDS
            foreach (GameObject h in sceneHazards)
            {
                PlatformController hc = h.GetComponent<PlatformController>();
                if (!hc.isActive && hc.highSanity)
                {
                    hc.setActiveState(true);
                }
                else if (!hc.highSanity)
                {
                    hc.setActiveState(false);
                }
            }
        }
        // MEDIUM SANITY
        else if (playerSanityState != 2 && sanity == PlayerController.SanityState.MEDIUM)
        {
            playerSanityState = 2;

            // ENEMIES
            foreach (GameObject e in sceneEnemies)
            {
                EnemyController ec = e.GetComponent<EnemyController>();
                if (ec.canSpawn && ec.spawnsMediumSanity)
                    ec.setActiveState(true);
                else if (!ec.mediumSanity)
                    ec.setActiveState(false);
            }

            // PLATFORMS
            foreach (GameObject p in scenePlatforms)
            {
                PlatformController pc = p.GetComponent<PlatformController>();
                if (!pc.isActive && pc.mediumSanity)
                {
                    pc.setActiveState(true);
                }
                else if (!pc.mediumSanity)
                {
                    pc.setActiveState(false);
                }
            }

            // HAZARDS
            foreach (GameObject h in sceneHazards)
            {
                PlatformController hc = h.GetComponent<PlatformController>();
                if (!hc.isActive && hc.mediumSanity)
                {
                    hc.setActiveState(true);
                }
                else if (!hc.mediumSanity)
                {
                    hc.setActiveState(false);
                }
            }
        }
        // LOW SANITY
        else if (playerSanityState != 3 && sanity == PlayerController.SanityState.LOW)
        {
            playerSanityState = 3;

            // ENEMIES
            foreach (GameObject e in sceneEnemies)
            {
                EnemyController ec = e.GetComponent<EnemyController>();
                if (ec.canSpawn && ec.spawnsLowSanity)
                    ec.setActiveState(true);
                else if (!ec.lowSanity)
                    ec.setActiveState(false);
            }

            // PLATFORMS
            foreach (GameObject p in scenePlatforms)
            {
                PlatformController pc = p.GetComponent<PlatformController>();
                if (!pc.isActive && pc.lowSanity)
                {
                    pc.setActiveState(true);
                }
                else if (!pc.lowSanity)
                {
                    pc.setActiveState(false);
                }
            }

            // HAZARDS
            foreach (GameObject h in sceneHazards)
            {
                PlatformController hc = h.GetComponent<PlatformController>();
                if (!hc.isActive && hc.lowSanity)
                {
                    hc.setActiveState(true);
                }
                else if (!hc.lowSanity)
                {
                    hc.setActiveState(false);
                }
            }

        }

    }


    // Function that reactivates scene 
    public void reactivateScene()
    {
        // Having added: using UnityEngine.SceneManagement;
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    IEnumerator waitReactiveScene()
    {
        yield return new WaitForSeconds(1f);
        reactivateScene();
    }

}


