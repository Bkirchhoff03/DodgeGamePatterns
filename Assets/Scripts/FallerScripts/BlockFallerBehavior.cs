using Assets.Scripts;
using UnityEngine;

public class BlockFallerBehavior : IFallerBehavior
{
    public bool UseSettleTimer => false;
    public bool FreezeRotation => true;
    public void Update(FallerController fc) { }
    public GameObject CreateGameObject(string name, Vector2 size) {
        string xName = Mathf.Round(size.x * 2f) / 2f == size.x
                    ? size.x.ToString("0.#") : size.x.ToString("0.#");
        string yName = size.y.ToString("0.#");
        GameObject fallerObject = GameObject.Instantiate(
            Resources.Load<GameObject>("Prefabs/" + xName + "_by_" + yName));
        int fallerLayer = LayerMask.NameToLayer("Fallers");
        foreach (Transform t in fallerObject.GetComponentsInChildren<Transform>(true))
            t.gameObject.layer = fallerLayer;
        fallerObject.name = name;
        return fallerObject;
    }
    public void BuildVisuals(GameObject fallerObj, Vector2 size) 
    {
        BoxCollider2D col = fallerObj.AddComponent<BoxCollider2D>();
        Bounds b = col.bounds;
        
        fallerObj.GetComponent<FallerController>().SetVertices(new Vector2[] { 
            new Vector2(b.min.x, b.min.y), new Vector2(b.max.x, b.min.y), 
            new Vector2(b.max.x, b.max.y), new Vector2(b.min.x, b.max.y) });
        
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
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.gravityScale = 0f;
        rb.mass = 10000f;
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
        GameManager.instance().Print("Block handle arm collision", 0);
        if (fc.IsFrozen)
        {
            GameManager.instance().Print("Faller is frozen: " + fc.IsFrozen, 0);
            if (!GameManager.instance().HasStamina(Constants.frozenPunchStaminaCost))
            {
                GameManager.instance().Print("No Stamina:( to punch block", 3);
                arm.CancelPunch();
                return;
            }
            float punchDir = arm.getPunchingVelocity() > 0 ? 1f : -1f;
            int stackCount = FallerManager.instance().GetFrozenFallersAbove(fc);
            GameManager.instance().Print("Frozen fallers above: " + stackCount, 0);
            float shift = punchDir * Constants.frozenBlockShiftAmount / (1f + stackCount * Constants.frozenPunchStackWeightFactor);
            shift = Mathf.Max(Mathf.Abs(shift), 0.05f) * punchDir;
            float sixSections = fc.FallerSize.y / 6f;
            for (int i = -3; i <= 3; i += 1)
            {
                if(Physics2D.Raycast(new Vector2(fc.gameObject.transform.position.x + (((fc.FallerSize.x / 2f) +0.01f)*punchDir), fc.gameObject.transform.position.y + (sixSections * i)), new Vector2(shift, 0f), Mathf.Abs(shift) + 0.01f))
                {
                    GameManager.instance().Print("Block punch would move block onto something at height offset: " + (sixSections * i) + ", cancelling punch", 3);
                    arm.CancelPunch();
                    return;
                }
            }
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(fc.gameObject.transform.position.x + (((fc.FallerSize.x / 2f) +0.01f)*punchDir), fc.gameObject.transform.position.y), new Vector2(shift, 0f), Mathf.Abs(shift) + 0.01f);
            if (hit && hit.collider.name != fc.gameObject.name)
            {
                GameManager.instance().Print("Block punch would move block onto "+hit.collider.name+", cancelling punch", 3);
                arm.CancelPunch();
                return;
            }
            GameManager.instance().Print("Moving block by: " + shift, 2);
            //fc.gameObject.transform.position += new Vector3(shift, 0f, 0f);
            fc.gameObject.GetComponent<Rigidbody2D>().position += new Vector2(shift, 0f);
            GameManager.instance().UseStamina(Constants.frozenPunchStaminaCost);
            GameManager.instance().TakeDamage(Constants.frozenPunchLifeCost);
            arm.CancelPunch();
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
    public bool IsRidingMe(FallerController fc, Vector2 playerPosition)
    {
        BoxCollider2D col = fc.gameObject.GetComponent<BoxCollider2D>();
        if (col == null)
        {
            Debug.LogError("BlockFallerBehavior IsRidingMe called but no BoxCollider2D found on faller");
            return false;
        }
        Bounds bounds = col.bounds;
        float leftBound = bounds.min.x;
        float rightBound = bounds.max.x;

        if ((playerPosition.y - bounds.max.y) > 0 &&
            (playerPosition.y - bounds.max.y) < Constants.halfPlayerHeight + 0.1f &&
            (playerPosition.x + Constants.halfPlayerWidth) > leftBound &&
            (playerPosition.x - Constants.halfPlayerWidth) < rightBound)
        {
            return true;
        }
        return false;
        
       
    }
}
