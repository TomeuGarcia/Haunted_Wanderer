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
    /*
    public GameObject[] highSanityEnemies;
    public GameObject[] mediumSanityEnemies;
    public GameObject[] lowSanityEnemies;
    */

    // Platforms
    private GameObject[] scenePlatforms;
    /*
    public GameObject[] highSanityPlatforms;
    public GameObject[] mediumSanityPlatforms;
    public GameObject[] lowSanityPlatforms;
    */

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

        /*
        // Store enemies that appear when Player's sanity is HIGH
        highSanityEnemies = GameObject.FindGameObjectsWithTag("HighSanityEnemies");
        foreach (GameObject e in highSanityEnemies)
        {
            e.GetComponent<EnemyController>().setActiveState(false);
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
        */


        // SET PLATFORMS
        scenePlatforms = GameObject.FindGameObjectsWithTag("Platform");
        foreach (GameObject e in scenePlatforms)
        {
            e.GetComponent<PlatformController>().setActiveState(false);
        }

        /*
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
        */

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
                if (ec.spawnsLowSanity)
                    ec.setActiveState(true);
                else
                    ec.setActiveState(false);
            }

            // PLATFORMS
            foreach (GameObject p in scenePlatforms)
            {
                PlatformController pc = p.GetComponent<PlatformController>();
                if (pc.spawnsLowSanity)
                    pc.setActiveState(true);
                else
                    pc.setActiveState(false);
            }

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
            */
            
        }
        // MEDIUM SANITY
        else if (playerSanityState != 2 && sanity == PlayerController.SanityState.MEDIUM)
        {
            playerSanityState = 2;

            // ENEMIES
            foreach (GameObject e in sceneEnemies)
            {
                EnemyController ec = e.GetComponent<EnemyController>();
                if (ec.spawnsMediumSanity)
                    ec.setActiveState(true);
                else if (ec.spawnsLowSanity)
                    ec.setActiveState(false);
            }

            // PLATFORMS
            foreach (GameObject p in scenePlatforms)
            {
                PlatformController pc = p.GetComponent<PlatformController>();
                if (pc.spawnsMediumSanity)
                    pc.setActiveState(true);
                else 
                    pc.setActiveState(false);
            }

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
            */
        }
        // LOW SANITY
        else if (playerSanityState != 3 && sanity == PlayerController.SanityState.LOW)
        {
            playerSanityState = 3;

            // ENEMIES
            foreach (GameObject e in sceneEnemies)
            {
                EnemyController ec = e.GetComponent<EnemyController>();
                if (ec.spawnsLowSanity)
                    ec.setActiveState(true);
            }

            // PLATFORMS
            foreach (GameObject p in scenePlatforms)
            {
                PlatformController pc = p.GetComponent<PlatformController>();
                if (pc.spawnsLowSanity)
                    pc.setActiveState(true);
                else
                    pc.setActiveState(false);
            }

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
            */
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
