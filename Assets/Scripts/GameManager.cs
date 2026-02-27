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
    FallerManager fallerController;
    float currentTimeBetweenSpawns = 5.0f;
    float TimeBetweenSpawns;
    public Sprite sprite;
    static GameManager instance_;
    private int playerLives = 3;
    public TextMeshProUGUI lifeCounter;
    public GameObject camera;
    public GameObject player;
    public GameObject trapDoor; // Assign the TrapDoor GameObject in the Inspector
    private float spawnHeight = Constants.spawnY;
    private float fallerSpawnCameraDiff;
    private PlayerController playerController;
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
        float trapDoorHeight = trapDoor != null ? trapDoor.GetComponent<TrapDoor>().height : 50.0f;
        fallerController = new FallerManager();
        // FallerManager now owns the faller dictionary, sprite, and spawn height logic
        fallerController.init(sprite, trapDoorHeight);
        playerController = player.GetComponent<PlayerController>();
        fallerSpawnCameraDiff = spawnHeight - camera.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (TimeBetweenSpawns > 0)
        {
            TimeBetweenSpawns -= Time.deltaTime;
        }
        else
        {
            SpawnObject();
            TimeBetweenSpawns = currentTimeBetweenSpawns;
        }
        if(player.transform.position.y > 4.0f)
        {
            camera.transform.position = new Vector3(0.0f, player.transform.position.y, -10.0f);
            spawnHeight = camera.transform.position.y + fallerSpawnCameraDiff;
        }
        else
        {
            camera.transform.position = new Vector3(0.0f, 4.0f, -10.0f);
            spawnHeight = camera.transform.position.y + fallerSpawnCameraDiff;
        }
    }
    public void HandlePlayerFallerCollision(GameObject player, GameObject faller, PlayerFallerCollisionType collisionType)
    {
        Debug.Log("FROM GAME MANAGER: Player " + player.name + " collided with Faller " + faller.name);
        PlayerController playerController = player.GetComponent<PlayerController>();
        FallerController fallerBehavior = faller.GetComponent<FallerController>();
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
            //playerController.bounceOff();
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
    public void givePlayerTime()
    {
        TimeBetweenSpawns += currentTimeBetweenSpawns;
    }
}
