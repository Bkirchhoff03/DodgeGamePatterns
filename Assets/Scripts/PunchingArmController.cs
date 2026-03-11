using UnityEngine;

public class PunchingArmController : MonoBehaviour
{
    private GameObject player;
    private bool isPunchingLeft = false;
    private bool isPunchingRight = false;
    private float punchingVelocity;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerCenter = player.transform.position;
        if (isPunchingRight)
        {
            ExecuteRightPunch();
            if (transform.position.x <= playerCenter.x)
            {
                isPunchingRight = false;
                transform.position = playerCenter;
            }
        }
        else if (isPunchingLeft)
        {
            ExecutePunchLeft();
            if (transform.position.x >= playerCenter.x)
            {
                isPunchingLeft = false;
                transform.position = playerCenter;
            }
        }
        else
        {
            GetComponent<Collider2D>().enabled = false;
            transform.position = playerCenter;
        }
        transform.position = new Vector3(transform.position.x, playerCenter.y, playerCenter.z);
    }

    public void PunchRight()
    {
        if (isPunchingRight)
        {
            return;
        }
        GetComponent<Collider2D>().enabled = true;
        isPunchingRight = true;
        isPunchingLeft = false;
        punchingVelocity = 3.0f;
    }
    private void ExecuteRightPunch()
    {
        if(transform.position.x >= player.transform.position.x + 0.5f)
        {
            punchingVelocity = -3.0f;
        }
        //punchingVelocity -= Time.deltaTime * 3.0f;
        transform.localPosition += new Vector3(punchingVelocity * Time.deltaTime, 0.0f);
    }
    public void PunchLeft()
    {
        if (isPunchingLeft)
        {
            return;
        }
        GetComponent<Collider2D>().enabled = true;
        isPunchingLeft = true;
        isPunchingRight = false;
        punchingVelocity = -3.0f;
    }
    public void ExecutePunchLeft()
    {
        if (transform.position.x <= player.transform.position.x - 0.5f)
        {
            punchingVelocity = 3.0f;
        }
        //punchingVelocity += Time.deltaTime * 3.0f;
        transform.localPosition += new Vector3(punchingVelocity * Time.deltaTime, 0.0f);
    }
    public float getPunchingVelocity()
    {
        if (isPunchingLeft)
        {
            if (punchingVelocity < 0)
            {
                return punchingVelocity;
            }
            else
            {
                return 0f;
            }
        }
        else if (isPunchingRight)
        {
            if (punchingVelocity > 0)
            {
                return punchingVelocity;
            }
            else
            {
                return 0f;
            }
        }
        return 0f;
    }
    public void CancelPunch()
    {
        isPunchingLeft = false;
        isPunchingRight = false;
        GetComponent<Collider2D>().enabled = false;
        transform.position = player.transform.position;
    }
}
