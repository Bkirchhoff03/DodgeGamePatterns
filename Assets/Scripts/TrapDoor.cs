using Assets.Scripts;
using UnityEngine;

// Goal object at the top of the level. Player reaching it resets the level.
// Height is configurable in the Inspector.
public class TrapDoor : MonoBehaviour
{
    public float height = 50.0f; // Set in Inspector to control how high the goal is

    void Start()
    {
        // Position at the configured height, spanning the full level width
        transform.position = new Vector3(0, height, 0);
        transform.localScale = new Vector3(Constants.maxX - Constants.minX, 2.0f, 1.0f);

        // Create a gold/yellow rectangle sprite at runtime
        SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
            new Vector2(0.5f, 0.5f));
        spriteRenderer.color = new Color(1.0f, 0.84f, 0.0f);

        // Trigger collider so player passes through rather than bouncing off
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }

    // When the player enters the trigger, reset the level
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            GameManager.instance().ResetLevel();
        }
    }
}
