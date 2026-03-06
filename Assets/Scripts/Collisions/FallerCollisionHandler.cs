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
        FallerController thisFaller = gameObject.GetComponent<FallerController>();
        if (thisFaller == null || thisFaller.IsFrozen)
        {
            return;
        }

        // Only freeze this faller if the other faller is already frozen (grounded).
        // This creates natural stacking: blocks only freeze when landing on
        // something connected to the ground. Two unfrozen fallers colliding
        // mid-air will bounce off each other via physics instead.
        FallerController otherFaller = collision.gameObject.GetComponent<FallerController>();
        if (otherFaller != null && otherFaller.IsFrozen)
        {
            if (collision.gameObject.GetComponent<Rigidbody2D>() != null && Mathf.Abs(collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity.x) > 0.1)
            {
                //Don't freeze, its moving left/right.
            }
            else
            {
                thisFaller.FloorPause();
            }
        }
    }
}
