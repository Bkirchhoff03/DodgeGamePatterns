using Assets.Scripts;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GameManager : MonoBehaviour
{
    public FallerManager fallerManager { get; private set; }
    float currentTimeBetweenSpawns = 2.5f;
    float TimeBetweenSpawns;
    public Sprite sprite;
    static GameManager instance_;
    private int playerLives = 3;
    public TextMeshProUGUI lifeCounter;
    //public GameObject camera;
    public GameObject player;
    public GameObject trapDoor; // Assign the TrapDoor GameObject in the Inspector
    private float spawnHeight = Constants.spawnY;
    private float fallerSpawnCameraDiff;
    private PlayerController playerController;
    public string currentFallerSaveFileName = "fallerSaveData.json"; // For testing purposes, the name of the file to save/load faller data
    public string currentPlayerSaveFileName = "playerSaveData.json"; // For testing purposes, the name of the file to save/load player data
    private float clickSpawnCooldown = 0.0f; // Minimum time between spawns when using clickToSpawn
    public bool clickToSpawn = false; // For testing purposes, allows spawning a faller on click instead of timer
    public bool spawnFallersFromFile = false; // For testing purposes, allows spawning fallers from a saved file on start
    public bool isPaused = false;
    private GameObject pausePanel;
    private TextMeshProUGUI HeightTracker;
    private float trapDoorHeight;
    private float cameraInitialY;
    public enum PlayerFallerCollisionType
    {
        Top,
        Bottom,
        Left,
        Right,
        None
    }
    public static GameManager instance() => instance_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance_ = this;
        TimeBetweenSpawns = currentTimeBetweenSpawns;
        // Read trapdoor height to cap faller spawn height; default to 50 if no trapdoor assigned
        trapDoorHeight = trapDoor != null ? trapDoor.GetComponent<TrapDoor>().height : 50.0f;

        fallerManager = new FallerManager();
        // FallerManager now owns the faller dictionary, sprite, and spawn height logic
        fallerManager.init(sprite, trapDoorHeight+10.0f);

        playerController = player.GetComponent<PlayerController>();
        HeightTracker = GameObject.Find("HeightTracker").GetComponent<TextMeshProUGUI>();
        HeightTracker.text = (trapDoorHeight - player.transform.position.y).ToString("0.00") + Constants.heightTrackerText;
        //if (GetComponent<Camera>() == null)
        //{
        //    camera = new GameObject("Main Camera");
        //    GetComponent<Camera>().AddComponent<Camera>();
        //    GetComponent<Camera>().transform.position = new Vector3(0.0f, 7.0f, -20.0f);
        //}
        cameraInitialY = Camera.main.transform.position.y;
        fallerSpawnCameraDiff = spawnHeight - Camera.main.transform.position.y;
        string pendingSave = PlayerPrefs.GetString("pendingSaveFile", "");
        if (!string.IsNullOrEmpty(pendingSave))
        {
            PlayerPrefs.DeleteKey("pendingSaveFile");
            fallerManager.LoadFallersFromFile(playerController, pendingSave);
            
        }else if (spawnFallersFromFile)
        {
            fallerManager.LoadFallersFromFile(playerController);
        }
            /*if(spawnFallersFromFile)
            {
                fallerManager.LoadFallersFromFile(playerController);
            }*/
            pausePanel = GameObject.Find("PausePanel");
        pausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(clickSpawnCooldown > 0)
        {
            clickSpawnCooldown -= Time.deltaTime;
        }
        
        if (TimeBetweenSpawns > 0)
        {
            TimeBetweenSpawns -= Time.deltaTime;
        }
        else if (!clickToSpawn)
        {
            SpawnObject();
            TimeBetweenSpawns = currentTimeBetweenSpawns;
        }
        if(player != null && player.transform.position.y > cameraInitialY)
        {

            Camera.main.transform.position = new Vector3(0.0f, player.transform.position.y, -20.0f);
            spawnHeight = Camera.main.transform.position.y + fallerSpawnCameraDiff;
        }
        else if(player != null)
        {
            Camera.main.transform.position = new Vector3(0.0f, cameraInitialY, -20.0f);
            spawnHeight = Camera.main.transform.position.y + fallerSpawnCameraDiff;
        }
        HeightTracker.text = (trapDoorHeight - player.transform.position.y).ToString("0.00") + Constants.heightTrackerText; 
    }
    public void HandlePlayerFallerCollision(GameObject player, GameObject faller, PlayerFallerCollisionType collisionType)
    {
        Debug.Log("FROM GAME MANAGER: Player " + player.name + " collided with Faller " + faller.name);
        PlayerController playerController = player.GetComponent<PlayerController>();
        NormalFallerController fallerBehavior = faller.GetComponent<NormalFallerController>();
        if (collisionType == PlayerFallerCollisionType.Bottom && playerController.canBeDamaged())
        {
            playerLives--;
            string text = "Lives: ";
            for (int i = 0; i < playerLives; i++)
            {
                text += "I";
            }
            lifeCounter.text = text;
            if (playerLives <= 0)
            {
                playerLives = 3;
                Debug.Log("GAME OVER");
            }
            playerController.crush();
            DeleteFaller(faller.name);
        }else if(collisionType == PlayerFallerCollisionType.Top)
        {
            if(faller.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
            {
                faller.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0.0f, 0.0001f);
            }
            playerController.rideFaller(faller);
        }else if(collisionType == PlayerFallerCollisionType.Left || collisionType == PlayerFallerCollisionType.Right)
        {
            playerController.BounceOff(faller, collisionType);
        }
    }
    // Delegates faller creation to FallerManager, which handles positioning and tracking
    void SpawnObject()
    {
        FallerManager.instance().SpawnFaller(spawnHeight);
    }
    // Delegates faller destruction to FallerManager, which handles cleanup from its dictionary
    void DeleteFaller(string nameOfFaller)
    {
        FallerManager.instance().RemoveFaller(nameOfFaller);
    }
    // Called by TrapDoor when the player reaches the goal; reloads the current scene
    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pausePanel.SetActive(isPaused);
    }

    public void ResumeGame() => TogglePause();

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    public void givePlayerTime()
    {
        TimeBetweenSpawns += currentTimeBetweenSpawns;
    }

    public void SpawnFallerAtClick(Vector3 clickPosition)
    {
        if (clickSpawnCooldown > 0)
        {
            return; // Prevent spawning if cooldown is active
        }
        clickSpawnCooldown = 0.5f; // Reset cooldown
        clickPosition.z = 20f; // Set z to a positive value to ensure it's in front of the camera
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(clickPosition);
        //Debug.Log("Spawning faller at: " + worldPosition + " from click position: " + clickPosition);
        worldPosition.z = 0f; // Set z to 0 for 2D
        FallerManager.instance().SpawnFallerAtPosition(worldPosition, Constants.defaultFallerSize);
    }
}
