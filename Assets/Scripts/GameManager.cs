using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
[DefaultExecutionOrder(-10)]
public class GameManager : MonoBehaviour
{
    public FallerManager fallerManager { get; private set; }
    float currentTimeBetweenSpawns = 1.5f;
    float TimeBetweenSpawns;
    public Sprite sprite;
    static GameManager instance_;
    private float playerLives = Constants.maxPlayerLives;
    private float playerStamina = Constants.maxPlayerStamina;
    public TextMeshProUGUI lifeCounter;
    [SerializeField] public Image lifeBarFill;
    [SerializeField] public Image staminaBarFill;
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
    private TextMeshProUGUI TimeTracker;
    private float trapDoorHeight;
    private float cameraInitialY;
    public Sprite LeftGrassTile;
    public Sprite RightGrassTile;
    public Sprite CenterGrassTile;

    public Sprite LeftDirtTile;
    public Sprite RightDirtTile;
    public Sprite CenterDirtTile;

    public FallerManager.FallerType fallerType = FallerManager.FallerType.Block;
    public FallerManager.FallerType[] FallerTypes;
    public bool verboseLogging = true; // Set to true to enable debug logs for player-faller collisions and other events
    public bool verboseFallerCollision = false;
    public bool verbosePlayerCollision = false;
    public bool verboseFallerStateChanges = false;
    public bool verbosePlayerStateChanges = false;
    public bool verboseGameState = false;
    public bool verboseSavingLoading = false;
    public bool verboseRescuing = false;
    public bool verboseAnimations = false;
    public bool verboseTesting = false;
    private bool[] verboseSettings = new bool[] {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false
    }; // Array to control verbose logging for different levels or categories of logs
     
    private float stuckTimer = 0f;
    private float stuckThreshold = 5.0f; // Set a default value for the stuck threshold
    private int recentHeightRecordCount = 50; // Number of recent heights to track for determining if the player is stuck
    private Queue<float> maxPlayerHeightRecently = new Queue<float>(); // Track the maximum height the player has reached recently to help determine if they're stuck
    private bool checkstuck = false;
    private float EMT_timer = 0f;
    private float EMT_duration = 5f; // Duration of the EMT effect in seconds
    public float TestingFallerSpawnSize = 7f;
    private Vector2[] TestingFallerSpawnSizeOptions = new Vector2[] {
        new Vector2(0.5f, 0.5f),
        new Vector2(0.5f, 1f),
        new Vector2(0.5f, 1.5f),
        new Vector2(0.5f, 2f),
        new Vector2(0.5f, 2.5f),
        new Vector2(0.5f, 3f),
        new Vector2(1f, 0.5f),
        new Vector2(1f, 1f),
        new Vector2(1f, 1.5f),
        new Vector2(1f, 2f),
        new Vector2(1f, 2.5f),
        new Vector2(1f, 3f),
        new Vector2(1.5f, 0.5f),
        new Vector2(1.5f, 1f),
        new Vector2(1.5f, 1.5f),
        new Vector2(1.5f, 2f),
        new Vector2(1.5f, 2.5f),
        new Vector2(1.5f, 3f),
        new Vector2(2f, 0.5f),
        new Vector2(2f, 1f),
        new Vector2(2f, 1.5f),
        new Vector2(2f, 2f),
        new Vector2(2f, 2.5f),
        new Vector2(2f, 3f),
        new Vector2(2.5f, 0.5f),
        new Vector2(2.5f, 1f),
        new Vector2(2.5f, 1.5f),
        new Vector2(2.5f, 2f),
        new Vector2(2.5f, 2.5f),
        new Vector2(2.5f, 3f),
        new Vector2(3f, 0.5f),
        new Vector2(3f, 1f),
        new Vector2(3f, 1.5f),
        new Vector2(3f, 2f),
        new Vector2(3f, 2.5f),
        new Vector2(3f, 3f)
    };
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
        verboseSettings = new bool[] { 
            verboseFallerCollision,
            verbosePlayerCollision,
            verboseFallerStateChanges,
            verbosePlayerStateChanges,
            verboseGameState,
            verboseSavingLoading,
            verboseRescuing,
            verboseAnimations,
            verboseTesting
        };
        instance_ = this;
        TimeBetweenSpawns = currentTimeBetweenSpawns;
        // Read trapdoor height to cap faller spawn height; default to 50 if no trapdoor assigned
        trapDoorHeight = trapDoor != null ? trapDoor.GetComponent<TrapDoor>().height : 50.0f;

