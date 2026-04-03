using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

public class FallerManager
{
    public enum FallerType { Block, Boulder }
    static FallerManager instance_;
    [System.Serializable]
    public class FallerData
    {
        public string name;
        public Vector3 position;
        public Vector3 size;
        public float rotation;
        public float currentSpeed;
        public bool isFrozen;
        public bool beingRidden;
        public FallerType fallerType;
    }
    [System.Serializable]
    public class FallerDataList
    {
        public List<FallerData> fallers;
        public FallerDataList() { 
            fallers = new List<FallerData>();
        }
    }
    [System.Serializable]
    public class SaveData
    {
        public string playerDataFileRef;
        public string fallerDataFileRef;
        public string levelScene;
    }
    private enum FallerSize
    {
        HALF = 1,
        ONE = 2,
        ONE_AND_HALF = 3,
        TWO = 4,
        TWO_AND_HALF = 5,
        THREE = 6
    }

    int numberOfSpawns = 0;
    FallerType _fallerType;
    Sprite sprite;
    float trapDoorHeight;
    int lastSpawnedFallerNumber = 0;
    // Tracks all active fallers by name; cleaned up each spawn cycle
    public Dictionary<string, FallerController> fallersInPlay = new Dictionary<string, FallerController>();
    private readonly string FallerDirectory = Constants.fallerDataSavePath + GameManager.instance().currentFallerSaveFileName;
    private readonly string PlayerDirectory = Constants.playerDataSavePath + GameManager.instance().currentPlayerSaveFileName;


    // Minimum vertical distance above the highest existing faller before spawning a new one
    const float minSpawnGap = 5.0f;

    public static FallerManager instance() => instance_;
    public void init(FallerType fallerType, float trapDoorHeight)
    {
        instance_ = this;
        _fallerType = fallerType;
        this.trapDoorHeight = trapDoorHeight;
    }
    /*public void init(Sprite sprite, float trapDoorHeight)
    {
        instance_ = this;
        this.sprite = sprite;
        this.trapDoorHeight = trapDoorHeight;
    }*/

    // Spawns a new faller at a safe height above existing fallers.
    // baseSpawnHeight is the camera-relative default; actual height is raised
    // if any existing faller is within minSpawnGap, capped at the trapdoor.
    public void SpawnFaller(float baseSpawnHeight)
    {
        // Remove stale entries from fallers that self-destroyed off-screen
        CleanupDestroyedFallers();

        // Ensure new faller spawns at least minSpawnGap above the highest existing one
        float highestY = GetHighestFallerY();
        if(GetHighestFrozenFallerY() >= trapDoorHeight)
        {
            Debug.Log("Highest frozen faller is above trapdoor, You Lose");
            GameManager.instance().GameOver("Highest frozen faller is above trapdoor");
            return;
        }
        float spawnHeight = Mathf.Max(baseSpawnHeight, highestY + minSpawnGap);
        // Never spawn above the trapdoor
        spawnHeight = Mathf.Min(spawnHeight, trapDoorHeight);

        float randomX = Random.Range(Constants.minX, Constants.maxX);
        float randomSizeX = Random.Range(Constants.minFallerSize, Constants.maxFallerSize);
        float randomSizeY = Random.Range(Constants.minFallerSize, Constants.maxFallerSize);
        if(_fallerType == FallerType.Block)
        {
            randomSizeX = Mathf.Round(randomSizeX * 2f) / 2f; // Round to nearest 0.5
            randomSizeY = Mathf.Round(randomSizeY * 2f) / 2f; // Round to nearest 0.5
        }
        numberOfSpawns++;
        string nameOfFaller = Constants.fallerNamePrefix + numberOfSpawns.ToString();
        //GameObject fallerObject = new GameObject(nameOfFaller);
        Vector3 spawnPosition = new Vector3(randomX, spawnHeight, 0);
        Vector3 size = new Vector3(randomSizeX, randomSizeY, Constants.minFallerSize);

        FallerController fallerBehavior = CreateFaller(nameOfFaller, _fallerType, size);
        fallerBehavior.Init(spawnPosition, size, Random.Range(Constants.minFallerSpeed, Constants.maxFallerSpeed), fallerBehavior.gameObject);
        fallersInPlay.Add(nameOfFaller, fallerBehavior);

        /*string randomXSizeName = randomSizeX.ToString("0.#");
        string randomYSizeName = randomSizeY.ToString("0.#");
        Debug.Log($"Spawning faller with size {randomXSizeName}_by_{randomYSizeName} at position {spawnPosition}");
        GameObject fallerObject = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + randomXSizeName + "_by_" + randomYSizeName));
        fallerObject.layer = LayerMask.NameToLayer("Fallers");
        fallerObject.name = nameOfFaller;
        //fallerObject.AddComponent<FallerController>();
        //fallerObject.AddComponent<FallerCollisionHandler>();

        FallerController fallerBehavior = fallerObject.GetComponent<FallerController>();

        fallerBehavior.Init(spawnPosition, new Vector3(randomSizeX, randomSizeY, Constants.minFallerSize),
            Random.Range(Constants.minFallerSpeed, Constants.maxFallerSpeed), sprite, fallerObject);

        fallersInPlay.Add(nameOfFaller, fallerBehavior);*/
    }

