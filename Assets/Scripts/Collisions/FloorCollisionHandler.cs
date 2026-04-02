using Assets.Scripts;
using UnityEngine;

public class FloorCollisionHandler : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        FallerController faller = collision.gameObject.GetComponent<FallerController>();
        if (faller != null)
        {
            if (!faller.UseSettleTimer)
            {
                faller.FloorPause();
            }
        }
        else if (collision.gameObject.TryGetComponent<PlayerController>(out var player))
        { 
            player.setState(new DodgingState());
        }
        
    }
}
