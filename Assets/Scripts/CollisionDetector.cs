using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        Debug.Log("FROM COLLISION DETECTOR Collision Detected with " + collision.gameObject.name);
        //Debug.Log("Contact Point: " + collision.GetContact(0).point);
        Debug.Log("Contact Normal: " + collision.GetContact(0).normal);
        //Debug.Log("Collision Impulse: " + collision.GetContact(0));
        //Figure out if object is hitting gameObject from left, right, top, bottom
        Vector2 collisionNormal = collision.GetContact(0).normal;
        Vector2 forceDirection = -collisionNormal.normalized;




        
    }
    public void OnCollisionExit(Collision collision)
    {
        Debug.Log("FROM COLLISION DETECTOR Collision Exited with " + collision.gameObject.name);
        gameObject.GetComponent<Rigidbody2D>().totalForce = Vector2.zero;
    }
}
