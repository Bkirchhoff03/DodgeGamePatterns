using UnityEngine;

public class FallerBehavior : MonoBehaviour
{
    GameObject fallerObject;
    float fallerSpeed;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void Init(GameObject fallerObj, float speed)
    {
        fallerObject = fallerObj;
        fallerSpeed = speed;
    }
    // Update is called once per frame
    void Update()
    {
        if (fallerObject.transform.position.y < -4.0f)
        {
            Destroy(fallerObject);
            Destroy(this);
        }
        //fallerObject.transform.position += Vector3.down * Time.deltaTime * fallerSpeed;
    }
}
