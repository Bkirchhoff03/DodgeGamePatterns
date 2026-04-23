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

    public bool UseSettleTimer => true;
    public bool FreezeRotation => false;

    public void BuildVisuals(GameObject fallerObj, Vector2 size)
    {
        BoulderShape shape = Shapes[Random.Range(0, Shapes.Length)];
        shapeColor = shape.color;

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
        meshRenderer.material.color = new Color(0f, 0.58f, 0f);
    }

    public void OnUnfreeze(GameObject fallerObj, Vector2 fallerSize)
    {
        meshRenderer.material.color = shapeColor;
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
    public void AddImpulse(FallerController fc, Vector2 impulse)
    {
        fc.gameObject.GetComponent<Rigidbody2D>().AddForce(impulse, ForceMode2D.Impulse);
    }

}
