using Assets.Scripts;
using System;
using System.Drawing;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public class FallerController : MonoBehaviour
{
    public GameObject fallerObject { get; private set; }
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        BeingRidden = false;
    }
    public void SetBehaviour(IFallerBehavior b) { behavior = b; }
    public void Init(Vector3 spawnPoint, Vector3 size, float speed, GameObject fallerObj)
    {
        fallerObject = fallerObj;
        fallerSpeed = speed;
        FallerSize = new Vector2(size.x, size.y);
        //SpriteRenderer spriteRenderer = fallerObject.AddComponent<SpriteRenderer>();
        //spriteRenderer.sortingOrder = 1;
        //spriteRenderer.sprite = sprite;
        fallerObject.transform.position = spawnPoint;
        fallerObject.transform.localScale = size; 
        behavior.BuildVisuals(fallerObject, FallerSize);
        rb = fallerObject.AddComponent<Rigidbody2D>();
        rb.sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        rb.gravityScale = Constants.gameGravity;
        //fallerObj.GetComponent<BoxCollider2D>().sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        rb.linearVelocity = new Vector2(0.0f, -speed);
        //rb.linearVelocity = new Vector2(0.0f, -0.01f);
        rb.mass = behavior.UseSettleTimer ? Constants.boulderDynamicMass : 1.0f;
        if(behavior.FreezeRotation)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
    }
    // Update is called once per frame
    void Update()
    {
        if (fallerObject.transform.position.y < -4.0f 
            || fallerObject.transform.position.x > 12.5f 
            || fallerObject.transform.position.x < -12.5f)
        {
            //Out of bounds either in the wall of water, or below the floor, so should be deleted
            DeleteMe();
            return;
        }
        if (!isFrozen && behavior != null && behavior.UseSettleTimer
              && rb != null && rb.bodyType == RigidbodyType2D.Dynamic)
        {
            if (rb.linearVelocity.magnitude < Constants.boulderSettleLinearThreshold
                && Mathf.Abs(rb.angularVelocity) < Constants.boulderSettleAngularThreshold)
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
        Destroy(fallerObject);
        Destroy(this);
    }
    public bool shouldPointDamage(Vector2 collisionPoint)
    {
        return collisionPoint.y < fallerObject.transform.position.y && !isFrozen;
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
    public bool isRidingMe(Vector3 playerPoint)
    {
        float leftBound = gameObject.transform.position.x - (gameObject.transform.localScale.x / 2.0f);
        float rightBound = gameObject.transform.position.x + (gameObject.transform.localScale.x / 2.0f);
        
        if ((playerPoint.x + Constants.halfPlayerWidth) > leftBound && (playerPoint.x - Constants.halfPlayerWidth) < rightBound)
        { 
            BeingRidden = true;
            return true;
        }
        else
        {
            BeingRidden = false;
            return false;
        }
    }
    public bool amIFrozen()
    {
        return isFrozen;
    }

    public void FloorPause()
    {
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        rb.gravityScale = 0f;
        rb.mass = 10000f;
        //Debug.Log("Faller " + gameObject.name + " is now frozen after colliding " + collisionCount + " times");
        behavior?.OnFloorPause(fallerObject, FallerSize);
        //gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0.0f, 0.580392157f, 0.0f);
        /*if(FallerSize.x == 0.5f)
        {
            Transform t1 = transform.Find("T1");
            t1.GetComponent<SpriteRenderer>().sprite = GameManager.instance().CenterGrassTile;
            //Debug.Log("Faller size is 0.5, setting tile to center grass tile");
        }
        else
        {
            Transform t1 = transform.Find("T1");
            t1.GetComponent<SpriteRenderer>().sprite = GameManager.instance().LeftGrassTile;
            Transform leftTop = transform.Find("T" + ((int)(FallerSize.x*2)).ToString());
            leftTop.GetComponent<SpriteRenderer>().sprite = GameManager.instance().RightGrassTile;
            
            for (int i = 2; i < (int)(FallerSize.x*2); i += 1)
            {
                Transform t = transform.Find("T" + i.ToString());
                t.GetComponent<SpriteRenderer>().sprite = GameManager.instance().CenterGrassTile;
            }
            //Debug.Log("Faller size is " + FallerSize.x + ", setting tile 1 to left grass tile, tile (" + ((int)(FallerSize.x * 2)).ToString() +  ")right grass tile, and center grass tiles");
        }*/
        isFrozen = true;
    }
    public void Unfreeze()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;

        rb.gravityScale = Constants.gameGravity;
        rb.mass = behavior != null && behavior.UseSettleTimer ? Constants.boulderDynamicMass : 1.0f;
        behavior?.OnUnfreeze(gameObject);
        //rb.bodyType = RigidbodyType2D.Dynamic;
        //gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(1.0f, 1.0f, 1.0f);
        isFrozen = false;
        settleTimer = 0f;
    }

    public void HandleArmCollision(PunchingArmController arm)
    {
        behavior?.HandleArmCollision(this, arm);
        /*if (isFrozen)
        {
            arm.CancelPunch();
        }
        else
        {
            //add to velocity of the faller based on the punch direction and velocity
            Rigidbody2D r = gameObject.GetComponent<Rigidbody2D>();
            float punchVelocity = arm.getPunchingVelocity();
            r.AddForce(new Vector2(punchVelocity * Constants.blockPunchForceMultiplier, 0.0f), ForceMode2D.Impulse);
            arm.CancelPunch();
        }*/
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
}
