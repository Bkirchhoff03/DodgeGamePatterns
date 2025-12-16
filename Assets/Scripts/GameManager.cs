using Assets.Scripts;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    float TimeBetweenSpawns = 5.0f;
    public Sprite sprite;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(TimeBetweenSpawns > 0)
        {
            TimeBetweenSpawns -= Time.deltaTime;
        }
        else
        {
            SpawnObject();
            TimeBetweenSpawns = 5.0f;
        }
    }

    void SpawnObject()
    {
        float randomX = Random.Range(Constants.minX, Constants.maxX);
        Vector3 spawnPosition = new Vector3(randomX, Constants.spawnY, 0);
        FallerController faller = new FallerController();
        faller.init(spawnPosition, new Vector3(Random.Range(Constants.minFallerSize, Constants.maxFallerSize), Random.Range(Constants.minFallerSize, Constants.maxFallerSize), Constants.minFallerSize), Random.Range(Constants.minFallerSpeed, Constants.maxFallerSpeed), sprite);
        
    }
}
