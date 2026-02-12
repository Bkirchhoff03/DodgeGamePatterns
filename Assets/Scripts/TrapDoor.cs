using UnityEngine;

public class TrapDoor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            //Reset game at the moment, but i'd like to make the game change levels eventually
            GameManager.instance().ResetGame();
        }
        else if(collision.gameObject.name.Contains("Faller"))
        {
            collision.gameObject.GetComponent<FallerController>().DeleteMe();
        }
    }
}
