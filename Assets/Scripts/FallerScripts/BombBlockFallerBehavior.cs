using Assets.Scripts;
using UnityEngine;

public class BombBlockFallerBehavior : IFallerBehavior
{
    private float bombRadius = 1.5f; // radius of the explosion effect
    private float flashTimer = 0f; // timer for flashing effect before explosion
    private int flashCount = 0; // count of flashes before explosion
    private float flashInterval = 0.2f; // interval between flashes
    private int frozenFlashCount = 0; // count of flashes while frozen
    public bool UseSettleTimer => false;
    public bool FreezeRotation => true;
    public void Update(FallerController fc) 
    { 
        flashTimer += Time.deltaTime;
        if(flashTimer >= flashInterval)
        {
            flashTimer = 0f;
            flashCount++;
            if (flashCount % 2 == 0)
                AddTint(fc, new Color(1f, 0f, 0f, 0.5f)); // Flash red
            else
                RemoveTint(fc); // Remove tint
            if (fc.IsFrozen)
            {
                flashInterval = 0.1f; // Speed up flashing when frozen
                frozenFlashCount++;
            }
        }
        if(frozenFlashCount >= 10) // If frozen for too long, explode anyway
        {

            float force = Mathf.Round(fc.transform.position.y / 10f);
            if(force < 1f) force = 1f;
            FallerManager.instance().UnfreezeImpulse(fc.transform.position, force);
            GameManager.instance().ImpulsePlayer(fc);
            GameManager.instance().StartFallerEMT(2.5f);
            fc.DeleteMe();
        }

    }
    public GameObject CreateGameObject(string name, Vector2 size) 
    {
        string xName = Mathf.Round(size.x * 2f) / 2f == size.x
                        ? size.x.ToString("0.#") : size.x.ToString("0.#");
        string yName = size.y.ToString("0.#");
        GameManager.instance().Print("Creating BombBlockFaller with size: " + xName + "x" + yName, 2);
        GameObject fallerObject = GameObject.Instantiate(
            Resources.Load<GameObject>("Prefabs/" + xName + "_by_" + yName));
        fallerObject.layer = LayerMask.NameToLayer("Fallers");
        fallerObject.name = name;
        AddTint(fallerObject.GetComponent<FallerController>(), new Color(1f, 0f, 0f, 0.5f)); // Add red tint to indicate it's a bomb block
        return fallerObject;
    }
    public void BuildVisuals(GameObject fallerObj, Vector2 size) 
    {
        BoxCollider2D col = fallerObj.AddComponent<BoxCollider2D>();
        col.sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
        if(col.sharedMaterial == null)
        {
            Debug.LogError("Failed to load physics material for faller. Check the path: " + Constants.fallerPhysicsMaterial2DPath);
        }
    }
    public void OnFloorPause(GameObject fallerObj, Vector2 fallerSize)
    {
        Rigidbody2D rb = fallerObj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        //rb.bodyType = RigidbodyType2D.Static;
        //rb.gravityScale = 0f;
        //rb.mass = 10000f;
        if (fallerSize.x == 0.5f)
        {
            fallerObj.transform.Find("T1").GetComponent<SpriteRenderer>().sprite =
                GameManager.instance().CenterGrassTile;
        }
        else
        {
            fallerObj.transform.Find("T1").GetComponent<SpriteRenderer>().sprite =
                GameManager.instance().LeftGrassTile;
            fallerObj.transform.Find("T" + ((int)(fallerSize.x * 2)).ToString())
                .GetComponent<SpriteRenderer>().sprite = GameManager.instance().RightGrassTile;
            for (int i = 2; i < (int)(fallerSize.x * 2); i++)
                fallerObj.transform.Find("T" + i).GetComponent<SpriteRenderer>().sprite =
                    GameManager.instance().CenterGrassTile;
        }
    }
    public void OnUnfreeze(GameObject fallerObj, Vector2 fallerSize) 
    {
        if (fallerSize.x == 0.5f)
        {
            fallerObj.transform.Find("T1").GetComponent<SpriteRenderer>().sprite =
                GameManager.instance().CenterDirtTile;
        }
        else
        {
            fallerObj.transform.Find("T1").GetComponent<SpriteRenderer>().sprite =
                GameManager.instance().LeftDirtTile;
            fallerObj.transform.Find("T" + ((int)(fallerSize.x * 2)).ToString())
                .GetComponent<SpriteRenderer>().sprite = GameManager.instance().RightDirtTile;
            for (int i = 2; i < (int)(fallerSize.x * 2); i++)
                fallerObj.transform.Find("T" + i).GetComponent<SpriteRenderer>().sprite =
                    GameManager.instance().CenterDirtTile;
        }
    }
    public void HandleArmCollision(FallerController fc, PunchingArmController arm)
    {
        if (fc.IsFrozen)
        {
            arm.CancelPunch();  // can't punch a frozen block
        }
        else
        {
            float punchVelocity = arm.getPunchingVelocity();
            fc.gameObject.GetComponent<Rigidbody2D>().AddForce(
                new Vector2(punchVelocity * Constants.blockPunchForceMultiplier, 0f), ForceMode2D.Impulse);
            arm.CancelPunch();
        }
    }
    public void AddImpulse(FallerController fc, Vector2 direction)
    {
        fc.gameObject.GetComponent<Rigidbody2D>().AddForce(direction * Constants.EMT_Impulse_block, ForceMode2D.Impulse);
    }
    public void AddTint(FallerController fc, Color tint)
    {
        SpriteRenderer sr = fc.gameObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = true;
            sr.sortingOrder = 2;
            sr.color = new UnityEngine.Color(tint.r, tint.g, tint.b, tint.a);
        }
    }
    public void RemoveTint(FallerController fc)
    {
        SpriteRenderer sr = fc.gameObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = false;
        }
    }

}
