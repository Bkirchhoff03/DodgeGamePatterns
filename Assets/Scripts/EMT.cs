using Codice.CM.Common;
using UnityEngine;

public class EMT : MonoBehaviour
{
    GameObject LocationReference;
    private bool isEMTed = false;
    private Vector3 EMTTarget;
    private float speed = 5f; // Speed at which the EMT moves towards the target
    private static EMT instance_;
    public EMT()
    {
        instance_ = this;
    }
    public static EMT instance() => instance_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance_ == null)
        {
            instance_ = this;
        }
        LocationReference = GameObject.Find("EMTLocation");
        // 1. Get the screen position of your UI element
        Vector3 screenPos = LocationReference.transform.position;
        // 2. Set the Z distance (how far into the world from the camera)
        // For perspective cameras, Z MUST be > 0 (e.g., NearClipPlane or a fixed distance)
        screenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        // 3. Convert to World Space
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        
        //GameManager.instance().Print("EMT screenPos: " + screenPos + ", worldPos: " + worldPos + ", lossyScale: " + LocationReference.transform.lossyScale, 1);

        transform.position = new Vector3(worldPos.x, worldPos.y, -0.02f);
    }

    // Update is called once per frame
    void Update()
    {
        if(isEMTed)
        {
            float step = speed * Time.unscaledDeltaTime; // Move distance per frame
            transform.position = Vector3.MoveTowards(transform.position, EMTTarget, step);
            if (Vector3.Distance(transform.position, EMTTarget) < 0.001f)
            {
                isEMTed = false; // Reached the target
                GameManager.instance().SetEMT(); // Trigger the EMT event in the GameManager
                this.GetComponent<SpriteRenderer>().enabled = false; // Hide the EMT sprite
            }
        }
    }
    public void EMTMe(Vector3 EMTPosition)
    {
        LocationReference = GameObject.Find("EMTLocation");
        // 1. Get the screen position of your UI element
        Vector3 screenPos = LocationReference.transform.position;
        // 2. Set the Z distance (how far into the world from the camera)
        // For perspective cameras, Z MUST be > 0 (e.g., NearClipPlane or a fixed distance)
        screenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        // 3. Convert to World Space
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        transform.position = new Vector3(worldPos.x, worldPos.y, -0.02f);
        this.GetComponent<SpriteRenderer>().enabled = true; // Ensure the EMT sprite is visible
        isEMTed = true;
        EMTTarget = new Vector3(EMTPosition.x, EMTPosition.y, -0.02f);
    }
}
