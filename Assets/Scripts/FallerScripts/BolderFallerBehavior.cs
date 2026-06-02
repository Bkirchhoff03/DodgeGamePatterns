using Assets.Scripts;
using UnityEngine;

public class BolderFallerBehavior : IFallerBehavior
{
    private struct BoulderShape { public Vector2[] vertices; public Color color; }

    private static readonly BoulderShape[] Shapes = new BoulderShape[]
    {
          new BoulderShape { vertices = new Vector2[] {
              new(-0.45f,-0.50f), new(0.45f,-0.50f), new(0.50f,-0.10f),
              new(0.35f,0.45f), new(0.00f,0.50f), new(-0.40f,0.35f), new(-0.50f,0.00f)
          }, color = new Color(0.44f,0.58f,0.77f) },
          new BoulderShape { vertices = new Vector2[] {
              new(-0.40f,-0.50f), new(0.40f,-0.50f), new(0.50f,0.10f),
              new(0.20f,0.50f), new(-0.20f,0.50f), new(-0.50f,0.10f)
          }, color = new Color(0.95f,0.52f,0.18f) },
          new BoulderShape { vertices = new Vector2[] {
              new(-0.30f,-0.50f), new(0.30f,-0.50f), new(0.50f,-0.20f), new(0.50f,0.20f),
              new(0.25f,0.50f), new(-0.25f,0.50f), new(-0.50f,0.20f), new(-0.50f,-0.20f)
          }, color = new Color(0.83f,0.35f,0.75f) },
          new BoulderShape { vertices = new Vector2[] {
              new(-0.30f,-0.50f), new(0.30f,-0.50f), new(0.50f,-0.30f), new(0.50f,0.30f),
              new(0.20f,0.50f), new(-0.20f,0.50f), new(-0.50f,0.30f), new(-0.50f,-0.30f)
          }, color = new Color(0.24f,0.75f,0.73f) },
          new BoulderShape { vertices = new Vector2[] {
              new(-0.45f,-0.50f), new(0.45f,-0.50f), new(0.50f,0.10f),
              new(0.00f,0.50f), new(-0.50f,0.10f)
          }, color = new Color(0.95f,0.80f,0.15f) },
          new BoulderShape { vertices = new Vector2[] {
              new(-0.25f,-0.50f), new(0.25f,-0.50f), new(0.50f,0.00f),
              new(0.40f,0.50f), new(-0.40f,0.50f), new(-0.50f,0.00f)
          }, color = new Color(0.48f,0.71f,0.36f) },
          new BoulderShape { vertices = new Vector2[] {
              new(-0.40f,-0.50f), new(0.15f,-0.50f), new(0.45f,-0.35f), new(0.50f,0.00f),
              new(0.40f,0.40f), new(0.10f,0.50f), new(-0.25f,0.50f), new(-0.50f,0.25f), new(-0.50f,-0.20f)
          }, color = new Color(0.90f,0.42f,0.29f) },
          new BoulderShape { vertices = new Vector2[] {
              new(-0.40f,-0.50f), new(0.40f,-0.50f), new(0.50f,-0.30f), new(0.50f,0.30f),
              new(0.30f,0.50f), new(-0.30f,0.50f), new(-0.50f,0.30f), new(-0.50f,-0.30f)
          }, color = new Color(0.60f,0.40f,0.80f) },
    };

    private MeshRenderer meshRenderer;
    private Color shapeColor;
    private bool isPaused = false;
    public bool UseSettleTimer => true;
    public bool FreezeRotation => false;
    public void Update(FallerController fc) { }
    public GameObject CreateGameObject(string name, Vector2 size) 
    {
        GameObject fallerObject = new GameObject(name);
        fallerObject.layer = LayerMask.NameToLayer("Fallers");
        fallerObject.AddComponent<FallerController>();
        fallerObject.AddComponent<FallerCollisionHandler>();
        return fallerObject;
    }

