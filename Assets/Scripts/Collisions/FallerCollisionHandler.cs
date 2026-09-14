using Assets.Scripts;
using UnityEngine;

public class FallerCollisionHandler : MonoBehaviour
{
    private FallerController thisFaller;
    private Rigidbody2D rb;

    private void Awake()
    {
        thisFaller = GetComponent<FallerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            return;
        }


        if (thisFaller == null || GameManager.instance().IsPlayerInEMT())
        {
            return;
        }
        if (collision.gameObject.name == "PunchingArm")
        {
            GameManager.instance().Print($"Arm collided with {gameObject.name}", 0);
            thisFaller.HandleArmCollision(collision.gameObject.GetComponent<PunchingArmController>());
        }

        if (thisFaller.IsFrozen)
        {
            return;
        }

        if (thisFaller.UseSettleTimer)
        {
            if(rb == null)
            {
                rb = thisFaller.GetComponent<Rigidbody2D>();
            }
            rb.gravityScale = Constants.fallerGravityPostCollision;
        }
        else
        {
            FreezeIfOnFrozenFaller(thisFaller, collision);
        }

    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        // Frozen blocks have nothing to do on stay — exit before any string comparisons or singleton calls
        if (thisFaller != null && thisFaller.IsFrozen && !thisFaller.UseSettleTimer) return;

        if (collision.gameObject.name == "Player") return;

        if (thisFaller == null || GameManager.instance().IsPlayerInEMT()) return;

        if (thisFaller.IsFrozen) return;

        GameManager.instance().Print($"Collision stay on {gameObject.name} with {collision.gameObject.name}", 0);

