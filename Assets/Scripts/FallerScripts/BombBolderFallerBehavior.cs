using Assets.Scripts;
using System.Linq;
using UnityEngine;

public class BombBolderFallerBehavior : IFallerBehavior
{
    // Collider silhouettes only now — visuals come from Prefabs/Boulder{index}, one per shape (same prefabs as BolderFallerBehavior)
    private static readonly Vector2[][] Shapes = new Vector2[][]
    {
          new Vector2[] {
              new(-0.45f,-0.50f), new(0.45f,-0.50f), new(0.50f,-0.10f),
              new(0.35f,0.45f), new(0.00f,0.50f), new(-0.40f,0.35f), new(-0.50f,0.00f)
          },
          new Vector2[] {
              new(-0.40f,-0.50f), new(0.40f,-0.50f), new(0.50f,0.10f),
              new(0.20f,0.50f), new(-0.20f,0.50f), new(-0.50f,0.10f)
          },
          new Vector2[] {
              new(-0.30f,-0.50f), new(0.30f,-0.50f), new(0.50f,-0.20f), new(0.50f,0.20f),
              new(0.25f,0.50f), new(-0.25f,0.50f), new(-0.50f,0.20f), new(-0.50f,-0.20f)
          },
          new Vector2[] {
              new(-0.30f,-0.50f), new(0.30f,-0.50f), new(0.50f,-0.30f), new(0.50f,0.30f),
              new(0.20f,0.50f), new(-0.20f,0.50f), new(-0.50f,0.30f), new(-0.50f,-0.30f)
          },
          new Vector2[] {
              new(-0.45f,-0.50f), new(0.45f,-0.50f), new(0.50f,0.10f),
              new(0.00f,0.50f), new(-0.50f,0.10f)
          },
          new Vector2[] {
              new(-0.25f,-0.50f), new(0.25f,-0.50f), new(0.50f,0.00f),
              new(0.40f,0.50f), new(-0.40f,0.50f), new(-0.50f,0.00f)
          },
          new Vector2[] {
              new(-0.40f,-0.50f), new(0.15f,-0.50f), new(0.45f,-0.35f), new(0.50f,0.00f),
              new(0.40f,0.40f), new(0.10f,0.50f), new(-0.25f,0.50f), new(-0.50f,0.25f), new(-0.50f,-0.20f)
          },
          new Vector2[] {
              new(-0.40f,-0.50f), new(0.40f,-0.50f), new(0.50f,-0.30f), new(0.50f,0.30f),
              new(0.30f,0.50f), new(-0.30f,0.50f), new(-0.50f,0.30f), new(-0.50f,-0.30f)
          },
    };
    private float bombRadius = 1.5f; // radius of the explosion effect
    private float flashTimer = 0f; // timer for flashing effect before explosion
    private int flashCount = 0; // count of flashes before explosion
    private float flashInterval = 0.2f; // interval between flashes
    private int frozenFlashCount = 0; // count of flashes while frozen
    private int shapeIndex;
    private Vector2 NameSize = Vector2.zero;
    public bool UseSettleTimer => true;
    public bool FreezeRotation => false;
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
            GameManager.instance().StartFallerEMT(1.0f);
            fc.DeleteMe();
        }

    }
    public GameObject CreateGameObject(string name, Vector2 size)
    {
        shapeIndex = Random.Range(0, Shapes.Length);
        size = new Vector2(size.x - 1f, size.y - 1f);
        GameObject fallerObject = GameObject.Instantiate(
            Resources.Load<GameObject>("Prefabs/Bolders/BolderBackground" + shapeIndex + "_" + Mathf.FloorToInt(size.x)));
        NameSize = size;

        int fallerLayer = LayerMask.NameToLayer("Fallers");
        fallerObject.GetComponent<SpriteRenderer>().sortingOrder = 25;

        /*foreach (Transform t in fallerObject.GetComponentsInChildren<Transform>(true))
        {
            t.gameObject.layer = fallerLayer;
        }*/
        fallerObject.name = name;
        AddTint(fallerObject.GetComponent<FallerController>(), new Color(1f, 0f, 0f, 0.5f)); // Add red tint to indicate it's a bomb bolder
        return fallerObject;
    }
    public void BuildVisuals(GameObject fallerObj, Vector2 size)
    {
        Vector2[] vertices = Shapes[shapeIndex].ToArray();
        // MAKE SURE TO SCALE THE VERTICES BASED ON THE NAME SIZE OF THE FALLER
        float mult = ((NameSize.x * 10f) + 40f) / 35f;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 v = vertices[i];
            vertices[i] = new Vector2(v.x * mult, v.y * mult);
        }
        fallerObj.GetComponent<FallerController>().SetVertices(vertices);

        PolygonCollider2D poly = fallerObj.AddComponent<PolygonCollider2D>();
        poly.SetPath(0, vertices);
        poly.sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
    }
    public void OnFloorPause(GameObject fallerObj, Vector2 fallerSize)
    {
        Rigidbody2D rb = fallerObj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        //rb.bodyType = RigidbodyType2D.Static;
        //rb.gravityScale = 0f;
        //rb.mass = 10000f;
        fallerObj.transform.GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f, 1f);
        //fallerObj.transform.Find("Body").GetComponent<SpriteRenderer>().sprite = GameManager.instance().BoulderSettledSprites[shapeIndex];
    }
    public void OnUnfreeze(GameObject fallerObj, Vector2 fallerSize)
    {
        fallerObj.transform.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

        //fallerObj.transform.Find("Body").GetComponent<SpriteRenderer>().sprite = GameManager.instance().BoulderSprites[shapeIndex];
    }
    public void HandleArmCollision(FallerController fc, PunchingArmController arm)
    {
        float punchVelocity = arm.getPunchingVelocity();
        if (fc.IsFrozen)
        {
            fc.Unfreeze();  // punch unfreezes a frozen boulder
        }
        fc.gameObject.GetComponent<Rigidbody2D>().AddForce(
            new Vector2(punchVelocity * Constants.boulderPunchForceMultiplier, 0f), ForceMode2D.Impulse);
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
        PolygonCollider2D poly = fc.gameObject.GetComponent<PolygonCollider2D>();
        if (poly == null) return false; // safety check
        Vector2 bottomLeft = playerPosition - new Vector2(Constants.halfPlayerWidth, Constants.halfPlayerHeight);
        Vector2 bottomRight = playerPosition - new Vector2(-Constants.halfPlayerWidth, Constants.halfPlayerHeight);
        if (poly.OverlapPoint(bottomLeft) || poly.OverlapPoint(bottomRight))
        {
            return true;
        }
        if (Vector2.Distance(poly.ClosestPoint(bottomRight), bottomRight) < 0.05f)
        {
            return true;
        }
        if (Vector2.Distance(poly.ClosestPoint(bottomLeft), bottomLeft) < 0.05f)
        {
            return true;
        }
        RaycastHit2D hit = Physics2D.Raycast(bottomLeft, Vector2.right, bottomRight.x - bottomLeft.x, LayerMask.GetMask("Fallers"));
        if (hit.collider != null && hit.collider.gameObject.name == fc.gameObject.name)
        {
            return true;
        }
        return false;
    }
}