    public void BuildVisuals(GameObject fallerObj, Vector2 size)
    {
        BoulderShape shape = Shapes[Random.Range(0, Shapes.Length)];
        shapeColor = shape.color;
        
        fallerObj.GetComponent<FallerController>().SetVertices(shape.vertices);

        MeshFilter mf = fallerObj.AddComponent<MeshFilter>();
        mf.mesh = BuildMesh(shape.vertices);
                
        meshRenderer = fallerObj.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Sprites/Default"));
        meshRenderer.material.color = shapeColor;
        meshRenderer.sortingOrder = 1;

        PolygonCollider2D poly = fallerObj.AddComponent<PolygonCollider2D>();
        poly.SetPath(0, shape.vertices);
        poly.sharedMaterial = Resources.Load<PhysicsMaterial2D>(Constants.fallerPhysicsMaterial2DPath);
    }

    public void OnFloorPause(GameObject fallerObj, Vector2 fallerSize)
    {
        Rigidbody2D rb = fallerObj.GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        rb.gravityScale = 0f;
        rb.mass = 10000f;
        meshRenderer.material.color = new Color(0f, 0.58f, 0f);
        isPaused = true;
    }

    public void OnUnfreeze(GameObject fallerObj, Vector2 fallerSize)
    {
        meshRenderer.material.color = shapeColor;
        isPaused = false;
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
            if(stackCount == 0)
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

    private static Mesh BuildMesh(Vector2[] verts2D)
    {
        int n = verts2D.Length;
        Vector3[] verts3D = new Vector3[n];
        for (int i = 0; i < n; i++)
            verts3D[i] = new Vector3(verts2D[i].x, verts2D[i].y, 0f);
        int[] tris = new int[3 * (n - 2)];
        for (int i = 0; i < n - 2; i++) { tris[3 * i] = 0; tris[3 * i + 1] = i + 1; tris[3 * i + 2] = i + 2; }
        Mesh mesh = new Mesh();
        mesh.vertices = verts3D;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        return mesh;
    }
    public void AddImpulse(FallerController fc, Vector2 direction)
    {
        fc.gameObject.GetComponent<Rigidbody2D>().AddForce(direction* Constants.EMT_Impulse_bolder, ForceMode2D.Impulse);
    }
    public void AddTint(FallerController fc, Color tint)
    {
        fc.gameObject.GetComponent<MeshRenderer>().material.color = new Color(tint.r, tint.g, tint.b, 1.0f);
    }
    public void RemoveTint(FallerController fc)
    {
        if(isPaused)
        {
            fc.gameObject.GetComponent<MeshRenderer>().material.color = new Color(0f, 0.58f, 0f);
            return;
        }
        fc.gameObject.GetComponent<MeshRenderer>().material.color = shapeColor;
    }
    public bool IsRidingMe(FallerController fc, Vector2 playerPosition)
    {
        Rect playerBounds = new Rect(
            playerPosition - new Vector2(Constants.halfPlayerWidth, Constants.halfPlayerHeight), 
            new Vector2(Constants.halfPlayerWidth*2f, Constants.halfPlayerHeight*2f));
        PolygonCollider2D poly = fc.gameObject.GetComponent<PolygonCollider2D>();
        if (poly == null) return false; // safety check
        Vector2 bottomLeft = playerPosition - new Vector2(Constants.halfPlayerWidth, Constants.halfPlayerHeight);
        Vector2 bottomRight = playerPosition - new Vector2(-Constants.halfPlayerWidth, Constants.halfPlayerHeight);
        if (poly.OverlapPoint(bottomLeft) || poly.OverlapPoint(bottomRight))
        {
            return true;
        }
        if(Vector2.Distance(poly.ClosestPoint(bottomRight), bottomRight) < 0.05f)
        {
            return true;
        }
        if(Vector2.Distance(poly.ClosestPoint(bottomLeft), bottomLeft) < 0.05f)
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
