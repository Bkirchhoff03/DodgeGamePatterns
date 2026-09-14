using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    private PlayerController pc;
    private int fallerLayer;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        fallerLayer = LayerMask.NameToLayer("Fallers");
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        GameManager.instance().Print("Player collided with: " + collision.gameObject.name, 1);
        if (collision.gameObject.layer != fallerLayer) return;
        HandlePlayerCollision(collision);
    }

    // Handles the case where the player clips onto the top of a faller (via corner physics
    // depenetration) without triggering a fresh OnCollisionEnter2D top-contact event.
    // Only acts when the player is already in FallingState and resting on a faller's top surface.
    public void OnCollisionStay2D(Collision2D collision)
    {
        // State check first — cheapest guard, avoids layer lookup on most frames
        if (pc == null || pc.state.getName() != Assets.Scripts.Constants.fallingStateName) return;
        if (collision.gameObject.layer != fallerLayer) return;
        HandlePlayerCollision(collision);
    }

    private void HandlePlayerCollision(Collision2D collision)
    {
        Vector2 contactNormal = collision.GetContact(0).normal;
        //Debug.Log("Contact Normal: " + contactNormal + " With: " + collision.gameObject.name);

        GameManager.PlayerFallerCollisionType collisionType = GameManager.PlayerFallerCollisionType.None;

        if (Mathf.Abs(contactNormal.x) < Mathf.Abs(contactNormal.y))
        {
            if (contactNormal.y < -0.1f)
            {
                //Debug.Log("Hits bottom of object");
                collisionType = GameManager.PlayerFallerCollisionType.Bottom;
            }
            else if (contactNormal.y > 0.1f)
            {
                //GameManager.instance().Print("Hits top of object");
                collisionType = GameManager.PlayerFallerCollisionType.Top;
            }
        }
        else if (Mathf.Abs(contactNormal.x) > Mathf.Abs(contactNormal.y))
        {
            if (contactNormal.x < -0.1f)
            {
                //GameManager.instance().Print("Hits left of object");
                collisionType = GameManager.PlayerFallerCollisionType.Left;
            }
            else if (contactNormal.x > 0.1f)
            {
                //GameManager.instance().Print("Hits right of object");
                collisionType = GameManager.PlayerFallerCollisionType.Right;
            }
        }

        if (collisionType != GameManager.PlayerFallerCollisionType.None)
        {
            GameManager.instance().HandlePlayerFallerCollision(gameObject, collision.gameObject, collisionType);
        }
    }
}
