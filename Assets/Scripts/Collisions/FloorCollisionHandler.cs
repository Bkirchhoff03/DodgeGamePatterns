using UnityEngine;

public class FloorCollisionHandler : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        FallerController faller = collision.gameObject.GetComponent<FallerController>();
        if (faller != null)
        {
            faller.FloorPause();
        }
    }
}
