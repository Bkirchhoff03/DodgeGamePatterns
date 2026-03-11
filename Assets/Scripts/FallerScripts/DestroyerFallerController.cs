
using Assets.Scripts;
using UnityEngine;

public class DestroyerFallerController : MonoBehaviour, IFallerType
{
    private string typeName = "NormalFallerController";

    public GameObject fallerObject { get; private set; }
    private bool beingRidden; 
    public bool BeingRidden
    {
        get => beingRidden;
        private set => beingRidden = value;
    }

    private bool isFrozen;
    public bool IsFrozen
    {
        get => isFrozen;
        private set => isFrozen = value;
    }
    public string TypeName { get => typeName; }
    void Start()
    {
        BeingRidden = false;
    }

    public void Init(Vector3 spawnPoint, Vector3 size, float speed, Sprite sprite, GameObject fallerObj)
    {
        fallerObject = fallerObj;
        SpriteRenderer spriteRenderer = fallerObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 1;
        spriteRenderer.sprite = sprite;
        fallerObject.transform.position = spawnPoint;
        fallerObject.transform.localScale = size;
        fallerObject.AddComponent<DestroyerFallerCollisionHandler>();
        fallerObject.AddComponent<BoxCollider2D>();
        fallerObject.GetComponent<BoxCollider2D>().sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        fallerObject.AddComponent<Rigidbody2D>();
        fallerObject.GetComponent<Rigidbody2D>().sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        fallerObject.GetComponent<Rigidbody2D>().gravityScale = Constants.gameGravity; // Could set the gravity to random speed sent to this function
        fallerObject.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0.0f, -0.01f);
        fallerObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }
    void Update()
    {
        if (fallerObject.transform.position.y < -4.0f || fallerObject.transform.position.x > 12.5f || fallerObject.transform.position.x < -12.5f)
        {
            //Out of bounds either in the wall of water, or below the floor, so should be deleted
            DeleteMe();
        }
        /*Rigidbody2D r = gameObject.GetComponent<Rigidbody2D>();
        if (!BeingRidden && r != null && Mathf.Abs(r.linearVelocity.x) < 0.001 && Mathf.Abs(r.linearVelocity.y) < 0.001) 
        {
            FloorPause();
        }*/
        //fallerObject.transform.position += Vector3.down * Time.deltaTime * fallerSpeed;
    }
    public bool isRidingMe(Vector3 playerPoint)
    {
        return BeingRidden;
    }
    public void DeleteMe()
    {
        Destroy(fallerObject);
        Destroy(this);
    }
    public void StartRiding()
    {
        BeingRidden = true;
    }
    public void StopRiding()
    {
        BeingRidden = false;
    }
    public void FloorPause()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
        gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        gameObject.GetComponent<Rigidbody2D>().mass = 10000f;
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0.0f, 0.580392157f, 0.0f);
        isFrozen = true;
    }

    public void Unfreeze()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = Constants.gameGravity;
        gameObject.GetComponent<Rigidbody2D>().mass = 0.0001f;
        //gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(1.0f, 1.0f, 1.0f);
        isFrozen = false;
    }

    public void HandleArmCollision(PunchingArmController arm)
    {
        if (isFrozen)
        {
            arm.CancelPunch();
        }
        else
        {
            //add to velocity of the faller based on the punch direction and velocity
            Rigidbody2D r = gameObject.GetComponent<Rigidbody2D>();
            float punchVelocity = arm.getPunchingVelocity();
            r.AddForce(new Vector2(punchVelocity * Constants.punchForceMultiplier, 0.0f), ForceMode2D.Impulse);
            arm.CancelPunch();
        }
    }
    public bool amIFrozen()
    {
        return isFrozen;
    }
}