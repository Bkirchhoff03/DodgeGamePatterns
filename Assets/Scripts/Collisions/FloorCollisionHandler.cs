using Assets.Scripts;
using UnityEngine;

public class FloorCollisionHandler : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        FallerController faller = collision.gameObject.GetComponent<FallerController>();
        if (faller != null)
        {
            if (!faller.UseSettleTimer || !GameManager.instance().IsPlayerInEMT())
            {
                faller.FloorPause();
            }
        }
        /*else if (collision.gameObject.TryGetComponent<PlayerController>(out var player))
        {
            GameManager.instance().Print( collision.gameObject.name + " collided with the floor, switching to dodging state from " +player.state.getName() +" at: " + player.gameObject.transform.position, 1);
            player.setState(new DodgingState());
        }*/
        
    }
    public void OnCollisionStay2D(Collision2D collision)
    {
        FallerController faller = collision.gameObject.GetComponent<FallerController>();
        if (faller != null)
        {
            if (!faller.UseSettleTimer || !GameManager.instance().IsPlayerInEMT())
            {
                faller.FloorPause();
            }
        }
    }
}
