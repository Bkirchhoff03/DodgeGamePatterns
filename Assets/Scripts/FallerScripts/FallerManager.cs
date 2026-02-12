using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class FallerManager
{
    static FallerManager instance_;
    
    int numberOfSpawns = 0;
    float fallerSpeed = 2.0f;
    public static FallerManager instance() => instance_;
    public void init()
    {
        instance_ = this;
    }
    public KeyValuePair<string, FallerController> CreateFaller(Sprite sprite, float spawnHeight)
    {
        Debug.Log("Spawning at: " + spawnHeight);
        float randomX = UnityEngine.Random.Range(Constants.minX, Constants.maxX);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0);
        numberOfSpawns++;
        string nameOfFaller = Constants.fallerNamePrefix + numberOfSpawns.ToString();
        GameObject fallerObject = new GameObject(nameOfFaller);
        fallerObject.AddComponent<FallerController>();
        fallerObject.AddComponent<FallerCollisionHandler>();
        FallerController fallerBehavior = fallerObject.GetComponent<FallerController>();
        
        fallerBehavior.Init(spawnPosition, new Vector3(
            Random.Range(Constants.minFallerSize, Constants.maxFallerSize),
            Random.Range(Constants.minFallerSize, Constants.maxFallerSize), Constants.minFallerSize),
            Random.Range(Constants.minFallerSpeed, Constants.maxFallerSpeed), sprite, fallerObject);
        KeyValuePair<string, FallerController> keyValue = new KeyValuePair<string, FallerController>(nameOfFaller, fallerBehavior);
        return keyValue;
    }
    public void ResetSpawns()
    {
        numberOfSpawns = 0;
    }

}
