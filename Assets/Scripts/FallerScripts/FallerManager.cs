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
    public int verbosity = 1; // Set to 1 to enable debug prints for rescue spawns and column checks
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
    public void SpawnFaller(float baseSpawnHeight, bool rescueFaller = false)
    {
        // Remove stale entries from fallers that self-destroyed off-screen
        CleanupDestroyedFallers();

        // Ensure new faller spawns at least minSpawnGap above the highest existing one
        float highestY = GetHighestFallerY();
        if(GetHighestFrozenFallerY() >= trapDoorHeight)
        {
            GameManager.instance().Print("Highest frozen faller is above trapdoor, You Lose");
            GameManager.instance().GameOver("Highest frozen faller is above trapdoor");
            return;
        }
        float spawnHeight = Mathf.Max(baseSpawnHeight, highestY + minSpawnGap);
        // Never spawn above the trapdoor
        spawnHeight = Mathf.Min(spawnHeight, trapDoorHeight);



        float randomX = GetSpawnXPos();
        
        
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
        if (rescueFaller)
        {
            SpriteRenderer sr = fallerBehavior.gameObject.GetComponent<SpriteRenderer>();
            sr.enabled = true;
            sr.color = new Color(0f, 1f, 0f, 0.098f);
            sr.sortingOrder = 2;
        }
    }
    public FallerController ForceSpawnFaller(float spawnHeight, float spawnX, Vector2 spawnSize, float speed = Constants.maxFallerSpeed ,bool rescueFaller = true)
    {
        // Remove stale entries from fallers that self-destroyed off-screen
        CleanupDestroyedFallers();

        float randomX = spawnX;


        float randomSizeX = spawnSize.x;
        float randomSizeY = spawnSize.y;
        if (_fallerType == FallerType.Block)
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
        fallerBehavior.Init(spawnPosition, size, speed, fallerBehavior.gameObject);
        fallersInPlay.Add(nameOfFaller, fallerBehavior);
        if (rescueFaller)
        {
            SpriteRenderer sr = fallerBehavior.gameObject.GetComponent<SpriteRenderer>();
            sr.enabled = true;
            sr.color = new Color(0f, 1f, 0f, 0.098f);
            sr.sortingOrder = 2;
        }
        return fallerBehavior;
    }

    public float GetSpawnXPos()
    {
        //Regular uniform random spawn between walls.
        return Random.Range(Constants.minX, Constants.maxX);
        //Should be a random X position between the min and max, but with a normal distribution centered in the middle to make it more likely to spawn towards the center
        //return RandomGaussian(Constants.minX, Constants.maxX);
    }

    public float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        float val = std * sigma + mean;
        if(val < minValue || val > maxValue)
        {
            Debug.LogWarning($"Generated value {val} is out of bounds [{minValue}, {maxValue}]. Clamping to bounds.");
            val = RandomGaussian(minValue, maxValue); // Regenerate if out of bounds
        }
        return val;
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
        fc.Init(data.position, data.size, -data.currentSpeed, fc.gameObject);
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
    private string UniqueFilePath(string directory, string prefix, string baseName, string suffix)
    {
        string path = directory + prefix + baseName + suffix;
        if (!File.Exists(path)) return path;
        int i = 1;
        while (File.Exists(directory + prefix + baseName + "_" + i + suffix))
            i++;
        return directory + prefix + baseName + "_" + i + suffix;
    }
    public void SaveFallersToFile(PlayerController playerController, string saveName = null)
    {
        bool isNamedSave = !string.IsNullOrEmpty(saveName);
        string baseName = isNamedSave ? saveName : DateTime.Now.ToString("yyyyMMddHHmm");

        if (!isNamedSave && lastSpawnedFallerNumber == numberOfSpawns)
        {
            GameManager.instance().Print("No new fallers to save since last save.");
            return;
        }
        GameManager.instance().Print("Saving faller data to file...");
        lastSpawnedFallerNumber = numberOfSpawns;

        FallerDataList fallerDataList = new FallerDataList();
        foreach (var kvp in fallersInPlay)
        {
            if (kvp.Value == null) continue;
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

        string NewFallerFileSave = UniqueFilePath(Constants.fallerDataSavePath, "FallerSave_", baseName, ".json");
        string NewPlayerFileSave = UniqueFilePath(Constants.playerDataSavePath, "PlayerSave_", baseName, ".json");
        string NewSaveFile = UniqueFilePath(Constants.saveFilePath, "Save_", baseName, ".json");

        string fallerJson = JsonUtility.ToJson(fallerDataList, true);
        File.WriteAllText(NewFallerFileSave, fallerJson);
        if (File.Exists(FallerDirectory)) File.Delete(FallerDirectory);
        File.WriteAllText(FallerDirectory, fallerJson);

        string playerJson = JsonUtility.ToJson(playerController.GetMyData(), true);
        File.WriteAllText(NewPlayerFileSave, playerJson);
        if (File.Exists(PlayerDirectory)) File.Delete(PlayerDirectory);
        File.WriteAllText(PlayerDirectory, playerJson);

        File.WriteAllText(NewSaveFile, JsonUtility.ToJson(new SaveData { playerDataFileRef = NewPlayerFileSave, fallerDataFileRef = NewFallerFileSave, levelScene = SceneManager.GetActiveScene().name }, true));
        GameManager.instance().Print($"Saved {fallerDataList.fallers.Count} fallers to {NewSaveFile}");
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
            GameManager.instance().Print($"Loading faller {data.name} at position {data.position} with size {data.size}, speed {data.currentSpeed}, frozen: {data.isFrozen}, being ridden: {data.beingRidden}");
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
                GameManager.instance().Print($"Loading faller {data.name} at position {data.position} with size {data.size}, speed {data.currentSpeed}, frozen: {data.isFrozen}, being ridden: {data.beingRidden}");
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
    public float GetHighestFrozenFallerY()
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
    public FallerController GetLowestReachableFaller(Vector3 playerPos, Vector3 reachableDistance)
    {
        //string reachableFallers = "Reachable fallers: ";
        FallerController lowestReachable = null;
        float lowestY = float.PositiveInfinity;
        List<FallerController> exposedFallers = GetExposedTopSurfaces();
        foreach (var faller in exposedFallers)
        {
            if (faller == null) continue;
            float topY = faller.transform.position.y + (faller.transform.localScale.y / 2);
            float dy = topY - playerPos.y;
            if (dy <= 0f) 
            { 
                //faller.RemoveRedTint(); 
                continue; 
            }

            float fallerLeft  = faller.transform.position.x - faller.transform.localScale.x / 2;
            float fallerRight = faller.transform.position.x + faller.transform.localScale.x / 2;
            bool playerUnderFaller = playerPos.x > fallerLeft && playerPos.x < fallerRight;

            float dx;
            if (playerUnderFaller)
            {
                // Player is trapped under this faller; they must walk to an edge before jumping on top.
                // Use distance to nearest edge so the ellipse check accounts for horizontal travel needed.
                dx = Mathf.Min(playerPos.x - fallerLeft, fallerRight - playerPos.x);
            }
            else
            {
                dx = Mathf.Max(0f, Mathf.Max(fallerLeft - playerPos.x, playerPos.x - fallerRight));
            }

            // Ellipse check: rectangular bounds overestimate reach at corners; this matches the actual jump arc
            float ellipse = (dx / reachableDistance.x) * (dx / reachableDistance.x)
                          + (dy / reachableDistance.y) * (dy / reachableDistance.y);
            if (ellipse > 1f) 
            { 
                //faller.RemoveRedTint(); 
                continue; 
            }

            // Raycast for obstructions
            if (dx > 0f)
            {
                Vector2 rayDir;
                Vector3 rayOrigin;
                if (playerUnderFaller)
                {
                    // Check the player can walk horizontally to the nearest edge
                    bool nearestEdgeIsLeft = (playerPos.x - fallerLeft) <= (fallerRight - playerPos.x);
                    rayDir = nearestEdgeIsLeft ? Vector2.left : Vector2.right;
                    rayOrigin = playerPos;
                }
                else
                {
                    rayDir = playerPos.x < faller.transform.position.x ? Vector2.right : Vector2.left;
                    rayOrigin = new Vector3(playerPos.x, playerPos.y + dy + 0.1f, 0f);
                }
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, dx, LayerMask.GetMask("Fallers"));
                if (hit.collider != null && hit.collider.gameObject != faller.gameObject)
                {
                    //faller.RemoveRedTint();
                    continue;
                }
            }

            //faller.AddRedTint();
            if (topY < lowestY)
            {
                lowestY = topY;
                lowestReachable = faller;
            }
        }
        //GameManager.instance().Print(reachableFallers, verbosity);
        return lowestReachable;
    }
    
    private List<FallerController> GetExposedTopSurfaces()
    {
        List<FallerController> exposedFallers = new List<FallerController>();
        foreach (var kvp in fallersInPlay)
        {
            if (kvp.Value == null) continue;
            float topY = kvp.Value.transform.position.y + (kvp.Value.transform.localScale.y / 2);
            bool isExposed = true;
            foreach (var otherKvp in fallersInPlay)
            {
                if (otherKvp.Value == null || otherKvp.Key == kvp.Key) continue;
                float otherBottomY = otherKvp.Value.transform.position.y - (otherKvp.Value.transform.localScale.y / 2);
                
                //float otherLeftX = otherKvp.Value.transform.position.x - (otherKvp.Value.transform.localScale.x / 2);
                //float otherRightX = otherKvp.Value.transform.position.x + (otherKvp.Value.transform.localScale.x / 2);
                if (otherBottomY >= topY - 0.1f && // Allow small tolerance
                    otherBottomY <= topY + 0.1f &&
                    kvp.Value.transform.position.x >= (otherKvp.Value.transform.position.x - (otherKvp.Value.transform.localScale.x / 2)) &&
                    kvp.Value.transform.position.x <= (otherKvp.Value.transform.position.x + (otherKvp.Value.transform.localScale.x / 2)))
                {
                    isExposed = false;
                    //kvp.Value.RemoveRedTint(); // Not exposed, remove any tint in case it was previously marked as reachable
                    break;
                }
            }
            if (isExposed)
            {
                exposedFallers.Add(kvp.Value);
            }
        }
        //GameManager.instance().Print($"Exposed fallers: {string.Join(", ", exposedFallers.ConvertAll(f => f.name))}", verbosity);
        return exposedFallers;
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
        GameManager.instance().Print("Checking for faller being ridden...");
        foreach (var kvp in fallersInPlay)
        {
            if (kvp.Value != null && kvp.Value.BeingRidden)
            {
                GameManager.instance().Print($"Faller being ridden: {kvp.Key}");
                return kvp.Value;
            }
        }
        GameManager.instance().Print("No faller is currently being ridden.");
        return null;
    }

    public FallerController SpawnRescue(Vector3 playerPosition, float spawnHeight)
    {
        float farLeftXBound = Mathf.Max(Constants.minXRescueSpawn, playerPosition.x - Constants.maxXJumpDistance);
        float farRightXBound = Mathf.Min(Constants.maxXRescueSpawn, playerPosition.x + Constants.maxXJumpDistance);
        GameManager.instance().Print("Triggering rescue spawn! from " + farLeftXBound + " to " + farRightXBound, verbosity);
        for (float x = playerPosition.x; x <= farRightXBound; x += 0.5f)
        {
            if (IsColumnClear(x, 0.55f, playerPosition.y, spawnHeight + 2f))
            {
                FallerController f = ForceSpawnFaller(spawnHeight, x, new Vector2(0.5f, 3f), 3.0f, true);
                if (f != null){
                    GameManager.instance().Print("Rescue spawn successful!", verbosity);
                    return f;
                }
            }
        }
        for (float x = playerPosition.x; x >= farLeftXBound; x -= 0.5f)
        {
            if (IsColumnClear(x, 0.55f, playerPosition.y, spawnHeight + 2f))
            {
                FallerController f = ForceSpawnFaller(spawnHeight, x, new Vector2(0.5f, 3f), 3.0f, true);
                if (f != null)
                {
                    GameManager.instance().Print("Rescue spawn successful!", verbosity);
                    return f;
                }
            }
        }
        return null;
    }    
    private bool IsColumnClear(float x, float fallerWidth, float fromY, float toY)
    {
        RaycastHit2D left = Physics2D.Raycast(new Vector2(x - (fallerWidth / 2f), fromY), Vector2.up, toY - fromY, LayerMask.GetMask("Fallers"));
        RaycastHit2D right = Physics2D.Raycast(new Vector2(x + (fallerWidth / 2f), fromY), Vector2.up, toY - fromY, LayerMask.GetMask("Fallers"));
        if(left.collider != null)
        {
            GameManager.instance().Print($"Column check at x={x}: Hit {left.collider.gameObject.name} on the left side", verbosity);
        }
        else
        {
            GameManager.instance().Print($"Column check at x={x}: No hit on the left side", verbosity);
        }
        if (right.collider != null)
        {
            GameManager.instance().Print($"Column check at x={x}: Hit {right.collider.gameObject.name} on the right side", verbosity);
        }
        else
        {
            GameManager.instance().Print($"Column check at x={x}: No hit on the right side", verbosity);
        }
        if (left.collider != null || right.collider != null)
        {
            return false; // There's an obstacle in the way, column is not clear
        }
        /*foreach (var kvp in fallersInPlay)
        {
            if (kvp.Value == null) continue;
            float otherLeftX = kvp.Value.transform.position.x - (kvp.Value.transform.localScale.x / 2);
            float otherRightX = kvp.Value.transform.position.x + (kvp.Value.transform.localScale.x / 2);
            float otherBottomY = kvp.Value.transform.position.y - (kvp.Value.transform.localScale.y / 2);
            float otherTopY = kvp.Value.transform.position.y + (kvp.Value.transform.localScale.y / 2);
            bool overlapsX = x + (fallerWidth / 2) > otherLeftX && x - (fallerWidth / 2) < otherRightX;
            bool overlapsY = toY > otherBottomY && fromY < otherTopY;
            if (overlapsX && overlapsY)
            {
                return false; // Column is not clear
            }
        }*/
        return true; // Column is clear
    }
    public void RemoveAllTints()
    {
        foreach (var kvp in fallersInPlay)
        {
            if (kvp.Value == null) continue;
            kvp.Value.RemoveTint();
        }
    }
    public List<FallerController> GetFallersInRadius(Vector3 position, float radius)
    {
        List<FallerController> fallersInRadius = new List<FallerController>();
        Collider2D[] collisions = Physics2D.OverlapCircleAll(position, radius, LayerMask.GetMask("Fallers"));
        foreach (var collision in collisions)
        {
            if (collision == null) continue;
            FallerController faller = collision.GetComponent<FallerController>();
            if (faller != null)
            {
                fallersInRadius.Add(faller);
            }
        }
        return fallersInRadius;
    }
    public List<FallerController> GetFallersOutRadius(Vector3 position, float radius)
    {
        List<FallerController> fallersOutRadius = new List<FallerController>();
        Collider2D[] collisions = Physics2D.OverlapCircleAll(position, radius, LayerMask.GetMask("Fallers"));
        List<string> collisionNames = new List<string>(collisions.Length);
        for (int i = 0; i < collisions.Length; i++)
        {
            collisionNames.Add(collisions[i] != null ? collisions[i].gameObject.name : "null");
        }
        foreach (var kvp in fallersInPlay)
        {
            if (kvp.Value == null) continue;
            FallerController faller = kvp.Value.GetComponent<FallerController>();
            if (faller != null && !collisionNames.Contains(kvp.Key))
            {
                fallersOutRadius.Add(faller);
            }
        }
        return fallersOutRadius;
    }
    public void UnfreezeImpulse(Vector3 playerPosition)
    {
        List<FallerController> fallersInRadius = GetFallersInRadius(playerPosition, Constants.EMT_Radius);
        List<FallerController> fallersOutRadius = GetFallersOutRadius(playerPosition, Constants.EMT_Radius);
        
        foreach (FallerController faller in fallersOutRadius)
        {
            faller.Unfreeze();
            //faller.AddTint(new Color(1f, 0f, 0f), 0.098f);
        }
        foreach (FallerController faller in fallersInRadius) 
        {
            faller.Unfreeze();
            Vector3 direction = (faller.transform.position - playerPosition);
            faller.AddImpulse(new Vector2(direction.x, direction.y));
            //faller.AddTint(new Color(0f, 0f, 1f), 0.098f);
        }
    }
}