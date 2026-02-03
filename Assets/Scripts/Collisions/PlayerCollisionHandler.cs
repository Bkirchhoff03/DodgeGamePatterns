using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<FallerController>() == null)
        {
            return;
        }

        Debug.Log("Player Collision with " + collision.gameObject.name);
        HandlePlayerCollision(collision);
    }

    private void HandlePlayerCollision(Collision2D collision)
    {
        Vector2 contactNormal = collision.GetContact(0).normal;
        Debug.Log("Contact Normal: " + contactNormal + " With: " + collision.gameObject.name);

        GameManager.PlayerFallerCollisionType collisionType = GameManager.PlayerFallerCollisionType.None;

        if (Mathf.Abs(contactNormal.x) < Mathf.Abs(contactNormal.y))
        {
            if (contactNormal.y < -0.1f)
            {
                Debug.Log("Hits bottom of object");
                collisionType = GameManager.PlayerFallerCollisionType.Bottom;
            }
            else if (contactNormal.y > 0.1f)
            {
                Debug.Log("Hits top of object");
                collisionType = GameManager.PlayerFallerCollisionType.Top;
            }
        }
        else if (Mathf.Abs(contactNormal.x) > Mathf.Abs(contactNormal.y))
        {
            if (contactNormal.x < -0.1f)
            {
                Debug.Log("Hits left of object");
                collisionType = GameManager.PlayerFallerCollisionType.Left;
            }
            else if (contactNormal.x > 0.1f)
            {
                Debug.Log("Hits right of object");
                collisionType = GameManager.PlayerFallerCollisionType.Right;
            }
        }

        if (collisionType != GameManager.PlayerFallerCollisionType.None)
        {
            GameManager.instance().HandlePlayerFallerCollision(gameObject, collision.gameObject, collisionType);
        }
    }
}
