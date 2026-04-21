using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
//using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GameManager : MonoBehaviour
{
    public FallerManager fallerManager { get; private set; }
    float currentTimeBetweenSpawns = 1.5f;
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
    public bool unlimitedLives = false; // For testing purposes, prevents player from losing lives
    public bool spawnFallersFromFile = false; // For testing purposes, allows spawning fallers from a saved file on start
    public bool isPaused = false;
    private GameObject pausePanel;
    private GameObject saveNamePanel;
    private TMPro.TMP_InputField saveNameInput;
    private GameObject gameOverPanel;
    private TextMeshProUGUI HeightTracker;
    private float trapDoorHeight;
    private float cameraInitialY;
    public Sprite LeftGrassTile;
    public Sprite RightGrassTile;
    public Sprite CenterGrassTile;
    public FallerManager.FallerType fallerType = FallerManager.FallerType.Block;
    public bool verboseLogging = true; // Set to true to enable debug logs for player-faller collisions and other events
    private float stuckTimer = 0f;
    private float stuckThreshold = 5.0f; // Set a default value for the stuck threshold
    private int recentHeightRecordCount = 50; // Number of recent heights to track for determining if the player is stuck
    private Queue<float> maxPlayerHeightRecently = new Queue<float>(); // Track the maximum height the player has reached recently to help determine if they're stuck
    private bool checkstuck = false;
    public enum PlayerFallerCollisionType
    {
        Top,
        Bottom,
        Left,
        Right,
        None
    }
    //Constructor for testing purposes; in normal gameplay, the instance is set in Start() and accessed via the static instance() method
    public GameManager()
    {
        instance_ = this;
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
        fallerManager.init(fallerType, trapDoorHeight+10.0f);

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
        if (PlayerPrefs.GetInt("clickToSpawnTester") == 1)
        {
            clickToSpawn = true;
        }
        else
        {
            clickToSpawn = false;
        }
        if (PlayerPrefs.GetInt("unlimitedLivesTester") == 1)
        {
            unlimitedLives = true;
        }
        else
        {
            unlimitedLives = false;
        }
        pausePanel = GameObject.Find("PausePanel");
        pausePanel.SetActive(false);
        saveNamePanel = GameObject.Find("SaveNamePanel");
        saveNamePanel.SetActive(false);
        saveNameInput = saveNamePanel.GetComponentInChildren<TMPro.TMP_InputField>();
        gameOverPanel = GameObject.Find("GameOverPanel");
        gameOverPanel.SetActive(false);
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

        CheckIfPlayerStuck();
        /*Camera.main.transform.position = new Vector3(0.0f, player.transform.position.y, -20.0f);
        spawnHeight = Camera.main.transform.position.y + fallerSpawnCameraDiff;*/
        if (player != null && player.transform.position.y > cameraInitialY)
        {

            Camera.main.transform.position = new Vector3(0.0f, player.transform.position.y, -20.0f);
            spawnHeight = Camera.main.transform.position.y + fallerSpawnCameraDiff;
        }
        else if (player != null)
        {
            Camera.main.transform.position = new Vector3(0.0f, cameraInitialY, -20.0f);
            spawnHeight = Camera.main.transform.position.y + fallerSpawnCameraDiff;
        }
        HeightTracker.text = (trapDoorHeight - player.transform.position.y).ToString("0.00") + Constants.heightTrackerText; 
    }

    private void triggerRescueSpawn()
    {
        FallerController rescue = FallerManager.instance().SpawnRescue(player.transform.position, Camera.main.transform.position.y + 9f);
        if (rescue == null)
        {
            GameManager.instance().Print("Rescue spawn failed!", 1);
            // Additional logic if rescue spawn fails, such as trying again after a delay or notifying the player
        }
    }
    private void CheckIfPlayerStuck()
    {
        if(fallerType == FallerManager.FallerType.Boulder || clickToSpawn)
        {
            return; // Don't check for stuck if we're already spawning boulders, haven't figured that out yet
        }
        if (!checkstuck)
        {
            //FallerManager.instance().RemoveAllTints();
            //FallerController l = FallerManager.instance().GetLowestReachableFaller(playerController.transform.position, new Vector3(3.83f, 4.94f, 0f));
            //GameManager.instance().Print(string.Join(", ", maxPlayerHeightRecently), 1);
            //playerController.PlayerAnimationGameObject.GetComponent<SpriteRenderer>().color = Color.white;
            maxPlayerHeightRecently.Enqueue(player.transform.position.y);
            if (maxPlayerHeightRecently.Count > recentHeightRecordCount)
            {
                float oldest = maxPlayerHeightRecently.Dequeue();
                if (oldest >= System.Linq.Enumerable.Max(maxPlayerHeightRecently) && player.transform.position.y < FallerManager.instance().GetHighestFrozenFallerY())
                {
                    checkstuck = true;
                    GameManager.instance().Print("Player may be stuck, starting timer...", 1);
                }
            }
        }
        else
        {
            //playerController.PlayerAnimationGameObject.GetComponent<SpriteRenderer>().color = new Color(stuckTimer/stuckThreshold, 0f, 0f, 1f);
            FallerController l = FallerManager.instance().GetLowestReachableFaller(playerController.transform.position, new Vector3(Constants.maxXJumpDistance, Constants.maxYJumpHeight, 0f));
            if (l == null)
            {
                //GameManager.instance().Print("No reachable fallers!! " + stuckTimer, 1);
                stuckTimer += Time.deltaTime;
                if (stuckTimer >= stuckThreshold)
                {
                    triggerRescueSpawn();
                    stuckTimer = 0f;
                }
                //FallerManager.instance().RemoveAllTints();
            }
            else
            {
                stuckTimer = 0f;
                checkstuck = false;
                maxPlayerHeightRecently.Clear();
                GameManager.instance().Print("Found a reachable faller!!", 1);
                //l.AddRedTint();
            }
        }
    }
    public void HandlePlayerFallerCollision(GameObject player, GameObject faller, PlayerFallerCollisionType collisionType)
    {
        Print("FROM GAME MANAGER: Player " + player.name + " collided with Faller " + faller.name);
        PlayerController playerController = player.GetComponent<PlayerController>();
        FallerController fallerBehavior = faller.GetComponent<FallerController>();
        if (collisionType == PlayerFallerCollisionType.Bottom && playerController.canBeDamaged() && !fallerBehavior.IsFrozen)
        {
            if (!unlimitedLives)
            {
                playerLives--;
            }
            string text = "Lives: ";
            for (int i = 0; i < playerLives; i++)
            {
                text += "I";
            }
            lifeCounter.text = text;
            if (playerLives <= 0)
            {
                playerLives = 3;
                Print("GAME OVER");
                GameOver("You ran out of lives!");
                //SceneManager.LoadScene("MainMenu");
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
        }/*else if(collisionType == PlayerFallerCollisionType.Left || collisionType == PlayerFallerCollisionType.Right)
        {
            playerController.BounceOff(faller, collisionType);
        }*/
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
    public void ResetGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1");
    }
    public void ShowSaveNamePanel()
    {
        saveNameInput.text = "";
        saveNamePanel.SetActive(true);
    }
    public void ConfirmSave()
    {
        string saveName = saveNameInput.text.Trim();
        if (string.IsNullOrEmpty(saveName))
            saveName = "Save";
        saveNamePanel.SetActive(false);
        FallerManager.instance().SaveFallersToFile(playerController, saveName);
    }
    public void CancelSave()
    {
        saveNamePanel.SetActive(false);
    }
    public void SaveLevel()
    {
        ShowSaveNamePanel();
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        pausePanel.SetActive(isPaused);
    }

    public void ResumeGame() => TogglePause();
    public void GameOver(string reason)
    {
        Time.timeScale = 0f;
        gameOverPanel.SetActive(true);
        TextMeshProUGUI gameOverText = gameOverPanel.transform.Find("GameOverReasonText").GetComponent<TextMeshProUGUI>();
        gameOverText.text = reason;
    }
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
        //GameManager.instance().Print("Spawning faller at: " + worldPosition + " from click position: " + clickPosition);
        worldPosition.z = 0f; // Set z to 0 for 2D
        FallerManager.instance().ForceSpawnFaller(worldPosition.y, worldPosition.x, Constants.defaultFallerSize, Constants.maxFallerSpeed, false);
        //FallerManager.instance().SpawnFallerAtPosition(worldPosition, Constants.defaultFallerSize);
    }
    public void Print(string message, int level = 0)
    {
        if (message != null && verboseLogging && level == 0)
        {
            Debug.Log(message);
        }
        else if (message != null && level == 1)
        {
            Debug.Log(message);
        }
    }
}
