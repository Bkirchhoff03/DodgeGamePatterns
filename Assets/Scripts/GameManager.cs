using Assets.Scripts;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GameManager : MonoBehaviour
{
    FallerManager fallerController;
    float currentTimeBetweenSpawns = 5.0f;
    float TimeBetweenSpawns;
    public Sprite sprite;
    private int numberOfSpawns = 0;
    static GameManager instance_;
    private int playerLives = 3;
    public TextMeshProUGUI lifeCounter;
    private Dictionary<string, FallerController> fallersInPlay = new Dictionary<string, FallerController>();
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
        fallerController = new FallerManager();
        fallerController.init();
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
            faller.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0.0f, 0.0001f);
            playerController.rideFaller(faller);
        }
    }
    void SpawnObject()
    {
        KeyValuePair<string, FallerController> faller = FallerManager.instance().CreateFaller(sprite);
        fallersInPlay.Add(faller.Key, faller.Value);

    }
    void DeleteFaller(string nameOfFaller)
    {
        fallersInPlay[nameOfFaller].DeleteMe();
        fallersInPlay.Remove(nameOfFaller);
    }
    public void givePlayerTime()
    {
        TimeBetweenSpawns += currentTimeBetweenSpawns;
    }
}
