using Assets.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;

// Goal object at the top of the level. Player reaching it resets the level.
// Height is configurable in the Inspector.
public class TrapDoor : MonoBehaviour
{
    public float height = 50.0f; // Set in Inspector to control how high the goal is

    void Start()
    {
        // Position at the configured height, spanning the full level width
        transform.position = new Vector3(0, height, 0);
        //transform.localScale = new Vector3(Constants.maxX - Constants.minX, 2.0f, 1.0f);

        // Create a gold/yellow rectangle sprite at runtime
        /*SpriteRenderer spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = Sprite.Create(
            Texture2D.whiteTexture,
            new Rect(0, 0, Texture2D.whiteTexture.width, Texture2D.whiteTexture.height),
            new Vector2(0.5f, 0.5f));
        spriteRenderer.color = new Color(1.0f, 0.84f, 0.0f);
        */
        // Trigger collider so player passes through rather than bouncing off
        BoxCollider2D collider = gameObject.AddComponent<BoxCollider2D>();
        collider.isTrigger = true;
    }

    // When the player enters the trigger, reset the level
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            if(SceneManager.GetActiveScene().name == "Level1")
            {
                PlayerPrefs.SetFloat("PlayerLivesFromLevel1", GameManager.instance().GetPlayerLives());
                // TEMPORARILY RESET LEVEL 1 WITH SLIGHTLY HARDER FALLER SPAWNING (FASTER AND MORE FREQUENT) TO TEST PROGRESSION
                    
                    // Space invaders: Less sprites to draw increases speed of the game, so when the level progresses, game gets faster
                    // New levels, enemies spawn lower on the screen, but still lots of sprites, so slower game.

                // Spawn fallers faster and faster as the level progresses, but also increase the speed fallers spawn with. 
                // This will make the game more difficult as the player progresses, but also more rewarding when they succeed.
                float timeBetweenSpawns = GameManager.instance().beginningTimeBetweenSpawns;
                SceneManager.LoadScene("Level1");
                GameManager.instance().UpdateSaveSession();
                GameManager.instance().SetSpawnTime(timeBetweenSpawns * 0.8f);
                GameManager.instance().SetFallerSpeedMultiplier(1.25f);
                GameManager.instance().Print("Spawn time: " + GameManager.instance().beginningTimeBetweenSpawns + ", Faller speed: " + GameManager.instance().FallerStartingSpeed);
            }
            else if(SceneManager.GetActiveScene().name == "Level2") 
            {
                PlayerPrefs.SetFloat("PlayerLivesFromLevel1", GameManager.instance().GetPlayerLives());
                SceneManager.LoadScene("Level3");
                GameManager.instance().UpdateSaveSession();
            }
            else if(SceneManager.GetActiveScene().name == "Level3") 
            {
                PlayerPrefs.SetFloat("PlayerLivesFromLevel1", GameManager.instance().GetPlayerLives());
                SceneManager.LoadScene("Level4");
                GameManager.instance().UpdateSaveSession();
            }
            else
            {
                PlayerPrefs.SetFloat("PlayerLivesFromLevel1", 3f);
                Time.timeScale = 0f;
                SceneManager.LoadScene("WinTheGame");
            }
        }
    }
}
