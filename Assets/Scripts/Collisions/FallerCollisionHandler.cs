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
        if (thisFaller == null || thisFaller.IsFrozen)
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
        FallerController thisFaller = GetComponent<FallerController>();
        if (thisFaller == null || thisFaller.IsFrozen) 
        { 
            return; 
        }
        GameManager.instance().Print($"Collision stay on {gameObject.name} with {collision.gameObject.name}");
        if (thisFaller.UseSettleTimer) 
        {
            if (collision.gameObject.TryGetComponent<FallerController>(out var other)
                && other.IsFrozen && !thisFaller.IsFrozen)
            {
                thisFaller.Collided();
            }else if (collision.gameObject.TryGetComponent<FallerController>(out var other2) && !other2.IsFrozen && thisFaller.IsFrozen)
            {
                other2.Collided();
            }
        }
        else
        {
            FreezeIfOnFrozenFaller(thisFaller, collision);
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
            Rigidbody2D otherRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (otherRb != null || Mathf.Abs(otherRb.linearVelocity.x) <= 0.1f)
            {
                thisFaller.FloorPause();
            }
        }
    }
}
