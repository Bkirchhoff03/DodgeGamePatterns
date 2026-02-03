using UnityEngine;

public class FallerCollisionHandler : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            return;
        }
        FallerController faller = collision.gameObject.GetComponent<FallerController>();
        if (faller != null)
        {
            faller.FloorPause();
        }
    }
}
