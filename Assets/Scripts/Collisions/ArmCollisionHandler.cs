using UnityEngine;

public class ArmCollisionHandler : MonoBehaviour
{
    // Arm collider is a trigger on a Kinematic Rigidbody2D so it fires against
    // both Dynamic (falling) and Static (frozen) fallers.
    public void OnTriggerEnter2D(Collider2D other)
    {
        GameManager.instance().Print("Player Arm Trigger with " + other.gameObject.name, 1);

        FallerController fc = other.gameObject.GetComponent<FallerController>();
        if (fc == null)
        {
            return;
        }

        fc.HandleArmCollision(GetComponent<PunchingArmController>());
    }
}