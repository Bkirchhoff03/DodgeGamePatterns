using Assets.Scripts;
using UnityEngine;

public class BolderFallerBehavior : IFallerBehavior
{
    // Collider silhouettes only now — visuals come from Prefabs/Boulder{index}, one per shape
    private static readonly Vector2[][] Shapes = new Vector2[][]
    {
        // BOLDER_1 7 sides
        new Vector2[] {
            new(-0.45f,-0.50f), new(0.45f,-0.50f), new(0.50f,-0.10f),
            new(0.35f,0.45f), new(0.00f,0.50f), new(-0.40f,0.35f), new(-0.50f,0.00f)
        },
        // BOLDER_2 6 sides
        new Vector2[] {
            new(-0.40f,-0.50f), new(0.40f,-0.50f), new(0.50f,0.10f),
            new(0.20f,0.50f), new(-0.20f,0.50f), new(-0.50f,0.10f)
        },
        // BOLDER_3 8 sides
        new Vector2[] {
            new(-0.30f,-0.50f), new(0.30f,-0.50f), new(0.50f,-0.20f), new(0.50f,0.20f),
            new(0.25f,0.50f), new(-0.25f,0.50f), new(-0.50f,0.20f), new(-0.50f,-0.20f)
        },
        // BOLDER_4 8 sides
        new Vector2[] {
            new(-0.30f,-0.50f), new(0.30f,-0.50f), new(0.50f,-0.30f), new(0.50f,0.30f),
            new(0.20f,0.50f), new(-0.20f,0.50f), new(-0.50f,0.30f), new(-0.50f,-0.30f)
        },
        // BOLDER_5 5 sides
        new Vector2[] {
            new(-0.45f,-0.50f), new(0.45f,-0.50f), new(0.50f,0.10f),
            new(0.00f,0.50f), new(-0.50f,0.10f)
        },
        // BOLDER_6 6 sides
        new Vector2[] {
            new(-0.25f,-0.50f), new(0.25f,-0.50f), new(0.50f,0.00f),
            new(0.40f,0.50f), new(-0.40f,0.50f), new(-0.50f,0.00f)
        },
        // BOLDER_7 9 sides
        new Vector2[] {
            new(-0.40f,-0.50f), new(0.15f,-0.50f), new(0.45f,-0.35f), new(0.50f,0.00f),
            new(0.40f,0.40f), new(0.10f,0.50f), new(-0.25f,0.50f), new(-0.50f,0.25f), new(-0.50f,-0.20f)
        },
        // BOLDER_8 8 sides
        new Vector2[] {
            new(-0.40f,-0.50f), new(0.40f,-0.50f), new(0.50f,-0.30f), new(0.50f,0.30f),
            new(0.30f,0.50f), new(-0.30f,0.50f), new(-0.50f,0.30f), new(-0.50f,-0.30f)
        },
    };

    private int shapeIndex;
    public bool UseSettleTimer => true;
    public bool FreezeRotation => false;
    public void Update(FallerController fc) { }
    public GameObject CreateGameObject(string name, Vector2 size)
    {
        shapeIndex = Random.Range(0, Shapes.Length);
        GameObject fallerObject = GameObject.Instantiate(
            Resources.Load<GameObject>("Prefabs/Bolders/BolderBackground" + shapeIndex + "_" + Mathf.FloorToInt(size.x)));
        int fallerLayer = LayerMask.NameToLayer("Fallers");
        fallerObject.GetComponent<SpriteRenderer>().sortingOrder = 25;
        fallerObject.gameObject.layer = fallerLayer;
        /*foreach (Transform t in fallerObject.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = fallerLayer;
            SpriteRenderer sr = t.gameObject.GetComponent<SpriteRenderer>();
            sr.sortingOrder = 25;
        }*/
        fallerObject.name = name;
        return fallerObject;
    }

    public void BuildVisuals(GameObject fallerObj, Vector2 size)
    {
        Vector2[] vertices = Shapes[shapeIndex];
        fallerObj.GetComponent<FallerController>().SetVertices(vertices);

        PolygonCollider2D poly = fallerObj.AddComponent<PolygonCollider2D>();
        poly.SetPath(0, vertices);
        poly.sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
    }

    public void OnFloorPause(GameObject fallerObj, Vector2 fallerSize)
    {
        Rigidbody2D rb = fallerObj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        rb.gravityScale = 0f;
        rb.mass = 10000f;
        fallerObj.transform.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
        //GameManager.instance().BoulderSettledSprites[shapeIndex];
    }

    public void OnUnfreeze(GameObject fallerObj, Vector2 fallerSize)
    {
        fallerObj.transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
        //.sprite =
        //GameManager.instance().BoulderSprites[shapeIndex];
    }

    public void HandleArmCollision(FallerController fc, PunchingArmController arm)
    {
        float punchVelocity = arm.getPunchingVelocity();
        if (fc.IsFrozen)
        {
            if (!GameManager.instance().HasStamina(Constants.frozenPunchStaminaCost))
            {
                arm.CancelPunch();
                return;
            }
            int stackCount = FallerManager.instance().GetFrozenFallersAbove(fc);
            if (stackCount == 0)
            {
                float forceMult = Constants.boulderPunchForceMultiplier / (1f + stackCount * Constants.frozenPunchStackWeightFactor);
                fc.Unfreeze();
                fc.gameObject.GetComponent<Rigidbody2D>().AddForce(
                    new Vector2(punchVelocity * forceMult, 0f), ForceMode2D.Impulse);
            }

            GameManager.instance().UseStamina(Constants.frozenPunchStaminaCost);
            GameManager.instance().TakeDamage(Constants.frozenPunchLifeCost);
        }
        else
        {
            fc.gameObject.GetComponent<Rigidbody2D>().AddForce(
                new Vector2(punchVelocity * Constants.boulderPunchForceMultiplier, 0f), ForceMode2D.Impulse);
        }
        arm.CancelPunch();
    }

    public void AddImpulse(FallerController fc, Vector2 direction)
    {
        fc.gameObject.GetComponent<Rigidbody2D>().AddForce(direction * Constants.EMT_Impulse_bolder, ForceMode2D.Impulse);
    }
    public void AddTint(FallerController fc, Color tint)
    {
        SpriteRenderer sr = fc.gameObject.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.enabled = true;
            sr.sortingOrder = 2;
            sr.color = new Color(tint.r, tint.g, tint.b, tint.a);
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
        Rect playerBounds = new Rect(
            playerPosition - new Vector2(Constants.halfPlayerWidth, Constants.halfPlayerHeight),
            new Vector2(Constants.halfPlayerWidth * 2f, Constants.halfPlayerHeight * 2f));
        PolygonCollider2D poly = fc.gameObject.GetComponent<PolygonCollider2D>();
        if (poly == null) return false; // safety check
        ColliderDistance2D distance = GameManager.instance().player.GetComponent<Collider2D>().Distance(poly);
        if (distance.isOverlapped)
        {
            return true;
        }
        if (distance.distance < 0.05f)
        {
            return true;
        }
        return false;
    }
}
