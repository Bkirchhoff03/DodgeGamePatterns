using UnityEngine;

public class DestroyerFallerCollisionHandler : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            return;
        }

        // Get this faller's controller; skip if already frozen
        DestroyerFallerController thisFaller = gameObject.GetComponent<DestroyerFallerController>();
        if (thisFaller == null || thisFaller.IsFrozen)
        {
            return;
        }

        FreezeIfOnFrozenFaller(thisFaller, collision);
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            return;
        }

        NormalFallerController thisFaller = gameObject.GetComponent<NormalFallerController>();
        if (thisFaller == null || thisFaller.IsFrozen)
        {
            return;
        }
        DestroyFallerBelow(thisFaller, collision);
        //FreezeIfOnFrozenFaller(thisFaller, collision);
    }

    // Only freeze this faller if the other faller is already frozen (grounded).
    // This creates natural stacking: blocks only freeze when landing on
    // something connected to the ground. Two unfrozen fallers colliding
    // mid-air will bounce off each other via physics instead.
    private void FreezeIfOnFrozenFaller(DestroyerFallerController thisFaller, Collision2D collision)
    {
        DestroyerFallerController otherFaller = collision.gameObject.GetComponent<DestroyerFallerController>();
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
    private void DestroyFallerBelow(NormalFallerController thisFaller, Collision2D collision)
    {
        NormalFallerController otherFaller = collision.gameObject.GetComponent<NormalFallerController>();
        if (otherFaller != null && otherFaller.IsFrozen)
        {
            if (collision.gameObject.GetComponent<Rigidbody2D>() != null && Mathf.Abs(collision.gameObject.GetComponent<Rigidbody2D>().linearVelocity.x) > 0.1)
            {
                //Don't destroy, its moving left/right.
            }
            else
            {
                otherFaller.DeleteMe();
            }
        }
    }
}
