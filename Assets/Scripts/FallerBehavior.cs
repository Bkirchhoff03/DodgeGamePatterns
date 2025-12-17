using Assets.Scripts;
using System.Drawing;
using UnityEngine;
using UnityEngine.U2D;

public class FallerBehavior : MonoBehaviour
{
    GameObject fallerObject;
    float fallerSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void Init(Vector3 spawnPoint, Vector3 size, float speed, Sprite sprite, GameObject fallerObj)
    {
        fallerObject = fallerObj;
        SpriteRenderer spriteRenderer = fallerObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
        fallerObject.transform.position = spawnPoint;
        fallerObject.transform.localScale = size;
        fallerSpeed = speed;
        fallerObject.AddComponent<BoxCollider2D>();
        fallerObject.GetComponent<BoxCollider2D>().sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        fallerObject.AddComponent<Rigidbody2D>();
        fallerObject.GetComponent<Rigidbody2D>().sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        fallerObject.GetComponent<Rigidbody2D>().gravityScale = 0.1f;
        fallerObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
        //fallerObject = fallerObject;
        fallerSpeed = speed;
    }
    // Update is called once per frame
    void Update()
    {
        if (fallerObject.transform.position.y < -4.0f)
        {
            DeleteMe();
        }
        //fallerObject.transform.position += Vector3.down * Time.deltaTime * fallerSpeed;
    }
    public void DeleteMe()
    {
        Destroy(fallerObject);
        Destroy(this);
    }
}
