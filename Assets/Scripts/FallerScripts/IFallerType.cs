using UnityEngine;

public interface IFallerType
{
    public GameObject fallerObject { get; }
    bool IsFrozen { get; }
    
    public bool BeingRidden { get; }
    string TypeName { get; }
    public void Init(Vector3 spawnPoint, Vector3 size, float speed, Sprite sprite, GameObject fallerObj);
    /*// Update is called once per frame
    void Update()
    {
        if (fallerObject.transform.position.y < -4.0f || fallerObject.transform.position.x > 12.5f || fallerObject.transform.position.x < -12.5f)
        {
            //Out of bounds either in the wall of water, or below the floor, so should be deleted
            DeleteMe();
        }
        *//*Rigidbody2D r = gameObject.GetComponent<Rigidbody2D>();
        if (!BeingRidden && r != null && Mathf.Abs(r.linearVelocity.x) < 0.001 && Mathf.Abs(r.linearVelocity.y) < 0.001) 
        {
            FloorPause();
        }*//*
        //fallerObject.transform.position += Vector3.down * Time.deltaTime * fallerSpeed;
    }*/
    public void StartRiding();
    public void StopRiding();
    /*public void DeleteMe()
    {
        Destroy(fallerObject);
        Destroy(this);
    }*/
    public bool shouldPointDamage(Vector2 collisionPoint)
    {
        bool pointDamages = false;
        Vector2 twoDPos = new Vector2(fallerObject.transform.position.x, fallerObject.transform.position.y);
        Vector2 direction = collisionPoint - twoDPos;
        Vector2 normal = direction.normalized;

        if (collisionPoint.y < fallerObject.transform.position.y)
        {
            pointDamages = true;
        }

        return pointDamages;
    }
    public bool isRidingMe(Vector3 playerPoint);
    /*public bool isRidingMe(Vector3 playerPoint)
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
    }*/
    public bool amIFrozen();
    public void FloorPause();
    /*    public void FloorPause()
        {
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 0f;
            gameObject.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            gameObject.GetComponent<Rigidbody2D>().mass = 10000f;
            gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(0.0f, 0.580392157f, 0.0f);
            isFrozen = true;
        }*/
    public void Unfreeze();
    /*public void Unfreeze()
    {
        gameObject.GetComponent<Rigidbody2D>().gravityScale = Constants.gameGravity;
        gameObject.GetComponent<Rigidbody2D>().mass = 0.0001f;
        //gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        gameObject.GetComponent<SpriteRenderer>().color = new UnityEngine.Color(1.0f, 1.0f, 1.0f);
        isFrozen = false;
    }*/
    public void HandleArmCollision(PunchingArmController arm);
    /*public void HandleArmCollision(PunchingArmController arm)
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
    }*/
}