using UnityEngine;

public class FallerController
{
    GameObject fallerObject;
    float fallerSpeed = 2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void init(Vector3 spawnPoint, Vector3 size, float speed)
    {
        fallerObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fallerObject.transform.position = spawnPoint;
        fallerObject.transform.localScale = size;
        fallerSpeed = speed;
        fallerObject.AddComponent<FallerBehavior>().Init(fallerObject, fallerSpeed);
    }
    
}
