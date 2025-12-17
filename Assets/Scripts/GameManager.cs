using Assets.Scripts;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GameManager : MonoBehaviour
{
    FallerController fallerController;
    float currentTimeBetweenSpawns = 5.0f;
    float TimeBetweenSpawns;
    public Sprite sprite;
    private int numberOfSpawns = 0;
    static GameManager instance_;
    private int playerLives = 3;
    public TextMeshProUGUI lifeCounter;
    private Dictionary<string, FallerBehavior> fallersInPlay = new Dictionary<string, FallerBehavior>();
    public enum PlayerFallerCollisionType
    {
        Top,
        Bottom,
        Left,
        Right
    }
    public static GameManager instance() => instance_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance_ = this;
        TimeBetweenSpawns = currentTimeBetweenSpawns;
        fallerController = new FallerController();
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
        FallerBehavior fallerBehavior = faller.GetComponent<FallerBehavior>();
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
        }
    }
    void SpawnObject()
    {
        KeyValuePair<string, FallerBehavior> faller = FallerController.instance().CreateFaller(sprite);
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