    public void SpawnFallerAtPosition(Vector3 worldPosition)
    {
        numberOfSpawns++;
        string nameOfFaller = Constants.fallerNamePrefix + numberOfSpawns.ToString();
        float randomSizeX = Random.Range(Constants.minFallerSize, Constants.maxFallerSize);
        float randomSizeY = Random.Range(Constants.minFallerSize, Constants.maxFallerSize);
        Vector3 size = new Vector3(randomSizeX, randomSizeY, Constants.minFallerSize);
        FallerController fc = CreateFaller(nameOfFaller, _fallerType, size);
        fc.Init(worldPosition, size, Random.Range(Constants.minFallerSpeed, Constants.maxFallerSpeed), fc.gameObject);
        fallersInPlay.Add(nameOfFaller, fc);

        /*GameObject fallerObject = new GameObject(nameOfFaller);
        fallerObject.AddComponent<FallerController>();
        fallerObject.AddComponent<FallerCollisionHandler>();
        FallerController fallerBehavior = fallerObject.GetComponent<FallerController>();

        

        fallerBehavior.Init(worldPosition, new Vector3(randomSizeX, randomSizeY, Constants.minFallerSize),
            Random.Range(Constants.minFallerSpeed, Constants.maxFallerSpeed), sprite, fallerObject);

        fallersInPlay.Add(nameOfFaller, fallerBehavior);*/
    }
    public void SpawnFallerAtPosition(Vector3 worldPosition, Vector3 size)
    {
        numberOfSpawns++;
        string nameOfFaller = Constants.fallerNamePrefix + numberOfSpawns.ToString();
        FallerController fc = CreateFaller(nameOfFaller, _fallerType, size);
        fc.Init(worldPosition, size, Random.Range(Constants.minFallerSpeed, Constants.maxFallerSpeed), fc.gameObject);
        fallersInPlay.Add(nameOfFaller, fc);

        /*        GameObject fallerObject = new GameObject(nameOfFaller);
        fallerObject.AddComponent<FallerController>();
        fallerObject.AddComponent<FallerCollisionHandler>();
        FallerController fallerBehavior = fallerObject.GetComponent<FallerController>();
        fallerBehavior.Init(worldPosition, size,
            Random.Range(Constants.minFallerSpeed, Constants.maxFallerSpeed), sprite, fallerObject);
        fallersInPlay.Add(nameOfFaller, fallerBehavior);*/
    }
    private void SpawnFallerAtData(FallerData data)
    {
        numberOfSpawns++;
        string nameOfFaller = Constants.fallerNamePrefix + numberOfSpawns.ToString();
        FallerController fc = CreateFaller(nameOfFaller, _fallerType, data.size);
        fc.Init(data.position, data.size, data.currentSpeed, fc.gameObject);
        fc.gameObject.transform.rotation = Quaternion.Euler(0f, 0f, data.rotation);
        // If the faller was frozen when saved, freeze it again after spawning
        if (data.isFrozen)
        {
            fc.FloorPause();
        }   
        if(data.beingRidden)
        {
            fc.StartRiding();
            //GameManager.instance().player.GetComponent<PlayerController>().rideFaller(fallerObject);
        }
        fallersInPlay.Add(nameOfFaller, fc);
    }
    // Creates the right GameObject + components + behaviour based on FallerType
    private FallerController CreateFaller(string name, FallerType type, Vector3 size)
    {
        GameObject fallerObject;
        FallerController fc;

        if (type == FallerType.Block)
        {
            string xName = Mathf.Round(size.x * 2f) / 2f == size.x
                ? size.x.ToString("0.#") : size.x.ToString("0.#");
            string yName = size.y.ToString("0.#");
            fallerObject = GameObject.Instantiate(
                Resources.Load<GameObject>("Prefabs/" + xName + "_by_" + yName));
            fallerObject.layer = LayerMask.NameToLayer("Fallers");
            fallerObject.name = name;
            fc = fallerObject.GetComponent<FallerController>();
        }
        else
        {
            fallerObject = new GameObject(name);
            fallerObject.layer = LayerMask.NameToLayer("Fallers");
            fallerObject.AddComponent<FallerController>();
            fallerObject.AddComponent<FallerCollisionHandler>();
            fc = fallerObject.GetComponent<FallerController>();
        }

        IFallerBehavior behaviour = type == FallerType.Block
            ? (IFallerBehavior)new BlockFallerBehavior()
            : new BolderFallerBehavior();
        fc.SetBehaviour(behaviour);
        return fc;
    }
    public void SaveFallersToFile(PlayerController playerController)
    {
        string dateTime = DateTime.Now.ToString("yyyyMMddHHmm");
        if (lastSpawnedFallerNumber == numberOfSpawns)
        {
            Debug.Log("No new fallers to save since last save.");
            return;
        }
        Debug.Log("Saving faller data to file...");
        lastSpawnedFallerNumber = numberOfSpawns;
        // Serialize faller data to JSON
        FallerDataList fallerDataList = new FallerDataList();
        foreach (var kvp in fallersInPlay)
        {
            if (kvp.Value == null) continue; // Skip destroyed fallers
            FallerController faller = kvp.Value;
            FallerData data = new FallerData
            {
                name = kvp.Key,
                position = faller.transform.position,
                size = faller.transform.localScale,
                rotation = faller.transform.rotation.eulerAngles.z,
                currentSpeed = faller.gameObject.GetComponent<Rigidbody2D>().linearVelocityY,
                isFrozen = faller.amIFrozen(),
                beingRidden = faller.BeingRidden,
                fallerType = _fallerType
            };
            fallerDataList.fallers.Add(data);
        }
        if(File.Exists(FallerDirectory))
        {
            File.Delete(FallerDirectory); // Clear old data before saving new
        }
        string NewFallerFileSave = Constants.fallerDataSavePath + "FallerSave" + dateTime + ".json";
        if (File.Exists(NewFallerFileSave))
        {
            File.Delete(NewFallerFileSave); // Clear old data before saving new
        }
        string json = JsonUtility.ToJson(fallerDataList, true);
        File.WriteAllText(NewFallerFileSave, json);
        File.WriteAllText(FallerDirectory, json);

        Debug.Log($"Saved {fallerDataList.fallers.Count} fallers to file at {FallerDirectory}");
        
        if (File.Exists(Constants.playerDataSavePath))
        {
            File.Delete(Constants.playerDataSavePath); // Clear old data before saving new
        }
        string NewPlayerFileSave = Constants.playerDataSavePath + "PlayerSave" + dateTime + ".json";
        File.WriteAllText(NewPlayerFileSave, JsonUtility.ToJson(playerController.GetMyData(), true));
        File.WriteAllText(PlayerDirectory, JsonUtility.ToJson(playerController.GetMyData(), true));
        File.WriteAllText(Constants.saveFilePath + "Save" + dateTime + ".json", JsonUtility.ToJson(new SaveData { playerDataFileRef = NewPlayerFileSave, fallerDataFileRef = NewFallerFileSave, levelScene = SceneManager.GetActiveScene().name }, true));
    }
    public void LoadFallersFromFile(PlayerController playerController)
    {
        if (!File.Exists(FallerDirectory))
        {
            Debug.LogWarning("No faller data file found to load.");
            return;
        }
        string json = File.ReadAllText(FallerDirectory);
        FallerDataList fallerDataList = JsonUtility.FromJson<FallerDataList>(json);
        foreach (FallerData data in fallerDataList.fallers) 
        {
            Debug.Log($"Loading faller {data.name} at position {data.position} with size {data.size}, speed {data.currentSpeed}, frozen: {data.isFrozen}, being ridden: {data.beingRidden}");
            SpawnFallerAtData(data);
        }
        if(File.Exists(PlayerDirectory))
        {
            string playerJson = File.ReadAllText(PlayerDirectory);
            PlayerController.PlayerData playerData = JsonUtility.FromJson<PlayerController.PlayerData>(playerJson);
            playerController.SetFromData(playerData);
        }
    }
    public void LoadFallersFromFile(PlayerController playerController, string fileToUse)
    {
        
        if (!File.Exists(fileToUse))
        {
            Debug.LogWarning("No faller data file found to load.");
            return;
        }
        string json = File.ReadAllText(fileToUse);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);
        string FallerFileToUse = saveData.fallerDataFileRef;
        string PlayerFileToUse = saveData.playerDataFileRef;

