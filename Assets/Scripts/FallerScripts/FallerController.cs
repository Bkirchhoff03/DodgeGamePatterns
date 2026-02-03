using Assets.Scripts;
using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.U2D;

public class FallerController : MonoBehaviour
{
    GameObject fallerObject;
    float fallerSpeed;
    bool isFrozen = false;
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
        fallerObject.AddComponent<BoxCollider2D>();
        fallerObject.GetComponent<BoxCollider2D>().sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        fallerObject.AddComponent<Rigidbody2D>();
        fallerObject.GetComponent<Rigidbody2D>().sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        fallerObject.GetComponent<Rigidbody2D>().gravityScale = Constants.gameGravity; // Could set the gravity to random speed sent to this function
        fallerObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
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
    public bool shouldPointDamage(Vector2 collisionPoint)
    {
        bool pointDamages = false;
        Vector2 twoDPos = new Vector2(fallerObject.transform.position.x, fallerObject.transform.position.y);
        Vector2 direction = collisionPoint - twoDPos;
        Vector2 normal = direction.normalized;

        if(collisionPoint.y < fallerObject.transform.position.y)
        {
            pointDamages = true;
        }

        return pointDamages;
    }
    public bool isRidingMe(Vector3 playerPoint)
    {
        float leftBound = gameObject.transform.position.x - (gameObject.transform.localScale.x / 2.0f);
        float rightBound = gameObject.transform.position.x + (gameObject.transform.localScale.x / 2.0f);

        if (playerPoint.x > leftBound && playerPoint.x < rightBound)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void FloorPause()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0.0f, 0.580392157f, 0.0f);
        isFrozen = true;
    }
}
