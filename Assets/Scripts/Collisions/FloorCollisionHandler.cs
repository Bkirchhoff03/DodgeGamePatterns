using Assets.Scripts;
using UnityEngine;

public class FloorCollisionHandler : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        NormalFallerController faller = collision.gameObject.GetComponent<NormalFallerController>();
        if (faller != null)
        {
            faller.FloorPause();
        }
        else 
        {
            if (collision.gameObject.TryGetComponent<PlayerController>(out var player))
            {
                player.setState(new DodgingState());
            }
        }
    }
}