        fallerManager = new FallerManager();
        // FallerManager now owns the faller dictionary, sprite, and spawn height logic
        if(FallerTypes == null || FallerTypes.Length == 0)
        {
            FallerTypes = new FallerManager.FallerType[] { fallerType };
        }
        fallerManager.init(FallerTypes, trapDoorHeight+10.0f);

        playerController = player.GetComponent<PlayerController>();
        HeightTracker = GameObject.Find("HeightTracker").GetComponent<TextMeshProUGUI>();
        HeightTracker.text = (trapDoorHeight - player.transform.position.y).ToString("0.00") + Constants.heightTrackerText;

        TimeTracker = GameObject.Find("TimeTracker").GetComponent<TextMeshProUGUI>();
        if(TimeManager.Instance == null)
        {
            if(GameObject.Find("TimeManager") == null)
            {
                GameObject timeManagerObject = new GameObject("TimeManager");
                timeManagerObject.AddComponent<TimeManager>();
            }
        }
        TimeTracker.text = TimeManager.Instance.GetTimeInGame().ToString() + Constants.timeTrackerText;
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
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            StartSaveSession();
        }
        else
        {
            float livesFromLevel1 = PlayerPrefs.GetFloat("PlayerLivesFromLevel1");
            if (livesFromLevel1 != 0)
            {
                playerLives = livesFromLevel1;

            }
            UpdateSaveSession();
        }
        UpdateLifeUI();

    }

    // Update is called once per frame
    void Update()
    {
        TimeTracker.text = TimeManager.Instance.GetTimeInGame().ToString() + Constants.timeTrackerText;
        if (clickSpawnCooldown > 0)
        {
            clickSpawnCooldown -= Time.deltaTime;
        }
        if (clickToSpawn && Input.mouseScrollDelta.y != 0f)
        {
            TestingFallerSpawnSize += Input.mouseScrollDelta.y;
            if(TestingFallerSpawnSize < 0)
            {
                TestingFallerSpawnSize = TestingFallerSpawnSizeOptions.Length - 1;
            }
            TestingFallerSpawnSize %= TestingFallerSpawnSizeOptions.Length;
            Print("Testing faller spawn size: " + TestingFallerSpawnSizeOptions[(int)TestingFallerSpawnSize], 8);
        }
        if (EMT_timer > 0f)
        {
            EMT_timer -= Time.deltaTime;
            if (EMT_timer <= 0f)
            {
                EMT_timer = 0f;
                //FallerManager.instance().RemoveAllTints();
                // Logic to end EMT effect
            }
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
        UpdateStaminaRegen();
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
        HeightTracker.text = (Mathf.Round((trapDoorHeight - player.transform.position.y)*10f)/10f).ToString("0.0") + Constants.heightTrackerText;
        
    }
    public bool IsPlayerInEMT() => EMT_timer > 0f;
    private void triggerRescueSpawn()
    {
        float distance = Mathf.Abs(Camera.main.transform.position.z); // Distance from the camera
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distance));
        //FallerController rescue = FallerManager.instance().SpawnRescue(player.transform.position, Camera.main.transform.position.y + 9f);
        FallerController rescue = FallerManager.instance().SpawnRescue(player.transform.position, topRight.y + 1.5f);
        if (rescue == null)
        {
            GameManager.instance().Print("Rescue spawn failed!", 6);
            // Additional logic if rescue spawn fails, such as trying again after a delay or notifying the player
        }
    }
    private void CheckIfPlayerStuck()
    {
        if(fallerType == FallerManager.FallerType.Boulder || fallerType == FallerManager.FallerType.BombBolder || clickToSpawn)
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
                    GameManager.instance().Print("Player may be stuck, starting timer...", 6);
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
                GameManager.instance().Print("Found a reachable faller!!", 6);
                //l.AddRedTint();
            }
        }
    }
    public void HandlePlayerFallerCollision(GameObject player, GameObject faller, PlayerFallerCollisionType collisionType)
    {
        Print("FROM GAME MANAGER: Player collided " + collisionType + " with Faller " + faller.name,1);
        PlayerController playerController = player.GetComponent<PlayerController>();
        FallerController fallerBehavior = faller.GetComponent<FallerController>();
        if (collisionType == PlayerFallerCollisionType.Bottom && !fallerBehavior.IsFrozen)
        {
            if (playerController.canBeDamaged())
            {
                Print("FROM GAME MANAGER: Player in " + playerController.state.getName() + " at " + playerController.gameObject.transform.position + " collided to lose a life with Faller " + faller.name, 3);
                TakeDamage(Constants.headBonkLifeCost);
                playerController.crush();
                DeleteFaller(faller.name);
            }
            else if (playerController.state.getName() == Constants.jumpingStateName)
            {
                playerController.setState(playerController.GetStateFromName(Constants.fallingStateName));
            }
            else if (!fallerBehavior.IsFrozen && playerController.state.getName() == Constants.crushedStateName)
            {
                fallerBehavior.gameObject.transform.position = new Vector3(fallerBehavior.gameObject.transform.position.x, fallerBehavior.gameObject.transform.position.y + 0.1f, fallerBehavior.gameObject.transform.position.z);
                fallerBehavior.gameObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, 0.0001f);
            }
        }else if(collisionType == PlayerFallerCollisionType.Top)
        {
            if(faller.GetComponent<Rigidbody2D>().bodyType == RigidbodyType2D.Dynamic)
            {
                faller.GetComponent<Rigidbody2D>().linearVelocity = faller.GetComponent<Rigidbody2D>().linearVelocity / 2f; //new Vector2(0.0f, 0.0001f);
            }
            playerController.rideFaller(faller);
        }
        else if (collisionType == PlayerFallerCollisionType.Left || collisionType == PlayerFallerCollisionType.Right)
        {
            //playerController.BounceOff(faller, collisionType);
        }
    }
    
    public void TakeDamage(float amount)
    {
        if (!unlimitedLives)
            playerLives = Mathf.Max(0f, playerLives - amount);
        UpdateLifeUI();
        if (playerLives <= 0f)
        {
            playerLives = Constants.maxPlayerLives;
            playerStamina = Constants.maxPlayerStamina;
            UpdateLifeUI();
            UpdateStaminaUI();
            Print("GAME OVER", 4);
            GameOver("You ran out of lives!");
        }
    }
    public bool HasStamina(float amount)
    {
        return playerStamina >= amount;
    }
    public bool UseStamina(float amount)
    {
        if (playerStamina < amount) return false;
        playerStamina -= amount;
        UpdateStaminaUI();
        return true;
    }
    private void UpdateStaminaRegen()
    {
        if (playerController == null) return;
        bool isMoving = Mathf.Abs(playerController.GetComponent<Rigidbody2D>().linearVelocity.x) > 0.5f;
        if (!isMoving && playerStamina < Constants.maxPlayerStamina)
        {
            playerStamina = Mathf.Min(Constants.maxPlayerStamina, playerStamina + Constants.staminaRegenRate * Time.deltaTime);
            UpdateStaminaUI();
        }
    }
    private void UpdateLifeUI()
    {
        if (lifeCounter != null)
            lifeCounter.text = playerLives.ToString("F1");
        if (lifeBarFill != null)
            lifeBarFill.fillAmount = playerLives / Constants.maxPlayerLives;
    }
    private void UpdateStaminaUI()
    {
        if (staminaBarFill != null)
            staminaBarFill.fillAmount = playerStamina / Constants.maxPlayerStamina;
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
        TimeManager.Instance.ResetTime();
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
            saveName = DateTime.Now.ToString("yyyyMMddHHmm");
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
    public void QuickSaveLevel()
    {
        string saveName = DateTime.Now.ToString("yyyyMMddHHmm");
        FallerManager.instance().SaveFallersToFileOverwrite(playerController, saveName);
    }
    public void StartSaveSession()
    {
        string saveName = PlayerPrefs.GetString("SessionSaveFile");
        FallerManager.instance().SaveFallersToFileOverwrite(playerController, saveName);
        
    }
    public void UpdateSaveSession()
    {
        string saveName = PlayerPrefs.GetString("SessionSaveFile");
        FallerManager.instance().SaveFallersToFileOverwrite(playerController, saveName);
    }
    public void ClearSaveSession()
    {
        string saveName = PlayerPrefs.GetString("SessionSaveFile");
        string savePath = Constants.saveFilePath+ "Save_" + saveName + ".json";
        string playerSavePath = Constants.playerDataSavePath + "PlayerSave_" + saveName + ".json";
        string fallerSavePath = Constants.fallerDataSavePath + "FallerSave_" + saveName + ".json";
        if(File.Exists(savePath))
        {
            File.Delete(savePath);
        }
        if (File.Exists(playerSavePath))
        {
            File.Delete(playerSavePath);
        }
        if (File.Exists(fallerSavePath))
        {
            File.Delete(fallerSavePath);
        }
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
        ClearSaveSession();
    }
    public void QuitGame()
    {
        Time.timeScale = 1f;
        UpdateSaveSession();
        SceneManager.LoadScene("MainMenu");
        TimeManager.Instance.ResetTime();
    }
    public void givePlayerTime()
    {
        TimeBetweenSpawns += currentTimeBetweenSpawns;
    }

    public void SpawnFallerAtClick(Vector3 clickPosition)
    {
        if (clickSpawnCooldown > 0 || !clickToSpawn)
        {
            return; // Prevent spawning if cooldown is active
        }
        clickSpawnCooldown = 0.5f; // Reset cooldown
        clickPosition.z = 20f; // Set z to a positive value to ensure it's in front of the camera
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(clickPosition);
        //GameManager.instance().Print("Spawning faller at: " + worldPosition + " from click position: " + clickPosition);
        worldPosition.z = 0f; // Set z to 0 for 2D
        FallerManager.instance().ForceSpawnFaller(worldPosition.y, worldPosition.x, TestingFallerSpawnSizeOptions[(int)TestingFallerSpawnSize], Constants.maxFallerSpeed, false);
        //FallerManager.instance().SpawnFallerAtPosition(worldPosition, Constants.defaultFallerSize);
    }
    public void Print(string message, int level = 0)
    {
        if (message != null && verboseSettings[level])
        {
            if(level == 0)
            {
                Debug.Log("<color=cyan>" + message + "</color>");
            }
            else if(level == 1)
            {
                Debug.Log("<color=magenta>" + message + "</color>");
            }
            else if(level == 2)
            {
                Debug.Log("<color=yellow>" + message + "</color>");
            }
            else if(level == 3)
            {
                Debug.Log("<color=teal>" + message + "</color>");
            }
            else if(level == 4)
            {
                Debug.Log("<color=green>" + message + "</color>");
            } else if(level == 5)
            {
                Debug.Log("<color=blue>" + message + "</color>");
            }else if(level == 6)
            {
                Debug.Log("<color=orange>" + message + "</color>");
            }
             else if (level == 7)
            {
                Debug.Log("<color=purple>" + message + "</color>");
            }
            else if (level == 8)
            {
                Debug.Log("<color=black>" + message + "</color>");
            }
            else
            {
                Debug.Log(message);
            }
        }
    }
    public void StartEMT()
    {
        if (playerLives <= 10)
        {
            Print("Not applying unfreeze impulse to fallers because player is on their last life", 4);
            EMT.instance().EMTOnOneLife();
            return; // Don't apply impulse if player is on their last life to avoid potential softlock
        }
        if (EMT_timer > 0)
        {
            Print("Not applying unfreeze impulse to fallers because player is already in EMT", 4);
            return; // Don't apply impulse if already in EMT
        }
        GameManager.instance().Print("Applying EMT unfreeze impulse to fallers", 4);
        TakeDamage(Constants.EMTLifeCost);
        EMT.instance().EMTMe(player.transform.position);
        EMT_timer = EMT_duration;
        Time.timeScale = 0f;
    }
    public void SetEMT()
    {
        
        Time.timeScale = 1f;
        GameManager.instance().Print("Attempting to apply unfreeze impulse to fallers", 4);
        
        Vector3 playerPosition = player.transform.position;
        FallerManager.instance().UnfreezeImpulse(playerPosition);
    }
    public void StartFallerEMT()
    {
        if(EMT_timer > 0)
        {
            return; // Don't apply impulse if already in EMT
        }
        else
        {
            EMT_timer = EMT_duration;
        }
    }
    public void StartFallerEMT(float EMTDuration)
    {
        if (EMT_timer > 0)
        {
            return; // Don't apply impulse if already in EMT
        }
        else
        {
            EMT_timer = EMTDuration;
        }
    }
    public void ImpulsePlayer(FallerController faller)
    {
        if(Mathf.Abs((player.transform.position - faller.transform.position).magnitude) <= Constants.EMT_Radius)
        {
            if(playerController.state.getName() == Constants.ridingFallerStateName)
            {
                if(faller.IsRidingMe(player.transform.position))
                {
                    playerController.setState(playerController.GetStateFromName(Constants.fallingStateName));
                }
            }
            playerController.GetBombed(faller.transform.position);
            TakeDamage(Constants.EMTLifeCost);
        }
    }
    public float GetPlayerLives()
    {
        return playerLives;
    }
    public void SetPlayerLives(float lives)
    {
        playerLives = lives;
        UpdateLifeUI();
    }
    /*public BoxCollider2D GetPlayerCollider()
    {
        return player.GetComponent<BoxCollider2D>();
    }*/
}
