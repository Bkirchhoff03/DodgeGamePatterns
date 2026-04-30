using TMPro;
using UnityEngine;

public class EMT : MonoBehaviour
{
    GameObject LocationReference;
    private bool isEMTed;
    private Vector3 EMTTarget;
    private float speed = 5f; // Speed at which the EMT moves towards the target
    private static EMT instance_;
    GameObject lifeCounter;
    bool flashingLife = false;
    int flashCount = 0;
    float flashDuration = 0.5f; // Duration of each flash
    public static EMT instance() => instance_;
    void Awake()
    {
        instance_ = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lifeCounter = GameObject.Find("LifeCounter");
        isEMTed = false;
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
        GetComponent<SpriteRenderer>().enabled = false; // Hide the EMT sprite at the start
    }

    // Update is called once per frame
    void Update()
    {
        if (isEMTed)
        {
            float step = speed * Time.unscaledDeltaTime; // Move distance per frame
            transform.position = Vector3.MoveTowards(transform.position, EMTTarget, step);
            flashDuration -= Time.unscaledDeltaTime;
            if(flashDuration <= 0f)
            {
                flashDuration = 0.5f; // Reset flash duration
                SpriteRenderer sr = GetComponent<SpriteRenderer>();
                GetComponent<SpriteRenderer>().color = sr.color == Color.red ? Color.white : Color.red;
            }
            if (Vector3.Distance(transform.position, EMTTarget) < 0.001f)
            {
                isEMTed = false; // Reached the target
                GameManager.instance().SetEMT(); // Trigger the EMT event in the GameManager
                this.GetComponent<SpriteRenderer>().enabled = false; // Hide the EMT sprite
                GetComponent<SpriteRenderer>().color = Color.red; // Reset color to red
                flashDuration = 0f;
            }
        }else if (flashingLife)
        {
            flashDuration -= Time.unscaledDeltaTime;
            if (flashDuration <= 0f)
            {
                TMP_Text tmp = lifeCounter.GetComponent<TMP_Text>();
                lifeCounter.GetComponent<TMP_Text>().color = tmp.color == Color.red ? Color.white : Color.red;

                flashDuration = 0.5f; // Reset flash duration
                if (tmp.color == Color.red)
                {
                    flashCount++;
                    if (flashCount >= 6) // Flash 3 times (6 toggles)
                    {
                        flashingLife = false;
                        lifeCounter.GetComponent<TMP_Text>().color = Color.white; // Ensure it's white at the end
                    }
                }
            }
        }
    }
    public void EMTMe(Vector3 EMTPosition)
    {
        //GameManager.instance().Print("EMTMe called with position: " + EMTPosition, 1);
        EMTToLife();
        //GameManager.instance().Print("EMT moved to life location (" + transform.position + "), now moving to EMT position: " + EMTPosition, 1);
        this.GetComponent<SpriteRenderer>().enabled = true; // Ensure the EMT sprite is visible
        isEMTed = true;
        EMTTarget = new Vector3(EMTPosition.x, EMTPosition.y, -0.02f);
        //GameManager.instance().Print("isEMTed: " + isEMTed + ", EMT target set to: " + EMTTarget, 1);
    }
    private void EMTToLife()
    {
        LocationReference = GameObject.Find("EMTLocation");
        // 1. Get the screen position of your UI element
        Vector3 screenPos = LocationReference.transform.position;
        // 2. Set the Z distance (how far into the world from the camera)
        // For perspective cameras, Z MUST be > 0 (e.g., NearClipPlane or a fixed distance)
        screenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        // 3. Convert to World Space
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        GameManager.instance().Print("sending EMT to " + worldPos, 1);

        transform.position = new Vector3(worldPos.x, worldPos.y, -0.02f);
    }
    public void EMTOnOneLife()
    {
        flashDuration = 0.5f; // Reset flash duration
        flashCount = 0; // Reset flash count
        flashingLife = true; // Start flashing
    }
}
