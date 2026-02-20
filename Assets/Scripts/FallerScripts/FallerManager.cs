using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class FallerManager
{
    static FallerManager instance_;

    int numberOfSpawns = 0;
    Sprite sprite;
    float trapDoorHeight;
    // Tracks all active fallers by name; cleaned up each spawn cycle
    Dictionary<string, FallerController> fallersInPlay = new Dictionary<string, FallerController>();

    // Minimum vertical distance above the highest existing faller before spawning a new one
    const float minSpawnGap = 5.0f;

    public static FallerManager instance() => instance_;
    public void init(Sprite sprite, float trapDoorHeight)
    {
        instance_ = this;
        this.sprite = sprite;
        this.trapDoorHeight = trapDoorHeight;
    }

    // Spawns a new faller at a safe height above existing fallers.
    // baseSpawnHeight is the camera-relative default; actual height is raised
    // if any existing faller is within minSpawnGap, capped at the trapdoor.
    public void SpawnFaller(float baseSpawnHeight)
    {
        // Remove stale entries from fallers that self-destroyed off-screen
        CleanupDestroyedFallers();

        // Ensure new faller spawns at least minSpawnGap above the highest existing one
        float highestY = GetHighestFallerY();
        float spawnHeight = Mathf.Max(baseSpawnHeight, highestY + minSpawnGap);
        // Never spawn above the trapdoor
        spawnHeight = Mathf.Min(spawnHeight, trapDoorHeight);

        float randomX = Random.Range(Constants.minX, Constants.maxX);
        float randomSizeX = Random.Range(Constants.minFallerSize, Constants.maxFallerSize);
        float randomSizeY = Random.Range(Constants.minFallerSize, Constants.maxFallerSize);

        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0);
        numberOfSpawns++;
        string nameOfFaller = Constants.fallerNamePrefix + numberOfSpawns.ToString();
        GameObject fallerObject = new GameObject(nameOfFaller);
        fallerObject.AddComponent<FallerController>();
        fallerObject.AddComponent<FallerCollisionHandler>();
        FallerController fallerBehavior = fallerObject.GetComponent<FallerController>();

        fallerBehavior.Init(spawnPosition, new Vector3(randomSizeX, randomSizeY, Constants.minFallerSize),
            Random.Range(Constants.minFallerSpeed, Constants.maxFallerSpeed), sprite, fallerObject);

        fallersInPlay.Add(nameOfFaller, fallerBehavior);
    }

    // Destroys a faller and removes it from tracking (e.g. when player is crushed by it)
    public void RemoveFaller(string name)
    {
        if (fallersInPlay.ContainsKey(name))
        {
            fallersInPlay[name].DeleteMe();
            fallersInPlay.Remove(name);
        }
    }

    // Returns the Y position of the highest active faller, used to calculate safe spawn height
    float GetHighestFallerY()
    {
        float highest = float.NegativeInfinity;
        foreach (var kvp in fallersInPlay)
        {
            if (kvp.Value == null) continue;
            float y = kvp.Value.transform.position.y;
            if (y > highest)
            {
                highest = y;
            }
        }
        return highest;
    }

    // Removes null entries left behind when fallers self-destroy after falling off-screen
    void CleanupDestroyedFallers()
    {
        List<string> toRemove = new List<string>();
        foreach (var kvp in fallersInPlay)
        {
            if (kvp.Value == null)
            {
                toRemove.Add(kvp.Key);
            }
        }
        foreach (string key in toRemove)
        {
            fallersInPlay.Remove(key);
        }
    }
}
