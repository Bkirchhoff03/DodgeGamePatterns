using UnityEngine;

public class ArmCollisionHandler : MonoBehaviour
{
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<FallerController>() == null)
        {
            return;
        }

        Debug.Log("Player Arm Collision with " + collision.gameObject.name);
        HandleArmFallerCollision(collision);
    }
    private void HandleArmFallerCollision(Collision2D collision)
    {
        collision.gameObject.GetComponent<FallerController>().HandleArmCollision(GetComponent<PunchingArmController>());
    }
}