        if (thisFaller.UseSettleTimer)
        {
            if (collision.gameObject.TryGetComponent<FallerController>(out var other)
                && other.IsFrozen)
            {
                thisFaller.Collided();
            }
        }
        else
        {
            FreezeIfOnFrozenFaller(thisFaller, collision);
        }
    }
    public void OnCollisionExit2D(Collision2D collision)
    {
        GameManager.instance().Print($"Collision EXIT on {gameObject.name} with {collision.gameObject.name}", 0);
        if (collision.gameObject.name == "Player")
        {
            return;
        }
        FallerController otherFaller = collision.gameObject.GetComponent<FallerController>();
        if (otherFaller != null && thisFaller != null)
        {
            thisFaller.FallerStopsCollidingWithMe(otherFaller);
            otherFaller.FallerStopsCollidingWithMe(thisFaller);
        }
    }

    // Only freeze this faller if the other faller is already frozen (grounded).
    // This creates natural stacking: blocks only freeze when landing on
    // something connected to the ground. Two unfrozen fallers colliding
    // mid-air will bounce off each other via physics instead.
    private void FreezeIfOnFrozenFaller(FallerController thisFaller, Collision2D collision)
    {
        FallerController frozenFaller = collision.gameObject.GetComponent<FallerController>();
        Collider2D frozenCollider = collision.collider;
        Collider2D thisCollider = collision.otherCollider;
        if (frozenFaller != null && frozenFaller.IsFrozen)
        {

            thisFaller.FallerCollidesWithMe(frozenFaller);
            frozenFaller.FallerCollidesWithMe(thisFaller);
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
            Rigidbody2D thisRb = thisFaller.gameObject.GetComponent<Rigidbody2D>();
            GameManager.instance().Print("otherRb velocity: " + otherRb.linearVelocity, 0);
            GameManager.instance().Print("thisRb velocity: " + thisRb.linearVelocity, 0);
            if (otherRb != null || Mathf.Abs(otherRb.linearVelocity.x) <= 0.1f)
            {
                GameManager.instance().Print($"Freezing {gameObject.name} on collision with frozen {collision.gameObject.name}", 0);
                thisFaller.FloorPause();
                if (thisFaller.UseSettleTimer)
                {
                    return;
                }
                Vector2 otherPos = frozenFaller.gameObject.transform.position;
                Vector2 thisPos = thisFaller.gameObject.transform.position;
                Bounds frozenBounds = frozenCollider.bounds;
                Bounds thisBounds = thisCollider.bounds;

                if(!frozenBounds.Intersects(thisBounds))
                {
                    Bounds dif = GetDifference(thisBounds, frozenBounds);
                    /*Debug.DrawLine(dif.min, dif.max, Color.red, 5f);
                    Debug.DrawLine(new Vector3(dif.min.x, dif.max.y), new Vector3(dif.max.x, dif.min.y), Color.red, 5f);*/
                    GameManager.instance().Print("Drew lines", 8);
                    if (dif.size.x >= dif.size.y)
                    {
                        // Handle Y difference
                        float dir = thisPos.y > otherPos.y ? -1 : 1;
                        thisFaller.gameObject.transform.position =
                            new Vector3(thisPos.x, thisPos.y + dir * dif.size.y);
                    }
                    else
                    {
                        // Handle X difference
                        float dir = thisPos.x > otherPos.x ? -1 : 1;
                        thisFaller.gameObject.transform.position =
                            new Vector3(thisPos.x + dir * dif.size.x, thisPos.y);
                    }
                }
                else
                {
                    Bounds overlap = GetOverlap(thisBounds, frozenBounds); 
                    /*Debug.DrawLine(overlap.min, overlap.max, Color.red, 5f);
                    Debug.DrawLine(new Vector3(overlap.min.x, overlap.max.y), new Vector3(overlap.max.x, overlap.min.y), Color.red, 5f);*/
                    GameManager.instance().Print("Drew lines", 8);
                    if (overlap.size.x >= overlap.size.y)
                    {
                        // Handle Y difference
                        float dir = thisPos.y > otherPos.y ? 1 : -1; 
                        thisFaller.gameObject.transform.position =
                            new Vector3(thisPos.x, thisPos.y + dir * overlap.size.y);
                    }
                    else
                    {
                        // Handle X difference
                        float dir = thisPos.x > otherPos.x ? 1 : -1;
                        thisFaller.gameObject.transform.position =
                            new Vector3(thisPos.x + dir * overlap.size.x, thisPos.y);
                    }
                }
                /*GameManager.instance().Print($"Y difference: {Mathf.Abs(thisPos.y - otherPos.y) - frozenFaller.FallerSize.y / 2} \n X difference: {Mathf.Abs(thisPos.x - otherPos.x) - frozenFaller.FallerSize.x / 2}", 0);
                UnityEngine.Debug.Break();
                if (Mathf.Abs(thisPos.y - otherPos.y) - frozenFaller.FallerSize.y / 2 >
                    Mathf.Abs(thisPos.x - otherPos.x) - frozenFaller.FallerSize.x / 2)
                {
                    // Y difference between distance of centers and sum of half-heights is greater than
                    // X difference between distance of centers and sum of half-widths,
                    // so we are colliding more on top/bottom than left/right. Snap to top of other faller.
                    float dir = thisPos.y > otherPos.y ? 1 : -1;
                    *//*thisFaller.gameObject.transform.position = 
                        new Vector3(thisPos.x, otherPos.y + dir * ((frozenFaller.FallerSize.y / 2f) + (thisFaller.FallerSize.y / 2f)));*//*
                }
                else
                {
                    // Otherwise, we are colliding more on left/right than top/bottom. Snap to right of other faller.
                    float dir = thisPos.x > otherPos.x ? 1 : -1;
                    *//*                    thisFaller.gameObject.transform.position = 
                                            new Vector3(otherPos.x + dir * ((frozenFaller.FallerSize.x / 2f) + (thisFaller.FallerSize.x / 2f)), thisPos.y);*//*
                }*/
            }
        }
    }

    private Bounds GetDifference(Bounds thisBound, Bounds frozenBounds)
    {
        Bounds intersection = new Bounds();
        //LowerMax, HigherMin
        Vector3 minA = thisBound.min; //(basically the bottom-left - back corner point)
        Vector3 maxA = thisBound.max; //(basically the top-right - front corner point)

        Vector3 minB = frozenBounds.min;
        Vector3 maxB = frozenBounds.max;

        // we want the smaller of the max and the higher of the min points
        Vector3 lowerMax = Vector3.Min(maxA, maxB);
        Vector3 higherMin = Vector3.Max(minA, minB);

        Vector3 totMax = new Vector3();
        Vector3 totMin = new Vector3();

        if (lowerMax.x < higherMin.x)
        {
            totMax.x = higherMin.x;
            totMin.x = lowerMax.x;
        }
        else
        {
            totMax.x = lowerMax.x;
            totMin.x = higherMin.x;
        }

        if (lowerMax.y < higherMin.y)
        {
            totMax.y = higherMin.y;
            totMin.y = lowerMax.y;
        }
        else
        {
            totMax.y = lowerMax.y;
            totMin.y = higherMin.y;
        }
        intersection.SetMinMax(totMin, totMax);
        if (!thisBound.Intersects(frozenBounds))
        {
            
        }
        else
        {
            
        }
        return intersection;
    }
    private Bounds GetOverlap(Bounds thisBound, Bounds frozenBounds)
    {
        Bounds intersection = new Bounds();
        //LowerMax, HigherMin
        Vector3 minA = thisBound.min; //(basically the bottom-left - back corner point)
        Vector3 maxA = thisBound.max; //(basically the top-right - front corner point)

        Vector3 minB = frozenBounds.min;
        Vector3 maxB = frozenBounds.max;

        // we want the smaller of the max and the higher of the min points
        Vector3 lowerMax = Vector3.Min(maxA, maxB);
        Vector3 higherMin = Vector3.Max(minA, minB);

        intersection.SetMinMax(higherMin, lowerMax);
        return intersection;
    }
}
