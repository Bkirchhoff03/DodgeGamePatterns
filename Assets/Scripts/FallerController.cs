using Assets.Scripts;
using UnityEngine;

public class FallerController
{
    GameObject fallerObject;
    float fallerSpeed = 2.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public void init(Vector3 spawnPoint, Vector3 size, float speed, Sprite sprite)
    {
        fallerObject = new GameObject("Square");
        SpriteRenderer spriteRenderer = fallerObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite; 
        fallerObject.transform.position = spawnPoint;
        fallerObject.transform.localScale = size;
        fallerSpeed = speed;
        fallerObject.AddComponent<BoxCollider2D>();
        fallerObject.GetComponent<BoxCollider2D>().sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        fallerObject.AddComponent<FallerBehavior>().Init(fallerObject, fallerSpeed);
        fallerObject.AddComponent<Rigidbody2D>();
        fallerObject.GetComponent<Rigidbody2D>().sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        fallerObject.GetComponent<Rigidbody2D>().gravityScale = 0.25f;
        //fallerObject.GetComponent<Rigidbody>().
    }
    
}
