using Assets.Scripts;
using UnityEngine;

public class BlockFallerBehavior : IFallerBehavior
{
    public bool UseSettleTimer => false;
    public bool FreezeRotation => true;
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
    public void OnUnfreeze(GameObject fallerObj) { }
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
}
