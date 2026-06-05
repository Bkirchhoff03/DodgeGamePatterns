using Assets.Scripts;
using UnityEngine;

public class FallerCollisionHandler : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            return;
        }

        // Get this faller's controller; skip if already frozen
        FallerController thisFaller = GetComponent<FallerController>();

        if (thisFaller == null || GameManager.instance().IsPlayerInEMT())
        {
            return;
        }
        if (collision.gameObject.name == "PunchingArm")
        {
            GameManager.instance().Print($"Arm collided with {gameObject.name}", 0);
            GetComponent<FallerController>().HandleArmCollision(collision.gameObject.GetComponent<PunchingArmController>());
        }

        if (thisFaller.IsFrozen)
        {
            return;
        }

        if (thisFaller.UseSettleTimer)
        {
            GetComponent<Rigidbody2D>().gravityScale = Constants.fallerGravityPostCollision;
        }
        else
        {
            FreezeIfOnFrozenFaller(thisFaller, collision);
        }

    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            return;
        }

        // Get this faller's controller; skip if already frozen
        FallerController thisFaller = GetComponent<FallerController>();

        if (thisFaller == null || GameManager.instance().IsPlayerInEMT())
        {
            return;
        }
        if (collision.gameObject.name == "PunchingArm")
        {
            GameManager.instance().Print($"Arm collided with {gameObject.name}", 0);
            GetComponent<FallerController>().HandleArmCollision(collision.gameObject.GetComponent<PunchingArmController>());
        }
        if (thisFaller.IsFrozen)
        {
            return;
        }
        GameManager.instance().Print($"Collision stay on {gameObject.name} with {collision.gameObject.name}", 0);
        if (thisFaller.UseSettleTimer)
        {
            if (collision.gameObject.TryGetComponent<FallerController>(out var other)
                && other.IsFrozen && !thisFaller.IsFrozen)
            {
                thisFaller.Collided();
            }
            else if (collision.gameObject.TryGetComponent<FallerController>(out var other2) && !other2.IsFrozen && thisFaller.IsFrozen)
            {
                other2.Collided();
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
        FallerController thisFaller = GetComponent<FallerController>();
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
        FallerController otherFaller = collision.gameObject.GetComponent<FallerController>();

        if (otherFaller != null && otherFaller.IsFrozen)
        {

            thisFaller.FallerCollidesWithMe(otherFaller);
            otherFaller.FallerCollidesWithMe(thisFaller);
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (otherRb != null || Mathf.Abs(otherRb.linearVelocity.x) <= 0.1f)
            {
                GameManager.instance().Print($"Freezing {gameObject.name} on collision with frozen {collision.gameObject.name}", 0);
                thisFaller.FloorPause();
                Vector2 otherPos = otherFaller.gameObject.transform.position;
                Vector2 thisPos = thisFaller.gameObject.transform.position;
                if (Mathf.Abs(thisPos.y - otherPos.y) - otherFaller.FallerSize.y / 2 > 
                    Mathf.Abs(thisPos.x - otherPos.x) - otherFaller.FallerSize.x / 2)
                {
                    // Y difference between distance of centers and sum of half-heights is greater than
                    // X difference between distance of centers and sum of half-widths,
                    // so we are colliding more on top/bottom than left/right. Snap to top of other faller.
                    float dir = thisPos.y > otherPos.y ? 1 : -1;
                    thisFaller.gameObject.transform.position = 
                        new Vector3(thisPos.x, otherPos.y + dir * ((otherFaller.FallerSize.y / 2f) + (thisFaller.FallerSize.y / 2f)));
                }
                else
                {
                    // Otherwise, we are colliding more on left/right than top/bottom. Snap to right of other faller.
                    float dir = thisPos.x > otherPos.x ? 1 : -1;
                    thisFaller.gameObject.transform.position = 
                        new Vector3(otherPos.x + dir * ((otherFaller.FallerSize.x / 2f) + (thisFaller.FallerSize.x / 2f)), thisPos.y);
                }
            }
        }
    }
}
