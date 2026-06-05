using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public class FallerController : MonoBehaviour
{
    public GameObject FallerObject { get; private set; }
    float fallerSpeed;
    bool isFrozen = false;
    public Vector2 FallerSize;
    public bool BeingRidden {get; private set;}
    // Public read-only access so collision handlers can check if this faller is grounded
    public bool IsFrozen => isFrozen;
    public bool UseSettleTimer => behavior != null && behavior.UseSettleTimer;

    private IFallerBehavior behavior;
    private Rigidbody2D rb;
    private float settleTimer = 0f;
    private int collisionCount = 0;
    private Vector2[] vertices;
    private Dictionary<string, FallerController> fallersCollidingWithMe = new Dictionary<string, FallerController>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BeingRidden = false;
    }
    public void SetBehaviour(IFallerBehavior b) { behavior = b; }
    public void Init(Vector3 spawnPoint, Vector3 size, float speed, GameObject fallerObj)
    {
        FallerObject = fallerObj;
        fallerSpeed = speed;
        FallerSize = (Vector2)size;
        //SpriteRenderer spriteRenderer = fallerObject.AddComponent<SpriteRenderer>();
        //spriteRenderer.sortingOrder = 1;
        //spriteRenderer.sprite = sprite;
        FallerObject.transform.position = spawnPoint;
        FallerObject.transform.localScale = size; 
        behavior.BuildVisuals(FallerObject, FallerSize);
        rb = FallerObject.AddComponent<Rigidbody2D>();
        rb.sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        rb.gravityScale = Constants.gameGravity;
        //fallerObj.GetComponent<BoxCollider2D>().sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        rb.linearVelocity = new Vector2(0.0f, -speed);
        //rb.linearVelocity = new Vector2(0.0f, -0.01f);
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.mass = behavior.UseSettleTimer ? Constants.boulderDynamicMass : 1.0f;
        if(behavior.FreezeRotation)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
    }
    public void Init(Vector3 spawnPoint, Vector3 size, Vector2 speed, GameObject fallerObj)
    {
        FallerObject = fallerObj;
        fallerSpeed = speed.y;
        FallerSize = (Vector2)size;
        //SpriteRenderer spriteRenderer = fallerObject.AddComponent<SpriteRenderer>();
        //spriteRenderer.sortingOrder = 1;
        //spriteRenderer.sprite = sprite;
        FallerObject.transform.position = spawnPoint;
        FallerObject.transform.localScale = size;
        behavior.BuildVisuals(FallerObject, FallerSize);
        rb = FallerObject.AddComponent<Rigidbody2D>();
        rb.sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        rb.gravityScale = Constants.gameGravity;
        //fallerObj.GetComponent<BoxCollider2D>().sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        rb.linearVelocity = speed;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        //rb.linearVelocity = new Vector2(0.0f, -0.01f);
        rb.mass = behavior.UseSettleTimer ? Constants.boulderDynamicMass : 1.0f;
        if (behavior.FreezeRotation)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

    }
    // Update is called once per frame
    void Update()
    {
        if (FallerObject.transform.position.y < -4.0f 
            || FallerObject.transform.position.x > 12.5f 
            || FallerObject.transform.position.x < -12.5f)
        {
            //Out of bounds either in the wall of water, or below the floor, so should be deleted
            DeleteMe();
            return;
        }
        if (!isFrozen && behavior != null && behavior.UseSettleTimer
              && rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            if (rb.linearVelocity.magnitude < Constants.boulderSettleLinearThreshold
                && Mathf.Abs(rb.angularVelocity) < Constants.boulderSettleAngularThreshold 
                && !GameManager.instance().IsPlayerInEMT())
            {
                settleTimer += Time.deltaTime;
                if (settleTimer >= Constants.boulderSettleTime)
                    FloorPause();
            }
            else
            {
                settleTimer = 0f;
            }
        }
        behavior?.Update(this);
    }
    public void StartRiding()
    {
        BeingRidden = true;
    }
    public void StopRiding() { 
        BeingRidden = false; 
    }
    public void DeleteMe()
    {
        Destroy(FallerObject);
        Destroy(this);
    }
    public bool ShouldPointDamage(Vector2 collisionPoint)
    {
        return collisionPoint.y < FallerObject.transform.position.y && !isFrozen;
        /*bool pointDamages = false;
        Vector2 twoDPos = new Vector2(fallerObject.transform.position.x, fallerObject.transform.position.y);
        Vector2 direction = collisionPoint - twoDPos;
        Vector2 normal = direction.normalized;

        if(collisionPoint.y < fallerObject.transform.position.y)
        {
            pointDamages = true;
        }

        return pointDamages;*/
    }
    public bool IsRidingMe(Vector3 playerPoint)
    {
        return behavior?.IsRidingMe(this, playerPoint) ?? false;
        
    }
    public bool AmIFrozen()
    {
        return isFrozen;
    }

    public void FloorPause()
    {
        
        //Debug.Log("Faller " + gameObject.name + " is now frozen after colliding " + collisionCount + " times");
        behavior?.OnFloorPause(FallerObject, FallerSize);
        //gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0.0f, 0.580392157f, 0.0f);
        
        isFrozen = true;
    }
    public void Unfreeze()
    {
        collisionCount = 0;
        settleTimer = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = behavior.FreezeRotation
          ? RigidbodyConstraints2D.FreezeRotation
          : RigidbodyConstraints2D.None;

        rb.gravityScale = Constants.gameGravity;
        rb.linearVelocity = new Vector2(0.0f, 0.0f);
        rb.mass = behavior.UseSettleTimer ? Constants.boulderDynamicMass : 1.0f;
        behavior?.OnUnfreeze(gameObject, FallerSize);
        //rb.bodyType = RigidbodyType2D.Dynamic;
        //gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(1.0f, 1.0f, 1.0f);
        isFrozen = false;
        settleTimer = 0f;
    }

    public void HandleArmCollision(PunchingArmController arm)
    {
        behavior?.HandleArmCollision(this, arm);
        
    }
    public void Collided()
    {
        //Debug.Log(gameObject.name+"collided with something, collision count is now " + collisionCount);
        collisionCount++;
        if(collisionCount >= Constants.fallerCollisionFreezeThreshold && !isFrozen)
        {
            FloorPause();
        }
    }
    public void AddImpulse(Vector2 direction)
    {
        direction.Normalize();
        behavior?.AddImpulse(this, direction);
    }
    public void AddRedTint()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if(sr != null && sr.color.r > 0.0f)
        {
            sr.enabled = true;
            sr.sortingOrder = 2;
            sr.color = new UnityEngine.Color(1f, 0f, 0f, 0.098f);
        }
    }
    public void RemoveRedTint()
    {
        SpriteRenderer sr = gameObject.GetComponent<SpriteRenderer>();
        if (sr != null && sr.color.g < 1.0f)
        {
            sr.enabled = false;
        }
    }
    public void AddTint(UnityEngine.Color color, float alpha)
    {
        behavior?.AddTint(this, new UnityEngine.Color(color.r, color.g, color.b, alpha));
    }
    public void RemoveTint()
    {
        behavior?.RemoveTint(this);
    }
    public void SetVertices(Vector2[] verts)
    {
        vertices = verts;
    }
    public Vector2[] GetVertices()
    {
        return vertices;
    }
    public void FallerCollidesWithMe(FallerController otherController)
    {
        if(!fallersCollidingWithMe.ContainsKey(otherController.FallerObject.name))
        {
            fallersCollidingWithMe.Add(otherController.FallerObject.name, otherController);
        }
        GameManager.instance().Print("Fallers Colliding with me ("+gameObject.name+"): " + string.Join(", ", fallersCollidingWithMe.Keys), 0);
    }
    public void FallerStopsCollidingWithMe(FallerController otherController)
    {
        fallersCollidingWithMe.Remove(otherController.FallerObject.name);
        GameManager.instance().Print("Fallers Colliding with me (" + gameObject.name + "): " + string.Join(", ", fallersCollidingWithMe.Keys), 0);
        if(otherController.FallerObject.transform.position.y < FallerObject.transform.position.y)
        {
            //If the faller that stopped colliding with me is below me, I should check if I need to unfreeze since I may have been frozen due to being on top of them
            if (isFrozen)
            {
                Unfreeze();
            }
        }
    }

}
