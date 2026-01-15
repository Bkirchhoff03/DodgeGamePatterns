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
        //Figure out if object is hitting gameObject from left, right, top, bottom
        //player hits objects bottom side: normal = (0,-1)
        //player hits objects top side: normal = (0,1)
        //player hits objects left side: normal = (-1,0)
        //player hits objects right side: normal = (1,0)
        Vector2 contactNormal = collision.GetContact(0).normal;
        Debug.Log("Contact Normal: " + collision.GetContact(0).normal + " With: " + collision.gameObject.name);
        if (Mathf.Abs(contactNormal.x) < Mathf.Abs(contactNormal.y))
        {
            //Vertical hit
            if(contactNormal.y < -0.1f)
            {
                //Hit on bottom of object
                Debug.Log("Hits bottom of object");
                GameManager.instance().HandlePlayerFallerCollision(gameObject, collision.gameObject, GameManager.PlayerFallerCollisionType.Bottom);
            }else if (contactNormal.y > 0.1f)
            {
                //Hit on top of object
                Debug.Log("Hits top of object");
                GameManager.instance().HandlePlayerFallerCollision(gameObject, collision.gameObject, GameManager.PlayerFallerCollisionType.Top);
                

            }

        }else if(Mathf.Abs(contactNormal.x) > Mathf.Abs(contactNormal.y))
        {
            //Horizontal hit
            if(contactNormal.x < -0.1f)
            {
                //Hit on left of object
                Debug.Log("Hits left of object");
                GameManager.instance().HandlePlayerFallerCollision(gameObject, collision.gameObject, GameManager.PlayerFallerCollisionType.Left);
            }else if (contactNormal.x > 0.1f)
            {
                //Hit on right of object
                Debug.Log("Hits right of object");
                GameManager.instance().HandlePlayerFallerCollision(gameObject, collision.gameObject, GameManager.PlayerFallerCollisionType.Right);
            }
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        Debug.Log("FROM COLLISION DETECTOR Collision Exited with " + collision.gameObject.name);
        //gameObject.GetComponent<Rigidbody2D>().totalForce = Vector2.zero;
    }
}