        if (!File.Exists(FallerFileToUse))
        {
            Debug.LogWarning("No faller data file found to load.");
        }
        else
        {
            string FallerJson = File.ReadAllText(FallerFileToUse);
            FallerDataList fallerDataList = JsonUtility.FromJson<FallerDataList>(FallerJson);
            foreach (FallerData data in fallerDataList.fallers)
            {
                Debug.Log($"Loading faller {data.name} at position {data.position} with size {data.size}, speed {data.currentSpeed}, frozen: {data.isFrozen}, being ridden: {data.beingRidden}");
                SpawnFallerAtData(data);
            }
        }
        if(!File.Exists(PlayerFileToUse))
        {
            Debug.LogWarning("No player data file found to load.");
        }
        else
        {
             string playerJson = File.ReadAllText(PlayerFileToUse);
             PlayerController.PlayerData playerData = JsonUtility.FromJson<PlayerController.PlayerData>(playerJson);
             playerController.SetFromData(playerData);
        }

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
    float GetHighestFrozenFallerY()
    {
        float highest = float.NegativeInfinity;
        foreach (var kvp in fallersInPlay)
        {
            if (kvp.Value == null) continue;
            if (!kvp.Value.amIFrozen()) continue;
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
    public FallerController GetFallerBeingRidden()
    {
        Debug.Log("Checking for faller being ridden...");
        foreach (var kvp in fallersInPlay)
        {
            if (kvp.Value != null && kvp.Value.BeingRidden)
            {
                Debug.Log($"Faller being ridden: {kvp.Key}");
                return kvp.Value;
            }
        }
        Debug.Log("No faller is currently being ridden.");
        return null;
    }
}
