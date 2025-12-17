using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class FallerController
{
    static FallerController instance_;
    
    int numberOfSpawns = 0;
    float fallerSpeed = 2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static FallerController instance() => instance_;
    public void init()
    {
        instance_ = this;
    }
    public KeyValuePair<string, FallerBehavior> CreateFaller(Sprite sprite)
    {
        float randomX = UnityEngine.Random.Range(Constants.minX, Constants.maxX);
        Vector3 spawnPosition = new Vector3(randomX, Constants.spawnY, 0);
        numberOfSpawns++;
        string nameOfFaller = Constants.fallerNamePrefix + numberOfSpawns.ToString();
        GameObject fallerObject = new GameObject(nameOfFaller);
        fallerObject.AddComponent<FallerBehavior>();
        
        FallerBehavior fallerBehavior = fallerObject.GetComponent<FallerBehavior>();
        
        fallerBehavior.Init(spawnPosition, new Vector3(
            Random.Range(Constants.minFallerSize, Constants.maxFallerSize),
            Random.Range(Constants.minFallerSize, Constants.maxFallerSize), Constants.minFallerSize),
            Random.Range(Constants.minFallerSpeed, Constants.maxFallerSpeed), sprite, fallerObject);
        KeyValuePair<string, FallerBehavior> keyValue = new KeyValuePair<string, FallerBehavior>(nameOfFaller, fallerBehavior);
        return keyValue;
    }

}
